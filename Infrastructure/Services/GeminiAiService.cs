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
        You are NexusAI, an expert research assistant. Answer strictly based on the provided Context.
        RULES:
        1. GROUNDING: Derive answers ONLY from the context.
        2. CITATIONS: Cite sources using [Filename] format at the end of statements.
        3. HONESTY: If information is missing, state "I cannot find this in the documents."
        4. FORMATTING: Use Markdown.
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
        return await AskQuestionWithImagesAsync(question, context, null, cancellationToken);
    }

    public async Task<Result<AiResponse>> AskQuestionWithImagesAsync(
        string question,
        string context,
        string[]? base64Images = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_apiKeyHolder.ApiKey))
                return Result.Failure<AiResponse>("API key is not configured");

            if (string.IsNullOrWhiteSpace(question))
                return Result.Failure<AiResponse>("Question cannot be empty");

            var body = CreateRequestBody(question, context, base64Images);
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

    private static object CreateRequestBody(string question, string context, string[]? base64Images = null)
    {
        var fullPrompt = string.IsNullOrWhiteSpace(context)
            ? question
            : $"""
              CONTEXT:
              {context}

              USER QUESTION:
              {question}
              """;

        var parts = new List<object> { new { text = fullPrompt } };

        // Add images if provided
        if (base64Images is not null && base64Images.Length > 0)
        {
            foreach (var imageData in base64Images)
            {
                parts.Add(new
                {
                    inlineData = new
                    {
                        mimeType = "image/jpeg", // or detect from base64
                        data = imageData
                    }
                });
            }
        }

        return new
        {
            systemInstruction = new
            {
                parts = new[] { new { text = SystemPrompt } }
            },
            contents = new[]
            {
                new
                {
                    parts = parts.ToArray()
                }
            },
            generationConfig = new
            {
                temperature = 0.3,
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
