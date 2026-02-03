using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Wiki;

public record UpdateWikiPageCommand(
    WikiPageId Id,
    string Title,
    string Content,
    string[] Tags);

public class UpdateWikiPageHandler(IWikiService wikiService)
{
    public async Task<Result<WikiPage>> HandleAsync(
        UpdateWikiPageCommand command,
        CancellationToken ct = default)
    {
        return await wikiService.UpdatePageAsync(
            command.Id,
            command.Title,
            command.Content,
            command.Tags,
            ct).ConfigureAwait(false);
    }
}
