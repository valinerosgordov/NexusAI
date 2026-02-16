#pragma warning disable MA0048
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Artifacts;

public sealed record GenerateArtifactCommand(
    ArtifactType Type,
    SourceDocument[] IncludedSources
);

public sealed class GenerateArtifactHandler
{
    private readonly IAiService _aiService;
    private const int MaxInputTokens = 1_000_000;
    private const int CharsPerToken = 4;
    private const int MaxContextChars = MaxInputTokens * CharsPerToken;

    public GenerateArtifactHandler(IAiService aiService)
    {
        _aiService = aiService;
    }

    public async Task<Result<Artifact>> HandleAsync(
        GenerateArtifactCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.IncludedSources.Length == 0)
            return Result.Failure<Artifact>("No sources are currently included. Please add and include at least one document.");

        var prompt = CreateArtifactPrompt(command.Type, command.IncludedSources);
        var context = AggregateContext(command.IncludedSources);
        
        var aiResult = await _aiService.AskQuestionAsync(prompt, context, cancellationToken).ConfigureAwait(false);

        if (aiResult.IsFailure)
        {
            return Result.Failure<Artifact>(aiResult.Error);
        }

        var artifact = new Artifact(
            Id: ArtifactId.NewId(),
            Type: command.Type,
            Title: GetArtifactTitle(command.Type),
            Content: aiResult.Value.Content,
            GeneratedAt: DateTime.UtcNow,
            SourceNames: command.IncludedSources.Select(s => s.Name).ToArray()
        );

        return Result.Success(artifact);
    }

    private static string AggregateContext(SourceDocument[] sources)
    {
        var sb = new System.Text.StringBuilder();
        int totalChars = 0;

        foreach (var source in sources)
        {
            var block = $"""
                <Source filename="{source.Name}">
                {source.Content}
                </Source>

                """;
            
            if (totalChars + block.Length > MaxContextChars)
                break;
                
            sb.Append(block);
            totalChars += block.Length;
        }

        return sb.ToString();
    }

#pragma warning disable MA0051 // Method length: large switch with string literals for prompts is intentional
    private static string CreateArtifactPrompt(ArtifactType type, SourceDocument[] sources)
#pragma warning restore MA0051
    {
        var names = string.Join(", ", sources.Select(s => $"[{s.Name}]"));
        return type switch
        {
            ArtifactType.FAQ => $"""
                Generate a comprehensive FAQ (Frequently Asked Questions) document based on the provided sources: {names}.
                
                Create 10-15 questions and detailed answers covering:
                - Key concepts and definitions
                - Common misconceptions
                - Practical applications
                - Advanced topics
                - Troubleshooting/common issues
                
                Format each Q&A as:
                **Q: [Question]**
                A: [Detailed answer with citations like [source_name]]
                
                Cite sources for each answer using brackets like [source_name].
                """,

            ArtifactType.StudyGuide => $"""
                Generate a comprehensive Study Guide based on the provided sources: {names}.
                
                Include:
                1. **Learning Objectives**: What should a student know after studying this material?
                2. **Key Terms & Definitions**: Important vocabulary with clear explanations
                3. **Main Concepts**: Core ideas organized hierarchically
                4. **Summary Points**: Bullet-point summaries of each major section
                5. **Practice Questions**: 5-10 questions to test understanding
                6. **Further Reading**: Topics for deeper exploration
                
                Format in clear Markdown with headings and bullet points.
                Cite sources using brackets like [source_name].
                """,

            ArtifactType.PodcastScript => $"""
                Generate an engaging 2-person Podcast Script based on the provided sources: {names}.
                
                Format:
                **Host:** [Enthusiastic introduction and topic overview]
                **Expert:** [Deep insights and explanations]
                **Host:** [Follow-up questions and clarifications]
                **Expert:** [Detailed responses with examples]
                
                Style:
                - Conversational and engaging
                - Host asks clarifying questions
                - Expert provides detailed explanations with citations
                - Include transitions between topics
                - End with key takeaways
                
                Length: ~800-1000 words
                Cite sources naturally in dialogue like "According to [source_name]..."
                """,

            ArtifactType.NotebookGuide => $"""
                Generate a "Notebook Guide" (Путеводитель по блокноту) based on the provided sources: {names}.
                Output ONE Markdown document with exactly these three sections. Use ONLY information from the sources.
                
                ## 1. Краткий обзор (Summary)
                A concise summary (2-4 paragraphs) of the main content and key ideas across all sources. Cite with [source_name].
                
                ## 2. Часто задаваемые вопросы (FAQ)
                5-10 questions and answers based on the sources. Format:
                **Q:** [question]
                **A:** [answer with citation [source_name]]
                
                ## 3. Оглавление и глоссарий (Table of Contents & Glossary)
                - **Key themes:** Bullet list of main topics covered.
                - **Glossary:** Important terms with short definitions (term: definition). Cite source when relevant.
                
                Be accurate. If something is not in the sources, do not include it. Cite every claim with [source_name].
                """,

            ArtifactType.Summary => $"""
                Generate a concise Summary of the provided sources: {names}.
                Include: main idea, key points, and conclusions. Use only information from the sources. Cite with [source_name].
                Format in clear Markdown (2-4 paragraphs).
                """,

            ArtifactType.Outline => $"""
                Generate an Outline (оглавление) and key terms glossary for the provided sources: {names}.
                Part 1: Hierarchical outline of main topics and subtopics. Part 2: Glossary of important terms with brief definitions.
                Use only information from the sources. Cite with [source_name]. Format in Markdown.
                """,

            _ => CreateDeepDivePrompt(sources)
        };
    }

    private static string CreateDeepDivePrompt(SourceDocument[] sources)
    {
        var names = string.Join(", ", sources.Select(s => $"[{s.Name}]"));
        return $"""
            Generate a comprehensive "Deep Dive" analysis of the provided sources: {names}.
            
            Your analysis should include:
            
            1. **Executive Summary**: A concise overview of the main themes and key insights (2-3 paragraphs)
            
            2. **Core Concepts**: Identify and explain the 5-7 most important concepts across all sources
            
            3. **Key Findings**: List the most significant findings, facts, or conclusions from the materials
            
            4. **Connections & Patterns**: Analyze how different sources relate to each other, noting agreements, contradictions, or complementary information
            
            5. **Practical Implications**: What are the actionable takeaways or real-world applications?
            
            6. **Areas for Further Exploration**: What questions remain unanswered? What topics deserve deeper investigation?
            
            Format your response in Markdown with clear headings. Cite sources frequently using brackets like [source_name].
            Be thorough but concise. This is an analytical deep dive, not just a summary.
            """;
    }

    private static string GetArtifactTitle(ArtifactType type) => type switch
    {
        ArtifactType.FAQ => "Frequently Asked Questions",
        ArtifactType.StudyGuide => "Study Guide",
        ArtifactType.PodcastScript => "Podcast Script",
        ArtifactType.NotebookGuide => "Путеводитель по блокноту",
        ArtifactType.Summary => "Summary",
        ArtifactType.Outline => "Outline & Glossary",
        _ => "Deep Dive Analysis"
    };
}
#pragma warning restore MA0048
