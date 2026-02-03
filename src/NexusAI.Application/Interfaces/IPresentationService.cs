using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.Interfaces;

public interface IPresentationService
{
    Task<Result<string>> CreatePresentationAsync(
        SlideDeck deck,
        string outputPath,
        CancellationToken cancellationToken = default);
    
    Result<bool> ValidateOutputPath(string path);
}
