using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Obsidian;

public sealed record LoadObsidianVaultCommand(string VaultPath, string? Subfolder = null);

public sealed class LoadObsidianVaultHandler
{
    private readonly IObsidianService _obsidianService;

    public LoadObsidianVaultHandler(IObsidianService obsidianService)
    {
        _obsidianService = obsidianService;
    }

    public async Task<Result<SourceDocument[]>> HandleAsync(
        LoadObsidianVaultCommand command,
        CancellationToken cancellationToken = default)
    {
        return await _obsidianService.LoadNotesAsync(
            command.VaultPath, 
            command.Subfolder, 
            cancellationToken);
    }
}
