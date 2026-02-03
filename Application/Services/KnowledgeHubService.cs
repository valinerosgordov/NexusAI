using NexusAI.Application.Interfaces;
using NexusAI.Domain;
using NexusAI.Domain.Models;
using NexusAI.Infrastructure.Services;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace NexusAI.Application.Services;

public sealed class KnowledgeHubService
{
    // ConcurrentBag is thread-safe for Add/Enumerate operations
    // No additional locks needed - rely on built-in thread safety
    private readonly ConcurrentBag<SourceDocument> _sources = [];
    private readonly ConcurrentBag<ChatMessage> _chatHistory = [];
    private readonly DocumentParserFactory _parserFactory;
    private readonly IObsidianService _obsidianService;
    private IAiService _aiService;

    private const int MaxInputTokens = 1_000_000;
    private const int CharsPerToken = 4;
    private const int MaxContextChars = MaxInputTokens * CharsPerToken;

    // ConcurrentBag.ToArray() is thread-safe - creates snapshot at point in time
    public IReadOnlyList<SourceDocument> Sources => _sources.ToArray();

    public IReadOnlyList<ChatMessage> ChatHistory => _chatHistory.ToArray();
    public bool WasLastContextTruncated { get; private set; }
    public int LastContextTokenCount { get; private set; }

    public KnowledgeHubService(
        DocumentParserFactory parserFactory,
        IObsidianService obsidianService,
        IAiService aiService)
    {
        _parserFactory = parserFactory;
        _obsidianService = obsidianService;
        _aiService = aiService;
    }

    public void SetAiService(IAiService aiService)
    {
        _aiService = aiService;
    }

    public async Task<Result<SourceDocument>> AddDocumentAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var parser = _parserFactory.GetParser(filePath);
        
        if (parser is null)
        {
            var extension = Path.GetExtension(filePath);
            return Result.Failure<SourceDocument>($"Unsupported file type: {extension}");
        }

        var result = await parser.ParseAsync(filePath, cancellationToken);

        if (result.IsSuccess)
        {
            _sources.Add(result.Value); // ConcurrentBag.Add is thread-safe
        }

