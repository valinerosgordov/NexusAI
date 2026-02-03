namespace NexusAI.Domain.Entities;

public sealed class Project
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? GitHubRepoUrl { get; set; }
    public required Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<ProjectTask> Tasks { get; set; } = [];
    public ICollection<ProjectFile> Files { get; set; } = [];
}
