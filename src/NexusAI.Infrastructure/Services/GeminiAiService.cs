using NexusAI.Application.Interfaces;
using NexusAI.Application.Services;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;
using NexusAI.Infrastructure.Services.Gemini;

namespace NexusAI.Infrastructure.Services;

public sealed class GeminiAiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly SessionContext _sessionContext;
    private readonly Func<string> _apiKeyProvider;

    public GeminiAiService(HttpClient httpClient, Func<string> apiKeyProvider, SessionContext sessionContext)
    {
        _httpClient = httpClient;
        _apiKeyProvider = apiKeyProvider;
        _sessionContext = sessionContext;
    }

    private GeminiHttpClient CreateClient()
    {
        const string modelName = "gemini-2.0-flash";
        var apiKey = _apiKeyProvider() ?? string.Empty;
        return new GeminiHttpClient(_httpClient, apiKey, modelName);
    }

    public Task<Result<AiResponse>> AskQuestionAsync(
        string question,
        string context,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var chatService = new GeminiChatService(client, _sessionContext);
        return chatService.AskQuestionAsync(question, context, cancellationToken);
    }

    public Task<Result<AiResponse>> AskQuestionWithImagesAsync(
        string question,
        string context,
        string[] base64Images,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var chatService = new GeminiChatService(client, _sessionContext);
        return chatService.AskQuestionWithImagesAsync(question, context, base64Images, cancellationToken);
    }

    public Task<Result<ProjectPlanTask[]>> GeneratePlanAsync(
        string idea,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var codeGenService = new GeminiCodeGenService(client);
        return codeGenService.GeneratePlanAsync(idea, cancellationToken);
    }

    public Task<Result<ScaffoldFile[]>> GenerateScaffoldAsync(
        string projectDescription,
        string[] technologies,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var codeGenService = new GeminiCodeGenService(client);
        return codeGenService.GenerateScaffoldAsync(projectDescription, technologies, cancellationToken);
    }

    public Task<Result<string>> GenerateMermaidDiagramAsync(
        string projectContext,
        string diagramType,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var diagramService = new GeminiDiagramService(client);
        return diagramService.GenerateMermaidDiagramAsync(projectContext, diagramType, cancellationToken);
    }

    public Task<Result<WikiStructure[]>> GenerateWikiStructureAsync(
        string topic,
        SourceDocument[] sources,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var docGenService = new GeminiDocGenService(client);
        return docGenService.GenerateWikiStructureAsync(topic, sources, cancellationToken);
    }

    public Task<Result<SlideContent[]>> GeneratePresentationStructureAsync(
        string topic,
        int slideCount,
        SourceDocument[] sources,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var docGenService = new GeminiDocGenService(client);
        return docGenService.GeneratePresentationStructureAsync(topic, slideCount, sources, cancellationToken);
    }
}
