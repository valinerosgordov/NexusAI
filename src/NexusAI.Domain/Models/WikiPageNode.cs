namespace NexusAI.Domain.Models;

public record WikiPageNode(
    WikiPageId Id,
    string Title,
    WikiPage Page,
    WikiPageNode[] Children
);
