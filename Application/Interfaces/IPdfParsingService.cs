using PersonalNBV.Domain;
using PersonalNBV.Domain.Models;

namespace PersonalNBV.Application.Interfaces;

public interface IPdfParsingService
{
    Task<Result<SourceDocument>> ParsePdfAsync(string filePath, CancellationToken cancellationToken = default);
}
