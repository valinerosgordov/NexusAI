using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using NexusAI.Application.Interfaces;
using NexusAI.Application.Services;
using NexusAI.Infrastructure.Services;
using NexusAI.Presentation.ViewModels;

namespace NexusAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<ApiKeyHolder>();
        
        // Register both AI services
        services.AddSingleton<GeminiAiService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(60);
            var apiKeyHolder = sp.GetRequiredService<ApiKeyHolder>();
            return new GeminiAiService(httpClient, apiKeyHolder);
        });

        services.AddSingleton<OllamaService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            return new OllamaService(httpClient);
        });

        // Default to Gemini
        services.AddSingleton<IAiService>(sp => sp.GetRequiredService<GeminiAiService>());

        // Register all document parsers
        services.AddSingleton<IDocumentParser, PdfParser>();
        services.AddSingleton<IDocumentParser, WordParser>();
        services.AddSingleton<IDocumentParser, PresentationParser>();
        services.AddSingleton<IDocumentParser, EpubParser>();
        services.AddSingleton<IDocumentParser, TextParser>();
        services.AddSingleton<DocumentParserFactory>();

        services.AddSingleton<IObsidianService, ObsidianService>();
        services.AddSingleton<IAudioService, SpeechSynthesisService>();
        services.AddSingleton<KnowledgeGraphService>();
        services.AddSingleton<KnowledgeHubService>();
        services.AddSingleton<MainViewModel>();

        return services;
    }
}

public sealed class ApiKeyHolder
{
    public string ApiKey { get; set; } = string.Empty;
}

