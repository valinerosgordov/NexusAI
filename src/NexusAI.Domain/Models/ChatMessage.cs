namespace NexusAI.Domain.Models;

public record ChatMessageId(Guid Value)
{
    public static ChatMessageId NewId() => new(Guid.NewGuid());
}

public static class MessageRole
{
    public const string User = "user";
    public const string Assistant = "assistant";
}

public record ChatMessage(
    ChatMessageId Id,
    string Content,
    string Role,
    DateTime Timestamp,
    string[]? SourceCitations = null,
    string[]? ThinkingSteps = null
);
