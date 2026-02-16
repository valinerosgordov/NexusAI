namespace NexusAI.Application.Interfaces;

/// <summary>
/// Metadata provider for document parsers.
/// Separated from IDocumentParser following Interface Segregation Principle.
/// Used by factory to route files to correct parser.
/// </summary>
public interface IDocumentParserMetadata
{
    string[] SupportedExtensions { get; }
    string DisplayName { get; }
}
