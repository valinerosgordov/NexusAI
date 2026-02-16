using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using NexusAI.Domain.Common;
using NexusAI.Infrastructure.Constants;

namespace NexusAI.Infrastructure.Services.Gemini;

internal sealed class GeminiHttpClient(HttpClient httpClient, string apiKey, string modelName)
{
    public async Task<Result<GeminiResponse>> SendRequestAsync<TBody>(
        TBody body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{ApiEndpoints.GeminiGenerateContent(modelName)}?key={apiKey}";
            
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = JsonContent.Create(body);

            var response = await httpClient.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                return Result.Failure<GeminiResponse>($"API Error ({response.StatusCode}): {errorContent}");
            }

            var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>(
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (geminiResponse is null)
                return Result.Failure<GeminiResponse>("Failed to deserialize API response");

            return Result.Success(geminiResponse);
        }
        catch (HttpRequestException ex)
        {
            return Result.Failure<GeminiResponse>($"Network error: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return Result.Failure<GeminiResponse>("Request timeout - check your internet connection or API key");
        }
        catch (Exception ex)
        {
            return Result.Failure<GeminiResponse>($"Unexpected error: {ex.Message}");
        }
    }

    public sealed record GeminiResponse(
        [property: JsonPropertyName("candidates")] Candidate[] Candidates,
        [property: JsonPropertyName("usageMetadata")] UsageMetadata? UsageMetadata
    );

    public sealed record Candidate(
        [property: JsonPropertyName("content")] ContentData Content
    );

    public sealed record ContentData(
        [property: JsonPropertyName("parts")] Part[] Parts
    );

    public sealed record Part(
        [property: JsonPropertyName("text")] string Text
    );

    public sealed record UsageMetadata(
        [property: JsonPropertyName("totalTokenCount")] int TotalTokenCount
    );
}
