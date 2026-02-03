using NexusAI.Application.Interfaces;

namespace NexusAI.Infrastructure.Parsers;

public sealed class DocumentParserFactory : IDocumentParserFactory
{
    private readonly IDocumentParserWithMetadata[] _parsers;

    public DocumentParserFactory(IEnumerable<IDocumentParser> parsers)
    {
        // Filter only parsers with metadata for routing
        _parsers = parsers.OfType<IDocumentParserWithMetadata>().ToArray();
    }

    public IDocumentParser? GetParser(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        return _parsers.FirstOrDefault(p => 
            p.SupportedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase));
    }

    public string GetFileDialogFilter()
    {
        if (_parsers.Length == 0)
        {
            return "All Files (*.*)|*.*";
        }

        // Build dynamic filter based on registered parsers
        var allExtensions = _parsers
            .SelectMany(p => p.SupportedExtensions)
            .Select(ext => $"*{ext}")
            .ToArray();

        List<string> filters =
        [
            $"All Supported Documents|{string.Join(";", allExtensions)}"
        ];

        // Add individual filters for each parser
        foreach (var parser in _parsers)
        {
            var extensions = string.Join(";", parser.SupportedExtensions.Select(ext => $"*{ext}"));
            filters.Add($"{parser.DisplayName} ({extensions})|{extensions}");
        }

        return string.Join("|", filters);
    }
}
