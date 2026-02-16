#pragma warning disable MA0048
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;

namespace NexusAI.Application.UseCases.Obsidian;

public sealed record ExportToObsidianCommand(
    string VaultPath,
    string Title,
    string Content,
    string[] SourceLinks
);

public sealed class ExportToObsidianHandler
{
    private readonly IObsidianService _obsidianService;

    public ExportToObsidianHandler(IObsidianService obsidianService)
    {
        _obsidianService = obsidianService;
    }

    public async Task<Result<string>> HandleAsync(
        ExportToObsidianCommand command,
        CancellationToken cancellationToken = default)
    {
        return await _obsidianService.SaveNoteAsync(
            command.VaultPath,
            command.Title,
            command.Content,
            command.SourceLinks,
            cancellationToken).ConfigureAwait(false);
    }
}
#pragma warning restore MA0048