        return result;
    }

    public async Task<Result<int>> LoadObsidianVaultAsync(string vaultPath, string? subfolder = null, CancellationToken cancellationToken = default)
    {
        var result = await _obsidianService.LoadNotesAsync(vaultPath, subfolder, cancellationToken);

        if (result.IsSuccess)
        {
            foreach (var doc in result.Value)
            {
                _sources.Add(doc); // ConcurrentBag.Add is thread-safe
            }
            return Result.Success(result.Value.Length);
        }

        return Result.Failure<int>(result.Error);
    }

    public void ToggleSourceInclusion(SourceDocumentId id)
    {
        // ConcurrentBag doesn't support in-place updates efficiently
        // Create new bag with updated item (acceptable for small collections)
        var sourcesList = _sources.ToList();
        var source = sourcesList.FirstOrDefault(s => s.Id == id);
        
        if (source is not null)
        {
            var updated = source with { IsIncluded = !source.IsIncluded };
            _sources.Clear(); // Clear is thread-safe
            
            foreach (var s in sourcesList)
            {
                _sources.Add(s.Id == id ? updated : s);
            }
        }
    }

    public void RemoveSource(SourceDocumentId id)
    {
        var filtered = _sources.Where(s => s.Id != id).ToList();
        _sources.Clear(); // Thread-safe
        
        foreach (var s in filtered)
        {
            _sources.Add(s);
        }
    }

    public void ClearSources()
    {
        _sources.Clear(); // ConcurrentBag.Clear is thread-safe
    }

    public async Task<Result<ChatMessage>> AskQuestionAsync(string question, CancellationToken cancellationToken = default)
    {
        return await AskQuestionWithImagesAsync(question, null, cancellationToken);
    }

    public async Task<Result<ChatMessage>> AskQuestionWithImagesAsync(
        string question, 
        string[]? base64Images = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(question))
            return Result.Failure<ChatMessage>("Question cannot be empty");

        var includedSources = _sources.Where(s => s.IsIncluded).ToArray();

        if (includedSources.Length == 0)
            return Result.Failure<ChatMessage>("No sources are currently included. Please add and include at least one document.");

        var userMessage = new ChatMessage(
            Id: ChatMessageId.NewId(),
            Content: question,
            Role: MessageRole.User,
            Timestamp: DateTime.UtcNow
        );

        _chatHistory.Add(userMessage); // Thread-safe

        var context = AggregateContext(includedSources);
        

        Result<AiResponse> aiResult;
        if (base64Images is not null && _aiService is GeminiAiService gemini)
        {
            aiResult = await gemini.AskQuestionWithImagesAsync(question, context, base64Images, cancellationToken);
        }
        else
        {
            aiResult = await _aiService.AskQuestionAsync(question, context, cancellationToken);
        }

        if (aiResult.IsFailure)
        {
            return Result.Failure<ChatMessage>(aiResult.Error);
        }

        var assistantMessage = new ChatMessage(
            Id: ChatMessageId.NewId(),
            Content: aiResult.Value.Content,
            Role: MessageRole.Assistant,
            Timestamp: DateTime.UtcNow,
            SourceCitations: aiResult.Value.SourcesCited
        );

        _chatHistory.Add(assistantMessage); // Thread-safe

        return Result.Success(assistantMessage);
    }

    public async Task<Result<string>> ExportToObsidianAsync(
        string vaultPath,
        string title,
        string content,
        CancellationToken cancellationToken = default)
    {
        var includedSources = _sources.Where(s => s.IsIncluded).ToArray();

        var sourceLinks = includedSources
            .Select(s => $"- [[{s.Name}]]")
            .ToArray();

        return await _obsidianService.SaveNoteAsync(vaultPath, title, content, sourceLinks, cancellationToken);
    }

    public async Task<Result<ChatMessage>> GenerateDeepDiveAsync(CancellationToken cancellationToken = default)
    {
        var includedSources = _sources.Where(s => s.IsIncluded).ToArray();

        if (includedSources.Length == 0)
            return Result.Failure<ChatMessage>("No sources are currently included. Please add and include at least one document.");

        var deepDivePrompt = CreateDeepDivePrompt(includedSources);
        var context = AggregateContext(includedSources);
        
        var aiResult = await _aiService.AskQuestionAsync(deepDivePrompt, context, cancellationToken);

        if (aiResult.IsFailure)
        {
            return Result.Failure<ChatMessage>(aiResult.Error);
        }

        var deepDiveMessage = new ChatMessage(
            Id: ChatMessageId.NewId(),
            Content: aiResult.Value.Content,
            Role: MessageRole.Assistant,
            Timestamp: DateTime.UtcNow,
            SourceCitations: aiResult.Value.SourcesCited
        );

        _chatHistory.Add(deepDiveMessage); // Thread-safe

        return Result.Success(deepDiveMessage);
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

    public async Task<Result<Artifact>> GenerateArtifactAsync(ArtifactType type, CancellationToken cancellationToken = default)
    {
        var includedSources = _sources.Where(s => s.IsIncluded).ToArray();

        if (includedSources.Length == 0)
            return Result.Failure<Artifact>("No sources are currently included. Please add and include at least one document.");

        var prompt = CreateArtifactPrompt(type, includedSources);
        var context = AggregateContext(includedSources);
        
        var aiResult = await _aiService.AskQuestionAsync(prompt, context, cancellationToken);

        if (aiResult.IsFailure)
        {
            return Result.Failure<Artifact>(aiResult.Error);
        }

        var artifact = new Artifact(
            Id: ArtifactId.NewId(),
            Type: type,
            Title: GetArtifactTitle(type),
            Content: aiResult.Value.Content,
            GeneratedAt: DateTime.UtcNow,
            SourceNames: includedSources.Select(s => s.Name).ToArray()
        );

        return Result.Success(artifact);
    }

    public async Task<Result<string[]>> GenerateFollowUpQuestionsAsync(string lastQuestion, string lastAnswer, CancellationToken cancellationToken = default)
    {
        var prompt = $"""
            Based on the previous question and answer, suggest 3 relevant follow-up questions that would help deepen understanding of the topic.
            
            Previous Question: {lastQuestion}
            Previous Answer: {lastAnswer}
            
            Generate 3 concise, specific follow-up questions (one per line, no numbering).
            Each question should explore a different angle or dig deeper into the topic.
            """;

        var aiResult = await _aiService.AskQuestionAsync(prompt, string.Empty, cancellationToken);

        if (aiResult.IsFailure)
        {
            return Result.Failure<string[]>(aiResult.Error);
        }

        var questions = aiResult.Value.Content
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(q => !string.IsNullOrWhiteSpace(q))
            .Take(3)
            .ToArray();

        return Result.Success(questions);
    }

    public void ClearChatHistory()
    {
        _chatHistory.Clear(); // ConcurrentBag.Clear is thread-safe
    }

    private static string CreateArtifactPrompt(ArtifactType type, SourceDocument[] sources)
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
    private string AggregateContext(SourceDocument[] sources)
    {
        var sb = new StringBuilder();
        int totalChars = 0;
        int added = 0;
        WasLastContextTruncated = false;

        foreach (var source in sources)
        {
            var block = $"""
                <Source filename="{source.Name}">
                {source.Content}
                </Source>

                """;
            
            if (totalChars + block.Length > MaxContextChars)
            {
                WasLastContextTruncated = true;
                break;
            }
            sb.Append(block);
            totalChars += block.Length;
            added++;
        }

        if (WasLastContextTruncated)
        {
            sb.AppendLine($"\n⚠️ Context truncated. {sources.Length - added} source(s) omitted due to token limit.");
        }

        LastContextTokenCount = totalChars / CharsPerToken;

        return sb.ToString();
    }

    public string? GetContextWarning()
    {
        if (WasLastContextTruncated)
        {
            return $"⚠️ Context limit reached! Using {LastContextTokenCount:N0} / {MaxInputTokens:N0} tokens. Some sources were excluded.";
        }

        var utilizationPercent = (LastContextTokenCount / (double)MaxInputTokens) * 100;

        if (utilizationPercent > 80)
        {
            return $"⚠️ High context usage: {LastContextTokenCount:N0} / {MaxInputTokens:N0} tokens ({utilizationPercent:F1}%). Consider reducing sources.";
        }

        if (utilizationPercent > 50)
        {
            return $"ℹ️ Context usage: {LastContextTokenCount:N0} / {MaxInputTokens:N0} tokens ({utilizationPercent:F1}%).";
        }

        return null;
    }
}
