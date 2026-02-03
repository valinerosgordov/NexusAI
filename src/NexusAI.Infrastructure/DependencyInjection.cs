using Microsoft.Extensions.DependencyInjection;
using NexusAI.Application.Interfaces;
using NexusAI.Infrastructure.Parsers;
using NexusAI.Infrastructure.Services;

namespace NexusAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        Func<IServiceProvider, string> geminiApiKeyFactory)
    {
        services.AddHttpClient();

        // AI Services
        services.AddSingleton<GeminiAiService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(60);
            var apiKey = geminiApiKeyFactory(sp);
            return new GeminiAiService(httpClient, apiKey);
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
        services.AddScoped<IAudioService, SpeechSynthesisService>();

        return services;
    }
}
