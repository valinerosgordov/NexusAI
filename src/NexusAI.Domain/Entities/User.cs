namespace NexusAI.Domain.Entities;

public sealed class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Project> Projects { get; set; } = [];
}
