namespace NexusAI.Domain.Models;

public record ChatMessageId(Guid Value)
{
    public static ChatMessageId NewId() => new(Guid.NewGuid());
}
