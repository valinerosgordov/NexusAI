#pragma warning disable MA0048
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Wiki;

public record GenerateWikiCommand(string Topic, SourceDocument[] Sources);

public class GenerateWikiHandler(
    IAiService aiService,
    IWikiService wikiService)
{
    public async Task<Result<WikiPageNode[]>> HandleAsync(
        GenerateWikiCommand command,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Topic))
            return Result.Failure<WikiPageNode[]>("Topic is required");

        if (command.Sources is not { Length: > 0 })
            return Result.Failure<WikiPageNode[]>("At least one source document is required");

        // Clear existing wiki
        await wikiService.DeleteAllPagesAsync(ct).ConfigureAwait(false);

        // Generate structure using AI
        var structureResult = await aiService.GenerateWikiStructureAsync(
            command.Topic,
            command.Sources,
            ct).ConfigureAwait(false);

        if (!structureResult.IsSuccess)
            return Result.Failure<WikiPageNode[]>($"AI generation failed: {structureResult.Error}");

        // Save to database
        var orderCounter = new OrderCounter();
        foreach (var structure in structureResult.Value)
        {
            await SaveWikiStructureAsync(structure, null, orderCounter, ct).ConfigureAwait(false);
        }

        // Build and return tree
        var treeResult = await wikiService.GetWikiTreeAsync(ct).ConfigureAwait(false);
        return treeResult;
    }

    private async Task SaveWikiStructureAsync(
        WikiStructure structure,
        WikiPageId? parentId,
        OrderCounter orderCounter,
        CancellationToken ct)
    {
        var currentOrder = orderCounter.GetNext();

        var pageResult = await wikiService.CreatePageAsync(
            structure.Title,
            structure.Content,
            parentId,
            [],
            currentOrder,
            ct).ConfigureAwait(false);

        if (!pageResult.IsSuccess)
            return;

        var currentPageId = pageResult.Value.Id;

        foreach (var subPage in structure.SubPages)
        {
            await SaveWikiStructureAsync(subPage, currentPageId, orderCounter, ct).ConfigureAwait(false);
        }
    }

    private sealed class OrderCounter
    {
        private int _order;

        public int GetNext() => _order++;
    }
}
#pragma warning restore MA0048
