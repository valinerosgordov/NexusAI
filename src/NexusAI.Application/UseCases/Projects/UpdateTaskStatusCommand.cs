#pragma warning disable MA0048
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Projects;

public record UpdateTaskStatusCommand(ProjectTaskId TaskId, Domain.Models.TaskStatus Status);

public class UpdateTaskStatusHandler(IProjectService projectService)
{
    public async Task<Result<ProjectTask>> HandleAsync(UpdateTaskStatusCommand command, CancellationToken ct = default)
    {
        return await projectService.UpdateTaskStatusAsync(command.TaskId, command.Status, ct).ConfigureAwait(false);
    }
}
#pragma warning restore MA0048
