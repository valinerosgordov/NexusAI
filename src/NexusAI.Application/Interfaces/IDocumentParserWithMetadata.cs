namespace NexusAI.Application.Interfaces;

/// <summary>
/// Combined interface for parsers that need both capabilities.
/// Most parsers will implement this.
/// </summary>
public interface IDocumentParserWithMetadata : IDocumentParser, IDocumentParserMetadata
{
}
