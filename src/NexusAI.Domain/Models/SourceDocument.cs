namespace NexusAI.Domain.Models;

public record SourceDocument(
    SourceDocumentId Id,
    string Name,
    string Content,
    SourceType Type,
    string FilePath,
    DateTime LoadedAt,
    bool IsIncluded = true
);

public readonly record struct SourceDocumentId(Guid Value)
{
    public static SourceDocumentId NewId() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public enum SourceType
{
    Document,
    ObsidianNote
}
