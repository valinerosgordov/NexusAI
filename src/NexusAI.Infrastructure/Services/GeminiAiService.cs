using NexusAI.Application.Interfaces;
using NexusAI.Application.Services;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;
using NexusAI.Infrastructure.Services.Gemini;

namespace NexusAI.Infrastructure.Services;

public sealed class GeminiAiService : IAiService
{
    private readonly GeminiChatService _chatService;
    private readonly GeminiCodeGenService _codeGenService;
    private readonly GeminiDiagramService _diagramService;
    private readonly GeminiDocGenService _docGenService;

    public GeminiAiService(HttpClient httpClient, string apiKey, SessionContext sessionContext)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key cannot be empty", nameof(apiKey));

        const string modelName = "gemini-2.0-flash";
        var geminiHttpClient = new GeminiHttpClient(httpClient, apiKey, modelName);

        _chatService = new GeminiChatService(geminiHttpClient, sessionContext);
        _codeGenService = new GeminiCodeGenService(geminiHttpClient);
        _diagramService = new GeminiDiagramService(geminiHttpClient);
        _docGenService = new GeminiDocGenService(geminiHttpClient);
    }

    public Task<Result<AiResponse>> AskQuestionAsync(
        string question,
        string context,
        CancellationToken cancellationToken = default)
    {
        return _chatService.AskQuestionAsync(question, context, cancellationToken);
    }

    public Task<Result<AiResponse>> AskQuestionWithImagesAsync(
        string question,
        string context,
        string[] base64Images,
        CancellationToken cancellationToken = default)
    {
        return _chatService.AskQuestionWithImagesAsync(question, context, base64Images, cancellationToken);
    }

    public Task<Result<ProjectPlanTask[]>> GeneratePlanAsync(
        string idea,
        CancellationToken cancellationToken = default)
    {
        return _codeGenService.GeneratePlanAsync(idea, cancellationToken);
    }

    public Task<Result<ScaffoldFile[]>> GenerateScaffoldAsync(
        string projectDescription,
        string[] technologies,
        CancellationToken cancellationToken = default)
    {
        return _codeGenService.GenerateScaffoldAsync(projectDescription, technologies, cancellationToken);
    }

    public Task<Result<string>> GenerateMermaidDiagramAsync(
        string projectContext,
        string diagramType,
        CancellationToken cancellationToken = default)
    {
        return _diagramService.GenerateMermaidDiagramAsync(projectContext, diagramType, cancellationToken);
    }

    public Task<Result<WikiStructure[]>> GenerateWikiStructureAsync(
        string topic,
        SourceDocument[] sources,
        CancellationToken cancellationToken = default)
    {
        return _docGenService.GenerateWikiStructureAsync(topic, sources, cancellationToken);
    }

    public Task<Result<SlideContent[]>> GeneratePresentationStructureAsync(
        string topic,
        int slideCount,
        SourceDocument[] sources,
        CancellationToken cancellationToken = default)
    {
        return _docGenService.GeneratePresentationStructureAsync(topic, slideCount, sources, cancellationToken);
    }
}
