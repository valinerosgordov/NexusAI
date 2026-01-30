using NexusAI.Application.Interfaces;
using NexusAI.Domain;
using NexusAI.Domain.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace NexusAI.Infrastructure.Services;

// вызовы Gemini API
public sealed class GeminiAiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly ApiKeyHolder _apiKeyHolder;
    private const string ModelName = "gemini-2.0-flash";
    private const string SystemPrompt = """
        You are a Grounded Research Assistant. You MUST follow these rules:
        1. Answer ONLY using information from the CONTEXT (source documents) provided below. Do not use general knowledge or the internet.
        2. If the answer is not in the provided documents, say clearly: "Этой информации нет в загруженных источниках" (or "This information is not in the provided sources") and do not invent an answer.
        3. For every factual claim, cite the source in square brackets: [source_name]. Use the exact document name from the context.
        4. Be accurate and concise. Prefer citing specific passages when possible.
        """;

    public GeminiAiService(HttpClient httpClient, ApiKeyHolder apiKeyHolder)
    {
        _httpClient = httpClient;
        _apiKeyHolder = apiKeyHolder;
    }

    public async Task<Result<AiResponse>> AskQuestionAsync(
        string question,
        string context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_apiKeyHolder.ApiKey))
                return Result.Failure<AiResponse>("API key is not configured");

            if (string.IsNullOrWhiteSpace(question))
                return Result.Failure<AiResponse>("Question cannot be empty");

            var body = CreateRequestBody(question, context);
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}:generateContent?key={_apiKeyHolder.ApiKey}";

            var response = await _httpClient.PostAsJsonAsync(url, body, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return Result.Failure<AiResponse>($"API Error ({response.StatusCode}): {errorContent}");
            }

            var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (geminiResponse?.Candidates is null || geminiResponse.Candidates.Length == 0)
                return Result.Failure<AiResponse>("No response from AI");

            var content = geminiResponse.Candidates[0].Content.Parts[0].Text;
            var sourceCitations = ExtractSourceCitations(content);
            var tokensUsed = geminiResponse.UsageMetadata?.TotalTokenCount ?? 0;

            var aiResponse = new AiResponse(
                Content: content,
                SourcesCited: sourceCitations,
                TokensUsed: tokensUsed
            );

            return Result.Success(aiResponse);
        }
        catch (HttpRequestException ex)
        {
            return Result.Failure<AiResponse>($"Network error: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return Result.Failure<AiResponse>("Request timeout");
        }
        catch (Exception ex)
        {
            return Result.Failure<AiResponse>($"Unexpected error: {ex.Message}");
        }
    }

    private static object CreateRequestBody(string question, string context)
    {
        var fullPrompt = $"""
            {SystemPrompt}

            CONTEXT (Source Documents):
            {context}

            USER QUESTION:
            {question}
            """;

        return new
        {
            contents = new[]
            {
                new
                {
                    parts = new[] { new { text = fullPrompt } }
                }
            },
            generationConfig = new
            {
                temperature = 0.2,
                topK = 40,
                topP = 0.95,
                maxOutputTokens = 2048
            }
        };
    }

    private static string[] ExtractSourceCitations(string content)
    {
        var list = new List<string>();
        int idx = 0;
        while ((idx = content.IndexOf('[', idx)) != -1)
        {
            var end = content.IndexOf(']', idx);
            if (end == -1) break;
            var cit = content.Substring(idx + 1, end - idx - 1);
            if (!string.IsNullOrWhiteSpace(cit) && !list.Contains(cit))
                list.Add(cit);
            idx = end + 1;
        }
        return list.ToArray();
    }

    private sealed record GeminiResponse(
        [property: JsonPropertyName("candidates")] Candidate[] Candidates,
        [property: JsonPropertyName("usageMetadata")] UsageMetadata? UsageMetadata
    );

    private sealed record Candidate(
        [property: JsonPropertyName("content")] ContentData Content
    );

    private sealed record ContentData(
        [property: JsonPropertyName("parts")] Part[] Parts
    );

    private sealed record Part(
        [property: JsonPropertyName("text")] string Text
    );

    private sealed record UsageMetadata(
        [property: JsonPropertyName("totalTokenCount")] int TotalTokenCount
    );
}
