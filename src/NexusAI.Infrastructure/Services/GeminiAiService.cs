using NexusAI.Application.Interfaces;
using NexusAI.Application.Services;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NexusAI.Infrastructure.Services;

public sealed class GeminiAiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly SessionContext _sessionContext;
    private const string ModelName = "gemini-2.0-flash";

    public GeminiAiService(HttpClient httpClient, string apiKey, SessionContext sessionContext)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
        _sessionContext = sessionContext;
    }

    private string GetDynamicSystemPrompt()
    {
        var modePrefix = _sessionContext.CurrentMode switch
        {
            AppMode.Professional => """
                You are an Executive Assistant for a professional. Be concise, professional, and focus on business value.
                Prioritize actionable insights, efficiency, and clear recommendations.
                """,
            AppMode.Student => """
                You are a Socratic Tutor helping a student learn. Explain concepts simply, use analogies, and help with study planning.
                Do not just give answers—teach the underlying principles. Ask guiding questions when appropriate.
                Break down complex topics into digestible parts.
                """,
            _ => string.Empty
        };

        return $"""
            {modePrefix}
            
            You are NexusAI, an expert research assistant with transparent reasoning. Answer strictly based on the provided Context.
            
            CRITICAL - SHOW YOUR THINKING:
            Before answering, document your reasoning process using [STEP] markers:
            - [STEP] Reading document X to find information about Y...
            - [STEP] Found relevant section discussing Z...
            - [STEP] Cross-referencing with document W...
            - [STEP] Synthesizing findings...
            
            ANSWER RULES:
            1. GROUNDING: Derive answers ONLY from the context.
            2. CITATIONS: Cite sources using [Filename] format at the end of statements.
            3. HONESTY: If information is missing, state "I cannot find this in the documents."
            4. FORMATTING: Use Markdown.
            
            Example structure:
            [STEP] Analyzing document1.pdf for information about clean architecture...
            [STEP] Found definition on page 3...
            [STEP] Comparing with examples from document2.pdf...
            [STEP] Concluding based on evidence from both sources...
            
            # Answer
            [Your detailed answer with citations here]
            """;
    }

    public async Task<Result<AiResponse>> AskQuestionAsync(
        string question,
        string context,
        CancellationToken cancellationToken = default)
    {
        return await AskQuestionWithImagesAsync(question, context, [], cancellationToken);
    }

    public async Task<Result<AiResponse>> AskQuestionWithImagesAsync(
        string question,
        string context,
        string[] base64Images,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                return Result.Failure<AiResponse>("API key is not configured");

            if (string.IsNullOrWhiteSpace(question))
                return Result.Failure<AiResponse>("Question cannot be empty");

            var body = CreateRequestBody(question, context, base64Images);
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}:generateContent";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("x-goog-api-key", _apiKey);
            request.Content = JsonContent.Create(body);

            var response = await _httpClient.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return Result.Failure<AiResponse>($"API Error ({response.StatusCode}): {errorContent}");
            }

            var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (geminiResponse?.Candidates is not { Length: > 0 } candidates)
                return Result.Failure<AiResponse>("No candidates in API response");

            var firstCandidate = candidates[0];
            if (firstCandidate?.Content?.Parts is not { Length: > 0 } parts)
                return Result.Failure<AiResponse>("No content parts in API response");

            var content = parts[0]?.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(content))
                return Result.Failure<AiResponse>("Empty response from AI");

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

    private object CreateRequestBody(string question, string context, string[] base64Images)
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
        if (base64Images is not null && base64Images.Length > 0)
        {
            foreach (var imageData in base64Images)
            {
                parts.Add(new
                {
                    inlineData = new
                    {
                        mimeType = "image/jpeg",
                        data = imageData
                    }
                });
            }
        }

        return new
        {
            systemInstruction = new
            {
                parts = new[] { new { text = GetDynamicSystemPrompt() } }
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
        return [.. list];
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

    public async Task<Result<ProjectPlanTask[]>> GeneratePlanAsync(
        string idea,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            return Result.Failure<ProjectPlanTask[]>("API key is not configured");

        if (string.IsNullOrWhiteSpace(idea))
            return Result.Failure<ProjectPlanTask[]>("Project idea cannot be empty");

        try
        {
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

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}:generateContent";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("x-goog-api-key", _apiKey);
            request.Content = JsonContent.Create(body);

            var response = await _httpClient.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return Result.Failure<ProjectPlanTask[]>($"API Error ({response.StatusCode}): {errorContent}");
            }

            var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (geminiResponse?.Candidates is not { Length: > 0 } candidates)
                return Result.Failure<ProjectPlanTask[]>("No candidates in API response");

            var firstCandidate = candidates[0];
            if (firstCandidate?.Content?.Parts is not { Length: > 0 } parts)
                return Result.Failure<ProjectPlanTask[]>("No content parts in API response");

            var jsonText = parts[0]?.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(jsonText))
                return Result.Failure<ProjectPlanTask[]>("Empty JSON response from AI");

            var planResponse = System.Text.Json.JsonSerializer.Deserialize<PlanJsonResponse>(jsonText);
            if (planResponse?.Tasks is not { Length: > 0 })
                return Result.Failure<ProjectPlanTask[]>("No tasks generated");

            var tasks = planResponse.Tasks
                .Select(t => new ProjectPlanTask(t.Title, t.Role, t.Hours))
                .ToArray();

            return Result.Success(tasks);
        }
        catch (HttpRequestException ex)
        {
            return Result.Failure<ProjectPlanTask[]>($"Network error: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return Result.Failure<ProjectPlanTask[]>("Request timeout");
        }
        catch (Exception ex)
        {
            return Result.Failure<ProjectPlanTask[]>($"Unexpected error: {ex.Message}");
        }
    }

    private sealed record PlanJsonResponse(
        [property: JsonPropertyName("tasks")] PlanTaskJson[] Tasks
    );

    private sealed record PlanTaskJson(
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("hours")] decimal Hours
    );

    public async Task<Result<ScaffoldFile[]>> GenerateScaffoldAsync(
        string projectDescription,
        string[] technologies,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            return Result.Failure<ScaffoldFile[]>("API key is not configured");

        if (string.IsNullOrWhiteSpace(projectDescription))
            return Result.Failure<ScaffoldFile[]>("Project description cannot be empty");

        try
        {
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

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}:generateContent";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("x-goog-api-key", _apiKey);
            request.Content = JsonContent.Create(body);

            var response = await _httpClient.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return Result.Failure<ScaffoldFile[]>($"API Error ({response.StatusCode}): {errorContent}");
            }

            var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (geminiResponse?.Candidates is not { Length: > 0 } candidates)
                return Result.Failure<ScaffoldFile[]>("No candidates in API response");

            var firstCandidate = candidates[0];
            if (firstCandidate?.Content?.Parts is not { Length: > 0 } parts)
                return Result.Failure<ScaffoldFile[]>("No content parts in API response");

            var jsonText = parts[0]?.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(jsonText))
                return Result.Failure<ScaffoldFile[]>("Empty JSON response from AI");

            var scaffoldResponse = System.Text.Json.JsonSerializer.Deserialize<ScaffoldJsonResponse>(jsonText);
            if (scaffoldResponse?.Files is not { Length: > 0 })
                return Result.Failure<ScaffoldFile[]>("No files generated");

            var files = scaffoldResponse.Files
                .Select(f => new ScaffoldFile(f.Path, f.Content))
                .ToArray();

            return Result.Success(files);
        }
        catch (HttpRequestException ex)
        {
            return Result.Failure<ScaffoldFile[]>($"Network error: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return Result.Failure<ScaffoldFile[]>("Request timeout");
        }
        catch (Exception ex)
        {
            return Result.Failure<ScaffoldFile[]>($"Unexpected error: {ex.Message}");
        }
    }

    private sealed record ScaffoldJsonResponse(
        [property: JsonPropertyName("files")] ScaffoldFileJson[] Files
    );

    private sealed record ScaffoldFileJson(
        [property: JsonPropertyName("path")] string Path,
        [property: JsonPropertyName("content")] string Content
    );

    public async Task<Result<string>> GenerateMermaidDiagramAsync(
        string projectContext,
        string diagramType,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            return Result.Failure<string>("API key is not configured");

        if (string.IsNullOrWhiteSpace(projectContext))
            return Result.Failure<string>("Project context cannot be empty");

        try
        {
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

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}:generateContent";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("x-goog-api-key", _apiKey);
            request.Content = JsonContent.Create(body);

            var response = await _httpClient.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return Result.Failure<string>($"API Error ({response.StatusCode}): {errorContent}");
            }

            var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (geminiResponse?.Candidates is not { Length: > 0 } candidates)
                return Result.Failure<string>("No candidates in API response");

            var firstCandidate = candidates[0];
            if (firstCandidate?.Content?.Parts is not { Length: > 0 } parts)
                return Result.Failure<string>("No content parts in API response");

            var mermaidSyntax = parts[0]?.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(mermaidSyntax))
                return Result.Failure<string>("Empty response from AI");

            // Extract Mermaid code from markdown code blocks if present
            var cleanedSyntax = ExtractMermaidCode(mermaidSyntax);

            return Result.Success(cleanedSyntax);
        }
        catch (HttpRequestException ex)
        {
            return Result.Failure<string>($"Network error: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return Result.Failure<string>("Request timeout");
        }
        catch (Exception ex)
        {
            return Result.Failure<string>($"Unexpected error: {ex.Message}");
        }
    }

    private static string ExtractMermaidCode(string text)
    {
        // Remove markdown code blocks if present
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

    public async Task<Result<WikiStructure[]>> GenerateWikiStructureAsync(
        string topic,
        SourceDocument[] sources,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            return Result.Failure<WikiStructure[]>("API key is not configured");

        if (string.IsNullOrWhiteSpace(topic))
            return Result.Failure<WikiStructure[]>("Topic cannot be empty");

        if (sources is not { Length: > 0 })
            return Result.Failure<WikiStructure[]>("At least one source document is required");

        try
        {
            var context = BuildWikiContext(sources);

            var exampleJson = @"[
  {
    ""title"": ""Introduction to " + topic + @""",
    ""content"": ""# Introduction\n\nDetailed markdown content..."",
    ""sub_pages"": [
      {
        ""title"": ""Core Concepts"",
        ""content"": ""## Core Concepts\n\n..."",
        ""sub_pages"": []
      }
    ]
  }
]";

            var prompt = $@"You are a Technical Writer creating comprehensive documentation.

Topic: {topic}

Based on the provided documents, create a structured hierarchy of documentation pages.

Context from documents:
{context}

Requirements:
1. Create a logical, hierarchical structure (3-5 main pages, each with 2-4 sub-pages)
2. Each page must have:
   - title: Clear, descriptive title
   - content: Comprehensive Markdown content (300-800 words) based HEAVILY on the provided context
   - sub_pages: Array of child pages (can be empty)
3. Use headings, lists, code blocks, and emphasis in the content
4. Extract specific facts, examples, and details from the source documents
5. Cross-reference related concepts
6. Organize content from general to specific (overview -> details -> advanced topics)

Return ONLY valid JSON (no markdown wrapper):
{exampleJson}";

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
                    temperature = 0.4,
                    response_mime_type = "application/json",
                    response_schema = new
                    {
                        type = "array",
                        items = new
                        {
                            type = "object",
                            properties = new
                            {
                                title = new { type = "string" },
                                content = new { type = "string" },
                                sub_pages = new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            title = new { type = "string" },
                                            content = new { type = "string" },
                                            sub_pages = new
                                            {
                                                type = "array",
                                                items = new { type = "object" }
                                            }
                                        },
                                        required = new[] { "title", "content", "sub_pages" }
                                    }
                                }
                            },
                            required = new[] { "title", "content", "sub_pages" }
                        }
                    }
                }
            };

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}:generateContent";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("x-goog-api-key", _apiKey);
            request.Content = JsonContent.Create(body);

            var response = await _httpClient.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return Result.Failure<WikiStructure[]>($"API Error ({response.StatusCode}): {errorContent}");
            }

            var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (geminiResponse?.Candidates is not { Length: > 0 } candidates)
                return Result.Failure<WikiStructure[]>("No candidates in API response");

            var firstCandidate = candidates[0];
            if (firstCandidate?.Content?.Parts is not { Length: > 0 } parts)
                return Result.Failure<WikiStructure[]>("No content parts in API response");

            var jsonText = parts[0]?.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(jsonText))
                return Result.Failure<WikiStructure[]>("Empty JSON response from AI");

            var wikiStructures = System.Text.Json.JsonSerializer.Deserialize<WikiStructureJson[]>(jsonText);
            if (wikiStructures is not { Length: > 0 })
                return Result.Failure<WikiStructure[]>("No wiki structures generated");

            var result = ConvertToWikiStructures(wikiStructures);

            return Result.Success(result);
        }
        catch (HttpRequestException ex)
        {
            return Result.Failure<WikiStructure[]>($"Network error: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return Result.Failure<WikiStructure[]>("Request timeout");
        }
        catch (Exception ex)
        {
            return Result.Failure<WikiStructure[]>($"Unexpected error: {ex.Message}");
        }
    }

    private static string BuildWikiContext(SourceDocument[] sources)
    {
        var maxCharsPerDoc = 3000;
        var contextParts = sources.Take(5).Select(doc =>
        {
            var content = doc.Content.Length > maxCharsPerDoc
                ? doc.Content[..maxCharsPerDoc] + "..."
                : doc.Content;
            return $"--- Document: {doc.FilePath} ---\n{content}\n";
        });

        return string.Join("\n\n", contextParts);
    }

    private static WikiStructure[] ConvertToWikiStructures(WikiStructureJson[] jsonStructures)
    {
        return jsonStructures.Select(json => new WikiStructure(
            json.Title,
            json.Content,
            ConvertToWikiStructures(json.SubPages ?? [])
        )).ToArray();
    }

    private sealed record WikiStructureJson(
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("content")] string Content,
        [property: JsonPropertyName("sub_pages")] WikiStructureJson[]? SubPages
    );

    public async Task<Result<SlideContent[]>> GeneratePresentationStructureAsync(
        string topic,
        int slideCount,
        SourceDocument[] sources,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            return Result.Failure<SlideContent[]>("API key is not configured");

        if (string.IsNullOrWhiteSpace(topic))
            return Result.Failure<SlideContent[]>("Topic cannot be empty");

        if (slideCount < 3 || slideCount > 20)
            return Result.Failure<SlideContent[]>("Slide count must be between 3 and 20");

        var context = BuildPresentationContext(sources);

        var exampleJson = @"[
  {
    ""title"": ""Introduction to Clean Architecture"",
    ""points"": [
      ""Software design philosophy by Robert C. Martin"",
      ""Focus on separation of concerns"",
      ""Key principle: The Dependency Rule""
    ],
    ""notes"": ""Start with a brief overview. Mention that Clean Architecture aims to create maintainable and testable systems.""
  },
  {
    ""title"": ""The Dependency Rule"",
    ""points"": [
      ""Dependencies point inward"",
      ""Inner layers know nothing about outer layers"",
      ""Domain layer is the core""
    ],
    ""notes"": ""Use diagram from document to illustrate the circular dependency flow.""
  }
]";

        var prompt = $$"""
You are a Presentation Expert creating professional PowerPoint slides.

Topic: {{topic}}
Target slide count: {{slideCount}} slides

Based on the provided documents, create a structured outline for a PowerPoint presentation.

Context from documents:
{{context}}

Requirements:
1. Create exactly {{slideCount}} slides
2. First slide should be a title slide with the topic
3. Last slide should be a conclusion/summary
4. Each slide must have:
   - title: Clear, concise slide title (5-8 words)
   - points: Array of 3-5 bullet points per slide (each point 5-15 words)
   - notes: Speaker notes (2-3 sentences providing context and talking points)
5. Extract specific facts and examples from the source documents
6. Ensure logical flow between slides
7. Use professional, clear language

Return ONLY valid JSON array matching this structure:
{{exampleJson}}

CRITICAL: Return ONLY the JSON array, no markdown code blocks, no explanations.
""";

        var requestBody = new
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
                response_mime_type = "application/json",
                response_schema = new
                {
                    type = "array",
                    items = new
                    {
                        type = "object",
                        properties = new
                        {
                            title = new { type = "string" },
                            points = new
                            {
                                type = "array",
                                items = new { type = "string" }
                            },
                            notes = new { type = "string" }
                        },
                        required = new[] { "title", "points", "notes" }
                    }
                }
            }
        };

        try
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}:generateContent";
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("x-goog-api-key", _apiKey);
            request.Content = JsonContent.Create(requestBody);

            var response = await _httpClient.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return Result.Failure<SlideContent[]>($"API Error ({response.StatusCode}): {errorContent}");
            }

            var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>(
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (geminiResponse?.Candidates is not { Length: > 0 })
                return Result.Failure<SlideContent[]>("No candidates in API response");

            var content = geminiResponse.Candidates[0]?.Content?.Parts?[0]?.Text;
            if (string.IsNullOrWhiteSpace(content))
                return Result.Failure<SlideContent[]>("Empty response from AI");

            var slideJsons = JsonSerializer.Deserialize<SlideJson[]>(content);
            if (slideJsons is null || slideJsons.Length == 0)
                return Result.Failure<SlideContent[]>("Failed to parse presentation structure from AI response");

            var slides = slideJsons.Select(json => new SlideContent(
                json.Title,
                json.Points ?? [],
                json.Notes ?? string.Empty
            )).ToArray();

            return Result.Success(slides);
        }
        catch (HttpRequestException ex)
        {
            return Result.Failure<SlideContent[]>($"Network error: {ex.Message}");
        }
        catch (JsonException ex)
        {
            return Result.Failure<SlideContent[]>($"Failed to parse JSON: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result.Failure<SlideContent[]>($"Unexpected error: {ex.Message}");
        }
    }

    private static string BuildPresentationContext(SourceDocument[] sources)
    {
        const int maxCharsPerDoc = 5000;
        const int maxTotalChars = 50000;

        var contextParts = sources
            .Take(10)
            .Select(doc =>
            {
                var content = doc.Content.Length > maxCharsPerDoc
                    ? doc.Content[..maxCharsPerDoc] + "..."
                    : doc.Content;
                return $"--- Document: {doc.Name} ---\n{content}\n";
            });

        var fullContext = string.Join("\n\n", contextParts);

        return fullContext.Length > maxTotalChars
            ? fullContext[..maxTotalChars] + "\n...(content truncated)"
            : fullContext;
    }

    private sealed record SlideJson(
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("points")] string[]? Points,
        [property: JsonPropertyName("notes")] string? Notes
    );
}
