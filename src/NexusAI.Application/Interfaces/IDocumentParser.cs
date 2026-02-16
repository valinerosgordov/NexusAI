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
