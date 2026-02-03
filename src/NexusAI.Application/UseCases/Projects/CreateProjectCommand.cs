using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Projects;

public record CreateProjectCommand(
    string Title,
    string? Description,
    UserId OwnerId,
    ProjectCategory Category = ProjectCategory.Work
);

public class CreateProjectHandler(IProjectService projectService)
{
    public async Task<Result<Project>> HandleAsync(CreateProjectCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Title))
            return Result<Project>.Failure("Project title is required");

        return await projectService.CreateProjectAsync(
            command.Title,
            command.Description,
            command.OwnerId,
            command.Category,
            ct);
    }
}
