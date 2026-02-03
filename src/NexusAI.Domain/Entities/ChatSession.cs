namespace NexusAI.Domain.Entities;

public sealed class ChatSession
{
    public Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required string Title { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastActivityAt { get; set; }

    public User User { get; set; } = null!;
    public ICollection<ChatMessage> Messages { get; set; } = [];
}
