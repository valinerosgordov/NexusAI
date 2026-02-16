using System.Collections.Concurrent;
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Infrastructure.Services;

public sealed class InMemoryProjectService : IProjectService
{
    private readonly ConcurrentDictionary<ProjectId, Project> _projects = new();
    private readonly ConcurrentDictionary<ProjectTaskId, ProjectTask> _tasks = new();

    public Task<Result<Project>> CreateProjectAsync(
        string title,
        string? description,
        UserId ownerId,
        ProjectCategory category = ProjectCategory.Work,
        CancellationToken ct = default)
    {
        var project = new Project
        {
            Id = new ProjectId(Guid.NewGuid()),
            Title = title,
            Description = description,
            OwnerId = ownerId,
            Category = category,
            CreatedAt = DateTime.UtcNow
        };

        _projects[project.Id] = project;
        return Task.FromResult(Result<Project>.Success(project));
    }

    public Task<Result<Project>> GetProjectByIdAsync(ProjectId id, CancellationToken ct = default) =>
        Task.FromResult(_projects.TryGetValue(id, out var project)
            ? Result<Project>.Success(project)
            : Result<Project>.Failure("Project not found"));

    public Task<Result<IEnumerable<Project>>> GetUserProjectsAsync(UserId userId, CancellationToken ct = default)
    {
        var userProjects = _projects.Values
            .Where(p => p.OwnerId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .AsEnumerable();

        return Task.FromResult(Result<IEnumerable<Project>>.Success(userProjects));
    }

    public Task<Result<Project>> UpdateProjectAsync(
        ProjectId id, string title, string? description, CancellationToken ct = default)
    {
        if (!_projects.TryGetValue(id, out var existing))
            return Task.FromResult(Result<Project>.Failure("Project not found"));

        var updated = existing with { Title = title, Description = description, UpdatedAt = DateTime.UtcNow };
        _projects[id] = updated;
        return Task.FromResult(Result<Project>.Success(updated));
    }

    public Task<Result<bool>> DeleteProjectAsync(ProjectId id, CancellationToken ct = default) =>
        Task.FromResult(_projects.TryRemove(id, out _)
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("Project not found"));

    public Task<Result<ProjectTask>> CreateTaskAsync(
        ProjectId projectId,
        string title,
        string? role,
        decimal hours,
        TaskPriority priority = TaskPriority.Medium,
        string? assignee = null,
        Guid? sourceDocumentId = null,
        CancellationToken ct = default)
    {
        var task = new ProjectTask
        {
            Id = new ProjectTaskId(Guid.NewGuid()),
            ProjectId = projectId,
            Title = title,
            Role = role,
            Hours = hours,
            Priority = priority,
            Assignee = assignee,
            SourceDocumentId = sourceDocumentId,
            CreatedAt = DateTime.UtcNow
        };

        _tasks[task.Id] = task;
        return Task.FromResult(Result<ProjectTask>.Success(task));
    }

    public Task<Result<ProjectTask>> GetTaskByIdAsync(ProjectTaskId id, CancellationToken ct = default) =>
        Task.FromResult(_tasks.TryGetValue(id, out var task)
            ? Result<ProjectTask>.Success(task)
            : Result<ProjectTask>.Failure("Task not found"));

    public Task<Result<IEnumerable<ProjectTask>>> GetProjectTasksAsync(ProjectId projectId, CancellationToken ct = default)
    {
        var projectTasks = _tasks.Values
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Priority)
            .AsEnumerable();

        return Task.FromResult(Result<IEnumerable<ProjectTask>>.Success(projectTasks));
    }

    public Task<Result<ProjectTask>> UpdateTaskStatusAsync(
        ProjectTaskId id, Domain.Models.TaskStatus status, CancellationToken ct = default)
    {
        if (!_tasks.TryGetValue(id, out var existing))
            return Task.FromResult(Result<ProjectTask>.Failure("Task not found"));

        var updated = existing with
        {
            Status = status,
            CompletedAt = status == Domain.Models.TaskStatus.Completed ? DateTime.UtcNow : existing.CompletedAt
        };

        _tasks[id] = updated;
        return Task.FromResult(Result<ProjectTask>.Success(updated));
    }

    public Task<Result<bool>> DeleteTaskAsync(ProjectTaskId id, CancellationToken ct = default) =>
        Task.FromResult(_tasks.TryRemove(id, out _)
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("Task not found"));
}
