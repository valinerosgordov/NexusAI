using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Wiki;

public record GetWikiTreeQuery;

public class GetWikiTreeHandler(IWikiService wikiService)
{
    public async Task<Result<WikiPageNode[]>> HandleAsync(
        GetWikiTreeQuery query,
        CancellationToken ct = default)
    {
        return await wikiService.GetWikiTreeAsync(ct).ConfigureAwait(false);
    }
}
