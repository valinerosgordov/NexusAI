namespace NexusAI.Domain.Models;

public record SourceDocumentId(Guid Value)
{
    public static SourceDocumentId NewId() => new(Guid.NewGuid());
}
