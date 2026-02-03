namespace NexusAI.Domain.Models;

public record ProjectId(Guid Value);

public enum ProjectCategory
{
    Work,
    Education,
    Personal
}

public record Project
{
    public required ProjectId Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required UserId OwnerId { get; init; }
    public ProjectCategory Category { get; init; } = ProjectCategory.Work;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; init; }
}
