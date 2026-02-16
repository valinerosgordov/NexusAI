using Microsoft.Extensions.DependencyInjection;
using NexusAI.Application.Interfaces;
using NexusAI.Infrastructure.Parsers;
using NexusAI.Infrastructure.Services;
using System.Runtime.Versioning;

namespace NexusAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        Func<string> geminiApiKeyProvider)
    {
        services.AddHttpClient();

        services.AddSingleton<IScaffoldingService, ScaffoldingService>();

        // AI Services
        services.AddSingleton<GeminiAiService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(60);
            var sessionContext = sp.GetRequiredService<NexusAI.Application.Services.SessionContext>();
            return new GeminiAiService(httpClient, geminiApiKeyProvider, sessionContext);
        });

        services.AddSingleton<OllamaService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            return new OllamaService(httpClient);
        });

        // AI Service Factory (Strategy Pattern for Provider Switching)
        services.AddSingleton<IAiServiceFactory, AiServiceFactory>();

        // Default AI Service (Gemini) for Use Cases that don't need switching
        services.AddSingleton<IAiService>(sp => sp.GetRequiredService<GeminiAiService>());

        // Document Parsers
        services.AddSingleton<IDocumentParser, PdfParser>();
        services.AddSingleton<IDocumentParser, WordParser>();
        services.AddSingleton<IDocumentParser, PresentationParser>();
        services.AddSingleton<IDocumentParser, EpubParser>();
        services.AddSingleton<IDocumentParser, TextParser>();
        services.AddSingleton<IDocumentParserFactory, DocumentParserFactory>();

        // Other Services
        services.AddSingleton<IObsidianService, ObsidianService>();
        if (OperatingSystem.IsWindows())
        {
            services.AddScoped<IAudioService, SpeechSynthesisService>();
        }
        services.AddSingleton<IPresentationService, PresentationService>();
        services.AddSingleton<IProjectService, InMemoryProjectService>();
        services.AddSingleton<IWikiService, InMemoryWikiService>();
        services.AddSingleton<IAuthService, InMemoryAuthService>();

        return services;
    }
}
