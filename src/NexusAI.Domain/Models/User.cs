namespace NexusAI.Domain.Models;

public record UserId(Guid Value);

public record User
{
    public required UserId Id { get; init; }
    public required string Name { get; init; }
    public required string PasswordHash { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
