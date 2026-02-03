using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.Interfaces;

public interface IObsidianService
{
    Task<Result<SourceDocument[]>> LoadNotesAsync(string vaultPath, string? subfolder = null, CancellationToken cancellationToken = default);
    Task<Result<string>> SaveNoteAsync(string vaultPath, string title, string content, string[] sourceLinks, CancellationToken cancellationToken = default);
}
