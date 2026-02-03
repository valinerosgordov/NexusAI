using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.Interfaces;

/// <summary>
/// Core document parsing capability.
/// Follows ISP - parser only needs to know how to parse, not metadata.
/// </summary>
public interface IDocumentParser
{
    Task<Result<SourceDocument>> ParseAsync(string filePath, CancellationToken cancellationToken = default);
}

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

/// <summary>
/// Combined interface for parsers that need both capabilities.
/// Most parsers will implement this.
/// </summary>
public interface IDocumentParserWithMetadata : IDocumentParser, IDocumentParserMetadata
{
}
