namespace NexusAI.Domain.Models;

public record WikiPageId(Guid Value);

public record WikiPage
{
    public required WikiPageId Id { get; init; }
    public WikiPageId? ParentPageId { get; init; }
    public required string Title { get; init; }
    public required string Content { get; init; }
    public string[] Tags { get; init; } = [];
    public int Order { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; init; }
}

public record WikiPageNode(
    WikiPageId Id,
    string Title,
    WikiPage Page,
    WikiPageNode[] Children
);
