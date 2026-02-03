using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Projects;

public record GetUserProjectsQuery(UserId UserId);

public class GetUserProjectsHandler(IProjectService projectService)
{
    public async Task<Result<IEnumerable<Project>>> HandleAsync(GetUserProjectsQuery query, CancellationToken ct = default)
    {
        return await projectService.GetUserProjectsAsync(query.UserId, ct);
    }
}
