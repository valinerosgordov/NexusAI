using Microsoft.EntityFrameworkCore;
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;
using NexusAI.Infrastructure.Data;

namespace NexusAI.Infrastructure.Services;

public class WikiService(AppDbContext dbContext) : IWikiService
{
    public async Task<Result<WikiPage>> CreatePageAsync(
        string title,
        string content,
        WikiPageId? parentPageId,
        string[] tags,
        int order,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<WikiPage>("Title cannot be empty");

        if (string.IsNullOrWhiteSpace(content))
            return Result.Failure<WikiPage>("Content cannot be empty");

        try
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

            dbContext.WikiPages.Add(page);
            await dbContext.SaveChangesAsync(ct).ConfigureAwait(false);

            return Result.Success(page);
        }
        catch (Exception ex)
        {
            return Result.Failure<WikiPage>($"Failed to create page: {ex.Message}");
        }
    }

    public async Task<Result<WikiPage>> GetPageByIdAsync(
        WikiPageId id,
        CancellationToken ct = default)
    {
        try
        {
            var page = await dbContext.WikiPages
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, ct)
                .ConfigureAwait(false);

            if (page is null)
                return Result.Failure<WikiPage>("Page not found");

            return Result.Success(page);
        }
        catch (Exception ex)
        {
            return Result.Failure<WikiPage>($"Failed to get page: {ex.Message}");
        }
    }

    public async Task<Result<WikiPage[]>> GetAllPagesAsync(
        CancellationToken ct = default)
    {
        try
        {
            var pages = await dbContext.WikiPages
                .AsNoTracking()
                .OrderBy(p => p.Order)
                .ToArrayAsync(ct)
                .ConfigureAwait(false);

            return Result.Success(pages);
        }
        catch (Exception ex)
        {
            return Result.Failure<WikiPage[]>($"Failed to get pages: {ex.Message}");
        }
    }

    public async Task<Result<WikiPage[]>> GetChildPagesAsync(
        WikiPageId? parentId,
        CancellationToken ct = default)
    {
        try
        {
            var pages = await dbContext.WikiPages
                .AsNoTracking()
                .Where(p => p.ParentPageId == parentId)
                .OrderBy(p => p.Order)
                .ToArrayAsync(ct)
                .ConfigureAwait(false);

            return Result.Success(pages);
        }
        catch (Exception ex)
        {
            return Result.Failure<WikiPage[]>($"Failed to get child pages: {ex.Message}");
        }
    }

    public async Task<Result<WikiPageNode[]>> GetWikiTreeAsync(
        CancellationToken ct = default)
    {
        try
        {
            var allPages = await dbContext.WikiPages
                .AsNoTracking()
                .OrderBy(p => p.Order)
                .ToArrayAsync(ct)
                .ConfigureAwait(false);

            var rootPages = allPages.Where(p => p.ParentPageId is null).ToArray();
            var tree = rootPages.Select(root => BuildTree(root, allPages)).ToArray();

            return Result.Success(tree);
        }
        catch (Exception ex)
        {
            return Result.Failure<WikiPageNode[]>($"Failed to build wiki tree: {ex.Message}");
        }
    }

    public async Task<Result<WikiPage>> UpdatePageAsync(
        WikiPageId id,
        string title,
        string content,
        string[] tags,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<WikiPage>("Title cannot be empty");

        if (string.IsNullOrWhiteSpace(content))
            return Result.Failure<WikiPage>("Content cannot be empty");

        try
        {
            var existingPage = await dbContext.WikiPages
                .FirstOrDefaultAsync(p => p.Id == id, ct)
                .ConfigureAwait(false);

            if (existingPage is null)
                return Result.Failure<WikiPage>("Page not found");

            var updatedPage = existingPage with
            {
                Title = title,
                Content = content,
                Tags = tags,
                UpdatedAt = DateTime.UtcNow
            };

            dbContext.WikiPages.Remove(existingPage);
            dbContext.WikiPages.Add(updatedPage);
            await dbContext.SaveChangesAsync(ct).ConfigureAwait(false);

            return Result.Success(updatedPage);
        }
        catch (Exception ex)
        {
            return Result.Failure<WikiPage>($"Failed to update page: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeletePageAsync(
        WikiPageId id,
        CancellationToken ct = default)
    {
        try
        {
            // Delete all children first (recursive)
            var children = await dbContext.WikiPages
                .Where(p => p.ParentPageId == id)
                .ToArrayAsync(ct)
                .ConfigureAwait(false);

            foreach (var child in children)
            {
                await DeletePageAsync(child.Id, ct).ConfigureAwait(false);
            }

            // Delete the page itself
            var page = await dbContext.WikiPages
                .FirstOrDefaultAsync(p => p.Id == id, ct)
                .ConfigureAwait(false);

            if (page is not null)
            {
                dbContext.WikiPages.Remove(page);
                await dbContext.SaveChangesAsync(ct).ConfigureAwait(false);
            }

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Failed to delete page: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteAllPagesAsync(
        CancellationToken ct = default)
    {
        try
        {
            var allPages = await dbContext.WikiPages.ToArrayAsync(ct).ConfigureAwait(false);
            dbContext.WikiPages.RemoveRange(allPages);
            await dbContext.SaveChangesAsync(ct).ConfigureAwait(false);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Failed to delete all pages: {ex.Message}");
        }
    }

    private static WikiPageNode BuildTree(WikiPage page, WikiPage[] allPages)
    {
        var children = allPages
            .Where(p => p.ParentPageId == page.Id)
            .Select(child => BuildTree(child, allPages))
            .ToArray();

        return new WikiPageNode
        {
            Page = page,
            Children = children
        };
    }
}
