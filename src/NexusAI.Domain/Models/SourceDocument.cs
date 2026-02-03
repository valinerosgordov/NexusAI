namespace NexusAI.Domain.Models;

public record SourceDocumentId(Guid Value)
{
    public static SourceDocumentId NewId() => new(Guid.NewGuid());
}

public static class SourceType
{
    public const string PDF = "PDF";
    public const string DOCX = "DOCX";
    public const string PPTX = "PPTX";
    public const string EPUB = "EPUB";
    public const string TXT = "TXT";
    public const string MD = "MD";
    public const string Obsidian = "Obsidian";
    public const string Document = "Document";
    public const string ObsidianNote = "ObsidianNote";
}

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
