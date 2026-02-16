using System.Collections.Concurrent;
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Infrastructure.Services;

public sealed class InMemoryWikiService : IWikiService
{
    private readonly ConcurrentDictionary<WikiPageId, WikiPage> _pages = new();

    public Task<Result<WikiPage>> CreatePageAsync(
        string title, string content, WikiPageId? parentPageId,
        string[] tags, int order, CancellationToken ct = default)
    {
        var page = new WikiPage
        {
            Id = new WikiPageId(Guid.NewGuid()),
            Title = title,
            Content = content,
            ParentPageId = parentPageId,
            Tags = tags,
            Order = order,
            CreatedAt = DateTime.UtcNow
        };

        _pages[page.Id] = page;
        return Task.FromResult(Result<WikiPage>.Success(page));
    }

    public Task<Result<WikiPage>> GetPageByIdAsync(WikiPageId id, CancellationToken ct = default) =>
        Task.FromResult(_pages.TryGetValue(id, out var page)
            ? Result<WikiPage>.Success(page)
            : Result<WikiPage>.Failure("Wiki page not found"));

    public Task<Result<WikiPage[]>> GetAllPagesAsync(CancellationToken ct = default) =>
        Task.FromResult(Result<WikiPage[]>.Success(
            _pages.Values.OrderBy(p => p.Order).ToArray()));

    public Task<Result<WikiPage[]>> GetChildPagesAsync(WikiPageId? parentId, CancellationToken ct = default) =>
        Task.FromResult(Result<WikiPage[]>.Success(
            _pages.Values
                .Where(p => p.ParentPageId == parentId)
                .OrderBy(p => p.Order)
                .ToArray()));

    public Task<Result<WikiPageNode[]>> GetWikiTreeAsync(CancellationToken ct = default)
    {
        var allPages = _pages.Values.ToArray();
        var rootPages = allPages.Where(p => p.ParentPageId is null).OrderBy(p => p.Order);
        var tree = rootPages.Select(p => BuildNode(p, allPages)).ToArray();
        return Task.FromResult(Result<WikiPageNode[]>.Success(tree));
    }

    public Task<Result<WikiPage>> UpdatePageAsync(
        WikiPageId id, string title, string content, string[] tags, CancellationToken ct = default)
    {
        if (!_pages.TryGetValue(id, out var existing))
            return Task.FromResult(Result<WikiPage>.Failure("Wiki page not found"));

        var updated = existing with { Title = title, Content = content, Tags = tags, UpdatedAt = DateTime.UtcNow };
        _pages[id] = updated;
        return Task.FromResult(Result<WikiPage>.Success(updated));
    }

    public Task<Result<bool>> DeletePageAsync(WikiPageId id, CancellationToken ct = default) =>
        Task.FromResult(_pages.TryRemove(id, out _)
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("Wiki page not found"));

    public Task<Result<bool>> DeleteAllPagesAsync(CancellationToken ct = default)
    {
        _pages.Clear();
        return Task.FromResult(Result<bool>.Success(true));
    }

    private static WikiPageNode BuildNode(WikiPage page, WikiPage[] allPages)
    {
        var children = allPages
            .Where(p => p.ParentPageId == page.Id)
            .OrderBy(p => p.Order)
            .Select(p => BuildNode(p, allPages))
            .ToArray();

        return new WikiPageNode(page.Id, page.Title, page, children);
    }
}
