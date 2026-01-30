using NexusAI.Domain;
using NexusAI.Domain.Models;

namespace NexusAI.Application.Interfaces;

public interface IAiService
{
    Task<Result<AiResponse>> AskQuestionAsync(
        string question,
        string context,
        CancellationToken cancellationToken = default);
}
