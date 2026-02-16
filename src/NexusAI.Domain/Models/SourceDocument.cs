namespace NexusAI.Domain.Models;

public record SourceDocument(
    SourceDocumentId Id,
    string Name,
    string FilePath,
    string Content,
    DateTime LoadedAt,
    string Type,
    string FileType = "",
    long SizeBytes = 0,
    bool IsIncluded = true
);
