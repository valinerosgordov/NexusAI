using Microsoft.Extensions.DependencyInjection;
using NexusAI.Application.Services;
using NexusAI.Application.UseCases.Artifacts;
using NexusAI.Application.UseCases.Chat;
using NexusAI.Application.UseCases.Documents;
using NexusAI.Application.UseCases.Obsidian;

namespace NexusAI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Use Case Handlers
        services.AddTransient<AddDocumentHandler>();
        services.AddTransient<AskQuestionHandler>();
        services.AddTransient<GenerateArtifactHandler>();
        services.AddTransient<LoadObsidianVaultHandler>();
        services.AddTransient<ExportToObsidianHandler>();
        services.AddTransient<GenerateFollowUpQuestionsHandler>();

        // Application Services
        services.AddSingleton<KnowledgeGraphService>();

        return services;
    }
}
