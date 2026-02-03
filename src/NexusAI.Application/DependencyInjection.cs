using Microsoft.Extensions.DependencyInjection;
using NexusAI.Application.Services;
using NexusAI.Application.UseCases.Artifacts;
using NexusAI.Application.UseCases.Auth;
using NexusAI.Application.UseCases.Chat;
using NexusAI.Application.UseCases.Diagrams;
using NexusAI.Application.UseCases.Documents;
using NexusAI.Application.UseCases.Obsidian;
using NexusAI.Application.UseCases.Projects;
using NexusAI.Application.UseCases.Scaffold;
using NexusAI.Application.UseCases.Wiki;
using NexusAI.Application.UseCases.Presentations;

namespace NexusAI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Document & Chat Use Case Handlers
        services.AddTransient<AddDocumentHandler>();
        services.AddTransient<AskQuestionHandler>();
        services.AddTransient<GenerateArtifactHandler>();
        services.AddTransient<LoadObsidianVaultHandler>();
        services.AddTransient<ExportToObsidianHandler>();
        services.AddTransient<GenerateFollowUpQuestionsHandler>();

        // Auth Use Case Handlers
        services.AddTransient<RegisterUserHandler>();
        services.AddTransient<LoginUserHandler>();

        // Project Use Case Handlers
        services.AddTransient<CreateProjectHandler>();
        services.AddTransient<CreateTaskHandler>();
        services.AddTransient<GetUserProjectsHandler>();
        services.AddTransient<GetProjectTasksHandler>();
        services.AddTransient<UpdateTaskStatusHandler>();
        services.AddTransient<GenerateProjectPlanHandler>();

        // Scaffold Use Case Handlers
        services.AddTransient<GenerateScaffoldHandler>();

        // Diagram Use Case Handlers
        services.AddTransient<GenerateDiagramHandler>();

        // Wiki Use Case Handlers
        services.AddTransient<GenerateWikiHandler>();
        services.AddTransient<GetWikiTreeHandler>();
        services.AddTransient<UpdateWikiPageHandler>();
        services.AddTransient<DeleteWikiPageHandler>();

        // Presentation Use Case Handlers
        services.AddTransient<GeneratePresentationHandler>();

        // Application Services
        services.AddSingleton<SessionContext>();
        services.AddSingleton<KnowledgeGraphService>();

        return services;
    }
}
