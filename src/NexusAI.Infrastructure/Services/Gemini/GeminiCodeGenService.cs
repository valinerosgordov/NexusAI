using System.Text.Json;
using System.Text.Json.Serialization;
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Infrastructure.Services.Gemini;

internal sealed class GeminiCodeGenService(GeminiHttpClient httpClient)
{
#pragma warning disable MA0051
    public async Task<Result<ProjectPlanTask[]>> GeneratePlanAsync(
        string idea,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idea))
            return Result.Failure<ProjectPlanTask[]>("Project idea cannot be empty");

        var prompt = $"""
            Generate a detailed project plan for the following idea:
            
            "{idea}"
            
            Break down the project into specific actionable tasks. For each task, specify:
            - title: Clear, actionable task description
            - role: Who should do it (Frontend, Backend, DevOps, Designer, QA, etc.)
            - hours: Estimated hours (realistic, 0.5-40 hours per task)
            
            Return 5-15 tasks. Be specific and practical.
            """;

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
                temperature = 0.7,
                response_mime_type = "application/json",
                response_schema = new
                {
                    type = "object",
                    properties = new
                    {
                        tasks = new
                        {
                            type = "array",
                            items = new
                            {
                                type = "object",
                                properties = new
                                {
                                    title = new { type = "string" },
                                    role = new { type = "string" },
                                    hours = new { type = "number" }
                                },
                                required = new[] { "title", "role", "hours" }
                            }
                        }
                    },
                    required = new[] { "tasks" }
                }
            }
        };

        var result = await httpClient.SendRequestAsync(body, cancellationToken).ConfigureAwait(false);

        if (!result.IsSuccess)
            return Result.Failure<ProjectPlanTask[]>(result.Error);

        var geminiResponse = result.Value;

        if (geminiResponse.Candidates is not { Length: > 0 } candidates)
            return Result.Failure<ProjectPlanTask[]>("No candidates in API response");

        var firstCandidate = candidates[0];
        if (firstCandidate?.Content?.Parts is not { Length: > 0 } parts)
            return Result.Failure<ProjectPlanTask[]>("No content parts in API response");

        var jsonText = parts[0]?.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(jsonText))
            return Result.Failure<ProjectPlanTask[]>("Empty JSON response from AI");

        try
        {
            var planResponse = JsonSerializer.Deserialize<PlanJsonResponse>(jsonText);
            if (planResponse?.Tasks is not { Length: > 0 })
                return Result.Failure<ProjectPlanTask[]>("No tasks generated");

            var tasks = planResponse.Tasks
                .Select(t => new ProjectPlanTask(t.Title, t.Role, t.Hours))
                .ToArray();

            return Result.Success(tasks);
        }
        catch (JsonException ex)
        {
            return Result.Failure<ProjectPlanTask[]>($"Failed to parse JSON: {ex.Message}");
        }
    }
#pragma warning restore MA0051

#pragma warning disable MA0051
    public async Task<Result<ScaffoldFile[]>> GenerateScaffoldAsync(
        string projectDescription,
        string[] technologies,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(projectDescription))
            return Result.Failure<ScaffoldFile[]>("Project description cannot be empty");

        var techStack = technologies is { Length: > 0 }
            ? string.Join(", ", technologies)
            : "appropriate technologies";

        var prompt = $"""
            Generate a complete project scaffold/boilerplate for the following project:
            
            Description: {projectDescription}
            Technologies: {techStack}
            
            Create a realistic, production-ready file structure with:
            - Configuration files (package.json, .csproj, etc.)
            - README.md with setup instructions
            - Folder structure (src/, tests/, docs/, etc.)
            - Essential code files with boilerplate code
            - .gitignore
            - Environment file templates
            
            For each file:
            - path: Relative path from project root (e.g., "src/Program.cs")
            - content: Full file content (use realistic, working code)
            
            For directories only (no files inside yet):
            - path: Directory path ending with / (e.g., "assets/")
            - content: Empty string
            
            Generate 10-30 files/folders. Be thorough but practical.
            """;

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
                temperature = 0.5,
                response_mime_type = "application/json",
                response_schema = new
                {
                    type = "object",
                    properties = new
                    {
                        files = new
                        {
                            type = "array",
                            items = new
                            {
                                type = "object",
                                properties = new
                                {
                                    path = new { type = "string" },
                                    content = new { type = "string" }
                                },
                                required = new[] { "path", "content" }
                            }
                        }
                    },
                    required = new[] { "files" }
                }
            }
        };

        var result = await httpClient.SendRequestAsync(body, cancellationToken).ConfigureAwait(false);

        if (!result.IsSuccess)
            return Result.Failure<ScaffoldFile[]>(result.Error);

        var geminiResponse = result.Value;

        if (geminiResponse.Candidates is not { Length: > 0 } candidates)
            return Result.Failure<ScaffoldFile[]>("No candidates in API response");

        var firstCandidate = candidates[0];
        if (firstCandidate?.Content?.Parts is not { Length: > 0 } parts)
            return Result.Failure<ScaffoldFile[]>("No content parts in API response");

        var jsonText = parts[0]?.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(jsonText))
            return Result.Failure<ScaffoldFile[]>("Empty JSON response from AI");

        try
        {
            var scaffoldResponse = JsonSerializer.Deserialize<ScaffoldJsonResponse>(jsonText);
            if (scaffoldResponse?.Files is not { Length: > 0 })
                return Result.Failure<ScaffoldFile[]>("No files generated");

            var files = scaffoldResponse.Files
                .Select(f => new ScaffoldFile(f.Path, f.Content))
                .ToArray();

            return Result.Success(files);
        }
        catch (JsonException ex)
        {
            return Result.Failure<ScaffoldFile[]>($"Failed to parse JSON: {ex.Message}");
        }
    }
#pragma warning restore MA0051

    private sealed record PlanJsonResponse(
        [property: JsonPropertyName("tasks")] PlanTaskJson[] Tasks
    );

    private sealed record PlanTaskJson(
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("hours")] decimal Hours
    );

    private sealed record ScaffoldJsonResponse(
        [property: JsonPropertyName("files")] ScaffoldFileJson[] Files
    );

    private sealed record ScaffoldFileJson(
        [property: JsonPropertyName("path")] string Path,
        [property: JsonPropertyName("content")] string Content
    );
}
