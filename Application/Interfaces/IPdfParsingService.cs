using NexusAI.Domain;
using NexusAI.Domain.Models;

namespace NexusAI.Application.Interfaces;

public interface IPdfParsingService
{
    Task<Result<SourceDocument>> ParsePdfAsync(string filePath, CancellationToken cancellationToken = default);
}
