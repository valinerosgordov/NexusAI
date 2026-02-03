using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.Interfaces;

public interface IWikiService
{
    Task<Result<WikiPage>> CreatePageAsync(
        string title,
        string content,
        WikiPageId? parentPageId,
        string[] tags,
        int order,
        CancellationToken ct = default);
    
    Task<Result<WikiPage>> GetPageByIdAsync(
        WikiPageId id,
        CancellationToken ct = default);
    
    Task<Result<WikiPage[]>> GetAllPagesAsync(
        CancellationToken ct = default);
    
    Task<Result<WikiPage[]>> GetChildPagesAsync(
        WikiPageId? parentId,
        CancellationToken ct = default);
    
    Task<Result<WikiPageNode[]>> GetWikiTreeAsync(
        CancellationToken ct = default);
    
    Task<Result<WikiPage>> UpdatePageAsync(
        WikiPageId id,
        string title,
        string content,
        string[] tags,
        CancellationToken ct = default);
    
    Task<Result<bool>> DeletePageAsync(
        WikiPageId id,
        CancellationToken ct = default);
    
    Task<Result<bool>> DeleteAllPagesAsync(
        CancellationToken ct = default);
}
