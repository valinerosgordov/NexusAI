namespace NexusAI.Domain.Entities;

public sealed class ProjectTask
{
    public Guid Id { get; set; }
    public required Guid ProjectId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public required string Role { get; set; }
    public double EstimatedHours { get; set; }
    public int OrderIndex { get; set; }
    public int? GitHubIssueNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Project Project { get; set; } = null!;
}

public enum TaskStatus
{
    Todo,
    InProgress,
    Done
}
