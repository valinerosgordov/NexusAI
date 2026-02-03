using Microsoft.EntityFrameworkCore;
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Entities;
using NexusAI.Infrastructure.Persistence;

namespace NexusAI.Infrastructure.Services;

public sealed class ProjectService(AppDbContext context) : IProjectService
{
    public async Task<Result<Project>> CreateProjectAsync(
        string title,
        string description,
        Guid userId,
        string? gitHubRepoUrl = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<Project>.Failure("Project title cannot be empty");

        if (string.IsNullOrWhiteSpace(description))
            return Result<Project>.Failure("Project description cannot be empty");

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            GitHubRepoUrl = gitHubRepoUrl,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        context.Projects.Add(project);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);

        return Result<Project>.Success(project);
    }

    public async Task<Result<Project>> GetProjectByIdAsync(Guid projectId, CancellationToken ct = default)
    {
        var project = await context.Projects
            .Include(p => p.Tasks)
            .Include(p => p.Files)
            .FirstOrDefaultAsync(p => p.Id == projectId, ct)
            .ConfigureAwait(false);

        return project is not null
            ? Result<Project>.Success(project)
            : Result<Project>.Failure("Project not found");
    }

    public async Task<Result<IReadOnlyList<Project>>> GetUserProjectsAsync(Guid userId, CancellationToken ct = default)
    {
        var projects = await context.Projects
            .Where(p => p.UserId == userId)
            .Include(p => p.Tasks)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return Result<IReadOnlyList<Project>>.Success(projects);
    }

    public async Task<Result<ProjectTask>> CreateTaskAsync(
        Guid projectId,
        string title,
        string description,
        string role,
        double estimatedHours,
        int orderIndex,
        int? gitHubIssueNumber = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<ProjectTask>.Failure("Task title cannot be empty");

        if (string.IsNullOrWhiteSpace(description))
            return Result<ProjectTask>.Failure("Task description cannot be empty");

        var task = new ProjectTask
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Title = title,
            Description = description,
            Role = role,
            EstimatedHours = estimatedHours,
            OrderIndex = orderIndex,
            GitHubIssueNumber = gitHubIssueNumber,
            Status = TaskStatus.Todo,
            CreatedAt = DateTime.UtcNow
        };

        context.Tasks.Add(task);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);

        return Result<ProjectTask>.Success(task);
    }

    public async Task<Result<ProjectTask>> UpdateTaskStatusAsync(
        Guid taskId,
        TaskStatus newStatus,
        CancellationToken ct = default)
    {
        var task = await context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId, ct)
            .ConfigureAwait(false);

        if (task is null)
            return Result<ProjectTask>.Failure("Task not found");

        task.Status = newStatus;
        await context.SaveChangesAsync(ct).ConfigureAwait(false);

        return Result<ProjectTask>.Success(task);
    }

    public async Task<Result<IReadOnlyList<ProjectTask>>> GetProjectTasksAsync(
        Guid projectId,
        CancellationToken ct = default)
    {
        var tasks = await context.Tasks
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.OrderIndex)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return Result<IReadOnlyList<ProjectTask>>.Success(tasks);
    }
}
