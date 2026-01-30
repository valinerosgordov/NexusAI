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
        
        services.AddSingleton<IAiService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(60);
            var apiKeyHolder = sp.GetRequiredService<ApiKeyHolder>();
            return new GeminiAiService(httpClient, apiKeyHolder);
        });

        services.AddSingleton<IPdfParsingService, PdfParsingService>();
        services.AddSingleton<IObsidianService, ObsidianService>();
        services.AddSingleton<KnowledgeHubService>();
        services.AddSingleton<MainViewModel>();

        return services;
    }
}

public sealed class ApiKeyHolder
{
    public string ApiKey { get; set; } = string.Empty;
}

