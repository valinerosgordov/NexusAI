namespace NexusAI.Domain.Entities;

public sealed class ProjectFile
{
    public Guid Id { get; set; }
    public required Guid ProjectId { get; set; }
    public required string FilePath { get; set; }
    public required string Content { get; set; }
    public required string Language { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Project Project { get; set; } = null!;
}
