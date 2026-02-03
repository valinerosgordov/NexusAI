using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.Interfaces;

public interface IScaffoldingService
{
    Task<Result<ScaffoldResult>> CreateStructureAsync(
        string rootPath,
        ScaffoldFile[] files,
        CancellationToken cancellationToken = default);
    
    Result<bool> ValidatePath(string path);
}
