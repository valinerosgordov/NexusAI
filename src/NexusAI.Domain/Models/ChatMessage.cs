namespace NexusAI.Domain.Models;

public record ChatMessage(
    ChatMessageId Id,
    string Content,
    string Role,
    DateTime Timestamp,
    string[]? SourceCitations = null,
    string[]? ThinkingSteps = null
);
