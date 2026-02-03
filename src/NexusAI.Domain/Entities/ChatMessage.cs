namespace NexusAI.Domain.Entities;

public sealed class ChatMessage
{
    public Guid Id { get; set; }
    public required Guid ChatSessionId { get; set; }
    public required string Content { get; set; }
    public required string Role { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public ChatSession ChatSession { get; set; } = null!;
}
