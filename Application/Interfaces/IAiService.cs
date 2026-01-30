using PersonalNBV.Domain;
using PersonalNBV.Domain.Models;

namespace PersonalNBV.Application.Interfaces;

public interface IAiService
{
    Task<Result<AiResponse>> AskQuestionAsync(
        string question,
        string context,
        CancellationToken cancellationToken = default);
}
