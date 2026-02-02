using NexusAI.Application.Interfaces;
using NexusAI.Domain;
using NexusAI.Domain.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NexusAI.Infrastructure.Services;

public sealed class OllamaService : IAiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://localhost:11434";
    private const string SystemPrompt = """
        You are NexusAI, an expert research assistant. Answer strictly based on the provided Context.
        RULES:
        1. GROUNDING: Derive answers ONLY from the context.
        2. CITATIONS: Cite sources using [Filename] format at the end of statements.
        3. HONESTY: If information is missing, state "I cannot find this in the documents."
        4. FORMATTING: Use Markdown.
        """;

    public string SelectedModel { get; set; } = "llama3";

    public OllamaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromMinutes(5); // Ollama can be slow
    }

    public async Task<Result<AiResponse>> AskQuestionAsync(
        string question,
        string context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(question))
                return Result.Failure<AiResponse>("Question cannot be empty");
            if (!await IsOllamaRunningAsync(cancellationToken))
                return Result.Failure<AiResponse>("Ollama is not running. Please start Ollama desktop app.");

            var fullPrompt = BuildPrompt(question, context);
            var requestBody = new
            {
                model = SelectedModel,
                messages = new[]
                {
                    new { role = "system", content = SystemPrompt },
                    new { role = "user", content = fullPrompt }
                },
                stream = false,
                options = new
                {
                    temperature = 0.3,
                    top_p = 0.95,
                    top_k = 40
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{BaseUrl}/api/chat", content, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return Result.Failure<AiResponse>($"Ollama API Error ({response.StatusCode}): {errorContent}");
            }

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(responseJson);

            if (ollamaResponse?.Message?.Content is null)
                return Result.Failure<AiResponse>("No response from Ollama");

            var responseContent = ollamaResponse.Message.Content;
            var sourceCitations = ExtractSourceCitations(responseContent);

            var aiResponse = new AiResponse(
                Content: responseContent,
                SourcesCited: sourceCitations,
                TokensUsed: 0 // Ollama doesn't return token count in non-streaming mode
            );

            return Result.Success(aiResponse);
        }
        catch (HttpRequestException ex)
        {
            return Result.Failure<AiResponse>($"Network error: Is Ollama running? {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return Result.Failure<AiResponse>("Request timeout - model may be too slow or not loaded");
        }
        catch (Exception ex)
        {
            return Result.Failure<AiResponse>($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<string[]>> GetAvailableModelsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/tags", cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                return Result.Failure<string[]>("Failed to fetch models from Ollama");

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var modelsResponse = JsonSerializer.Deserialize<OllamaModelsResponse>(json);

            if (modelsResponse?.Models is null || modelsResponse.Models.Length == 0)
                return Result.Success(Array.Empty<string>());

            var modelNames = modelsResponse.Models.Select(m => m.Name).ToArray();
            return Result.Success(modelNames);
        }
        catch (Exception ex)
        {
            return Result.Failure<string[]>($"Failed to fetch models: {ex.Message}");
        }
    }

    public async Task<bool> IsOllamaRunningAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/tags", cancellationToken)
                .ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private static string BuildPrompt(string question, string context)
    {
        if (string.IsNullOrWhiteSpace(context))
            return question;

        return $"""
               CONTEXT:
               {context}

               USER QUESTION:
               {question}
               """;
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
    private sealed record OllamaResponse(
        [property: JsonPropertyName("message")] OllamaMessage? Message,
        [property: JsonPropertyName("done")] bool Done
    );

    private sealed record OllamaMessage(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("content")] string Content
    );

    private sealed record OllamaModelsResponse(
        [property: JsonPropertyName("models")] OllamaModel[] Models
    );

    private sealed record OllamaModel(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("size")] long Size
    );
}
