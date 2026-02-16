#pragma warning disable MA0048
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Wiki;

public record DeleteWikiPageCommand(WikiPageId Id);

public class DeleteWikiPageHandler(IWikiService wikiService)
{
    public async Task<Result<bool>> HandleAsync(
        DeleteWikiPageCommand command,
        CancellationToken ct = default)
    {
        return await wikiService.DeletePageAsync(command.Id, ct).ConfigureAwait(false);
    }
}
#pragma warning restore MA0048
