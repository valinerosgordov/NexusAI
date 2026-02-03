using NexusAI.Application.Interfaces;
using NexusAI.Application.Services;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Projects;

public record GenerateProjectPlanCommand(
    string Idea,
    string Title,
    UserId OwnerId,
    Guid? SourceDocumentId = null
);

public class GenerateProjectPlanHandler(
    IAiService aiService,
    IProjectService projectService,
    SessionContext sessionContext)
{
    public async Task<Result<(Project Project, ProjectTask[] Tasks)>> HandleAsync(
        GenerateProjectPlanCommand command,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Idea))
            return Result.Failure<(Project, ProjectTask[])>("Project idea is required");

        if (string.IsNullOrWhiteSpace(command.Title))
            return Result.Failure<(Project, ProjectTask[])>("Project title is required");

        // Generate plan using AI
        var planResult = await aiService.GeneratePlanAsync(command.Idea, ct);
        if (!planResult.IsSuccess)
            return Result.Failure<(Project, ProjectTask[])>($"AI generation failed: {planResult.Error}");

        // Determine category based on current mode
        var category = sessionContext.CurrentMode == AppMode.Student
            ? ProjectCategory.Education
            : ProjectCategory.Work;

        // Create project
        var projectResult = await projectService.CreateProjectAsync(
            command.Title,
            command.Idea,
            command.OwnerId,
            category,
            ct);

        if (!projectResult.IsSuccess)
            return Result.Failure<(Project, ProjectTask[])>($"Project creation failed: {projectResult.Error}");

        var project = projectResult.Value;

        // Create tasks from AI-generated plan, linking to source document
        List<ProjectTask> tasks = [];
        foreach (var taskPlan in planResult.Value)
        {
            // Assign priority based on role (example logic)
            var priority = taskPlan.Role?.ToLower() switch
            {
                "frontend" or "backend" => TaskPriority.High,
                "design" or "testing" => TaskPriority.Medium,
                _ => TaskPriority.Low
            };

            var taskResult = await projectService.CreateTaskAsync(
                project.Id,
                taskPlan.Title,
                taskPlan.Role,
                taskPlan.Hours,
                priority,
                null, // Assignee (can be set later by user)
                command.SourceDocumentId, // Link to source document
                ct);

            if (taskResult.IsSuccess)
                tasks.Add(taskResult.Value);
        }

        return Result.Success((project, tasks.ToArray()));
    }
}
