using PersonalNBV.Domain;
using PersonalNBV.Domain.Models;

namespace PersonalNBV.Application.Interfaces;

public interface IObsidianService
{
    Task<Result<SourceDocument[]>> LoadNotesAsync(string vaultPath, string? subfolder = null, CancellationToken cancellationToken = default);
    Task<Result<string>> SaveNoteAsync(string vaultPath, string fileName, string content, string[]? sourceLinks = null, CancellationToken cancellationToken = default);
}
