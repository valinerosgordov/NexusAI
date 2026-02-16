using NexusAI.Domain.Models;

namespace NexusAI.Application.Services;

public sealed class KnowledgeGraphService
{
    private static readonly char[] WordSeparators = [' ', '\n', '\r', '\t', '.', ',', ';', ':', '!', '?'];

    public record GraphNode(
        SourceDocumentId DocumentId,
        string Name,
        double X,
        double Y,
        string[] Keywords
    );

    public record GraphEdge(
        SourceDocumentId Source,
        SourceDocumentId Target,
        int SharedKeywords
    );

    public static (GraphNode[] Nodes, GraphEdge[] Edges) BuildGraph(SourceDocument[] documents)
    {
        if (documents.Length == 0)
            return (Array.Empty<GraphNode>(), Array.Empty<GraphEdge>());

        var nodes = documents.Select((doc, index) =>
        {
            var keywords = ExtractKeywords(doc.Content);
            var angle = 2 * Math.PI * index / documents.Length;
            var radius = 200;
            
            return new GraphNode(
                doc.Id,
                doc.Name,
                300 + radius * Math.Cos(angle),
                300 + radius * Math.Sin(angle),
                keywords
            );
        }).ToArray();

        List<GraphEdge> edges = [];
        for (int i = 0; i < nodes.Length; i++)
        {
            for (int j = i + 1; j < nodes.Length; j++)
            {
                var shared = nodes[i].Keywords.Intersect(nodes[j].Keywords, StringComparer.OrdinalIgnoreCase).Count();
                if (shared > 2)
                {
                    edges.Add(new GraphEdge(
                        nodes[i].DocumentId,
                        nodes[j].DocumentId,
                        shared
                    ));
                }
            }
        }

        return (nodes, edges.ToArray());
    }

    private static string[] ExtractKeywords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Array.Empty<string>();
        var stopwords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "the", "a", "an", "and", "or", "but", "in", "on", "at", "to", "for",
            "of", "with", "by", "from", "is", "are", "was", "were", "be", "been",
            "this", "that", "these", "those", "it", "its", "can", "will", "would"
        };

        var words = text
            .Split(WordSeparators, StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 4 && !stopwords.Contains(w))
            .GroupBy(w => w.ToLowerInvariant(), StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Select(g => g.Key)
            .ToArray();

        return words;
    }
}
