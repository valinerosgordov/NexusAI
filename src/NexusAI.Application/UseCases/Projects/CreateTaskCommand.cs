using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Projects;

public record CreateTaskCommand(
    ProjectId ProjectId,
    string Title,
    string? Role,
    decimal Hours,
    TaskPriority Priority = TaskPriority.Medium,
    string? Assignee = null,
    Guid? SourceDocumentId = null
);

public class CreateTaskHandler(IProjectService projectService)
{
    public async Task<Result<ProjectTask>> HandleAsync(CreateTaskCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Title))
            return Result<ProjectTask>.Failure("Task title is required");

        if (command.Hours < 0)
            return Result<ProjectTask>.Failure("Hours cannot be negative");

        return await projectService.CreateTaskAsync(
            command.ProjectId,
            command.Title,
            command.Role,
            command.Hours,
            command.Priority,
            command.Assignee,
            command.SourceDocumentId,
            ct);
    }
}
