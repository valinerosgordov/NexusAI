namespace PersonalNBV.Domain.Models;

public record ChatMessage(
    ChatMessageId Id,
    string Content,
    MessageRole Role,
    DateTime Timestamp,
    string[]? SourceCitations = null
);

public readonly record struct ChatMessageId(Guid Value)
{
    public static ChatMessageId NewId() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public enum MessageRole
{
    User,
    Assistant,
    System
}
