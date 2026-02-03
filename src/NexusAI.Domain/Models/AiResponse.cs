namespace NexusAI.Domain.Models;

public record AiResponse(
    string Content,
    int TokensUsed,
    bool WasTruncated = false,
    string[]? SourcesCited = null
);
