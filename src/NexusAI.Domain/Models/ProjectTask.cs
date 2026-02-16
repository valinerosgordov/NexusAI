namespace NexusAI.Domain.Models;

public record ProjectTask
{
    public required ProjectTaskId Id { get; init; }
    public required ProjectId ProjectId { get; init; }
    public required string Title { get; init; }
    public string? Role { get; init; }
    public TaskStatus Status { get; init; } = TaskStatus.Pending;
    public TaskPriority Priority { get; init; } = TaskPriority.Medium;
    public decimal Hours { get; init; }
    public string? Assignee { get; init; }
    public Guid? SourceDocumentId { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; init; }
}
