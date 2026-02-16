#pragma warning disable MA0048
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Projects;

public record GetProjectTasksQuery(ProjectId ProjectId);

public class GetProjectTasksHandler(IProjectService projectService)
{
    public async Task<Result<IEnumerable<ProjectTask>>> HandleAsync(
        GetProjectTasksQuery query,
        CancellationToken ct = default)
    {
        return await projectService.GetProjectTasksAsync(query.ProjectId, ct).ConfigureAwait(false);
    }
}
#pragma warning restore MA0048
