using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using PersonalNBV.Application.Interfaces;
using PersonalNBV.Application.Services;
using PersonalNBV.Infrastructure.Services;
using PersonalNBV.Presentation.ViewModels;

namespace PersonalNBV.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Infrastructure
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

        // Application
        services.AddSingleton<KnowledgeHubService>();

        // ViewModels
        services.AddSingleton<MainViewModel>();

        return services;
    }
}

public sealed class ApiKeyHolder
{
    public string ApiKey { get; set; } = string.Empty;
}

