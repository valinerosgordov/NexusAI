using NexusAI.Domain.Common;

namespace NexusAI.Infrastructure.Services.Gemini;

internal sealed class GeminiDiagramService(GeminiHttpClient httpClient)
{
#pragma warning disable MA0051
    public async Task<Result<string>> GenerateMermaidDiagramAsync(
        string projectContext,
        string diagramType,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(projectContext))
            return Result.Failure<string>("Project context cannot be empty");

        var prompt = diagramType.ToLowerInvariant() switch
        {
            "architecture" => $"""
                Generate a Mermaid.js diagram showing the Clean Architecture layers for this project:
                
                Context: {projectContext}
                
                Create a flowchart (graph TD) that shows:
                - Domain layer (entities, value objects)
                - Application layer (use cases, interfaces)
                - Infrastructure layer (services, repositories)
                - Presentation layer (ViewModels, Views)
                - Dependency directions (arrows pointing inward)
                
                Return ONLY the Mermaid syntax (starting with ```mermaid and ending with ```).
                Use clear labels, colors (classDef), and proper Mermaid flowchart syntax.
                """,

            "flow" => $"""
                Generate a Mermaid.js sequence diagram for the main user flow:
                
                Context: {projectContext}
                
                Show the interaction between:
                - User
                - UI (ViewModel)
                - Use Case Handler
                - AI Service
                - Database
                
                Return ONLY the Mermaid syntax (sequenceDiagram).
                """,

            "entity" => $"""
                Generate a Mermaid.js ER diagram (erDiagram) for the database entities:
                
                Context: {projectContext}
                
                Show all entities, their properties, and relationships.
                Return ONLY the Mermaid syntax.
                """,

            _ => $"""
                Generate a Mermaid.js diagram that best represents this project structure:
                
                Context: {projectContext}
                
                Return ONLY the Mermaid syntax (graph TD, sequenceDiagram, or erDiagram).
                """
        };

        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[] { new { text = prompt } }
                }
            },
            generationConfig = new
            {
                temperature = 0.3,
                candidateCount = 1
            }
        };

        var result = await httpClient.SendRequestAsync(body, cancellationToken).ConfigureAwait(false);

        if (!result.IsSuccess)
            return Result.Failure<string>(result.Error);

        var geminiResponse = result.Value;

        if (geminiResponse.Candidates is not { Length: > 0 } candidates)
            return Result.Failure<string>("No candidates in API response");

        var firstCandidate = candidates[0];
        if (firstCandidate?.Content?.Parts is not { Length: > 0 } parts)
            return Result.Failure<string>("No content parts in API response");

        var mermaidSyntax = parts[0]?.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(mermaidSyntax))
            return Result.Failure<string>("Empty response from AI");

        var cleanedSyntax = ExtractMermaidCode(mermaidSyntax);

        return Result.Success(cleanedSyntax);
    }
#pragma warning restore MA0051

    private static string ExtractMermaidCode(string text)
    {
        var cleaned = text.Trim();

        if (cleaned.StartsWith("```mermaid", StringComparison.OrdinalIgnoreCase))
        {
            cleaned = cleaned["```mermaid".Length..];
        }
        else if (cleaned.StartsWith("```", StringComparison.OrdinalIgnoreCase))
        {
            cleaned = cleaned[3..];
        }

        if (cleaned.EndsWith("```", StringComparison.OrdinalIgnoreCase))
        {
            cleaned = cleaned[..^3];
        }

        return cleaned.Trim();
    }
}
