namespace NexusAI.Domain.Models;

public record AiResponse(
    string Content,
    string[] SourcesCited,
    int TokensUsed
);
