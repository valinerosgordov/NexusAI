using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.Interfaces;

public interface IProjectService
{
    Task<Result<Project>> CreateProjectAsync(
        string title,
        string? description,
        UserId ownerId,
        ProjectCategory category = ProjectCategory.Work,
        CancellationToken ct = default);
    Task<Result<Project>> GetProjectByIdAsync(ProjectId id, CancellationToken ct = default);
    Task<Result<IEnumerable<Project>>> GetUserProjectsAsync(UserId userId, CancellationToken ct = default);
    Task<Result<Project>> UpdateProjectAsync(ProjectId id, string title, string? description, CancellationToken ct = default);
    Task<Result<bool>> DeleteProjectAsync(ProjectId id, CancellationToken ct = default);
    
    Task<Result<ProjectTask>> CreateTaskAsync(
        ProjectId projectId,
        string title,
        string? role,
        decimal hours,
        TaskPriority priority = TaskPriority.Medium,
        string? assignee = null,
        Guid? sourceDocumentId = null,
        CancellationToken ct = default);
    Task<Result<ProjectTask>> GetTaskByIdAsync(ProjectTaskId id, CancellationToken ct = default);
    Task<Result<IEnumerable<ProjectTask>>> GetProjectTasksAsync(ProjectId projectId, CancellationToken ct = default);
    Task<Result<ProjectTask>> UpdateTaskStatusAsync(ProjectTaskId id, Domain.Models.TaskStatus status, CancellationToken ct = default);
    Task<Result<bool>> DeleteTaskAsync(ProjectTaskId id, CancellationToken ct = default);
}
