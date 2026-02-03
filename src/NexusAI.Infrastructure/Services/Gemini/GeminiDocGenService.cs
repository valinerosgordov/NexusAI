using System.Text.Json;
using System.Text.Json.Serialization;
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Infrastructure.Services.Gemini;

internal sealed class GeminiDocGenService(GeminiHttpClient httpClient)
{
    public async Task<Result<WikiStructure[]>> GenerateWikiStructureAsync(
        string topic,
        SourceDocument[] sources,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(topic))
            return Result.Failure<WikiStructure[]>("Topic cannot be empty");

        if (sources is not { Length: > 0 })
            return Result.Failure<WikiStructure[]>("At least one source document is required");

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

        var result = await httpClient.SendRequestAsync(body, cancellationToken);

        if (!result.IsSuccess)
            return Result.Failure<WikiStructure[]>(result.Error);

        var geminiResponse = result.Value;

        if (geminiResponse.Candidates is not { Length: > 0 } candidates)
            return Result.Failure<WikiStructure[]>("No candidates in API response");

        var firstCandidate = candidates[0];
        if (firstCandidate?.Content?.Parts is not { Length: > 0 } parts)
            return Result.Failure<WikiStructure[]>("No content parts in API response");

        var jsonText = parts[0]?.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(jsonText))
            return Result.Failure<WikiStructure[]>("Empty JSON response from AI");

        try
        {
            var wikiStructures = JsonSerializer.Deserialize<WikiStructureJson[]>(jsonText);
            if (wikiStructures is not { Length: > 0 })
                return Result.Failure<WikiStructure[]>("No wiki structures generated");

            var convertedResult = ConvertToWikiStructures(wikiStructures);
            return Result.Success(convertedResult);
        }
        catch (JsonException ex)
        {
            return Result.Failure<WikiStructure[]>($"Failed to parse JSON: {ex.Message}");
        }
    }

    public async Task<Result<SlideContent[]>> GeneratePresentationStructureAsync(
        string topic,
        int slideCount,
        SourceDocument[] sources,
        CancellationToken cancellationToken = default)
    {
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

        var result = await httpClient.SendRequestAsync(requestBody, cancellationToken);

        if (!result.IsSuccess)
            return Result.Failure<SlideContent[]>(result.Error);

        var geminiResponse = result.Value;

        if (geminiResponse.Candidates is not { Length: > 0 })
            return Result.Failure<SlideContent[]>("No candidates in API response");

        var content = geminiResponse.Candidates[0]?.Content?.Parts?[0]?.Text;
        if (string.IsNullOrWhiteSpace(content))
            return Result.Failure<SlideContent[]>("Empty response from AI");

        try
        {
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
        catch (JsonException ex)
        {
            return Result.Failure<SlideContent[]>($"Failed to parse JSON: {ex.Message}");
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

    private sealed record SlideJson(
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("points")] string[]? Points,
        [property: JsonPropertyName("notes")] string? Notes
    );
}
