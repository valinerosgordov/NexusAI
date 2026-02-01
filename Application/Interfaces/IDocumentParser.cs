using NexusAI.Domain;
using NexusAI.Domain.Models;

namespace NexusAI.Application.Interfaces;

public interface IDocumentParser
{
    bool CanParse(string extension);
    Task<Result<SourceDocument>> ParseAsync(string filePath, CancellationToken cancellationToken = default);
}
