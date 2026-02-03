using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;

namespace NexusAI.Application.UseCases.Chat;

public sealed record GenerateFollowUpQuestionsCommand(string LastQuestion, string LastAnswer);

public sealed class GenerateFollowUpQuestionsHandler
{
    private readonly IAiService _aiService;

    public GenerateFollowUpQuestionsHandler(IAiService aiService)
    {
        _aiService = aiService;
    }

    public async Task<Result<string[]>> HandleAsync(
        GenerateFollowUpQuestionsCommand command,
        CancellationToken cancellationToken = default)
    {
        var prompt = $"""
            Based on the previous question and answer, suggest 3 relevant follow-up questions that would help deepen understanding of the topic.
            
            Previous Question: {command.LastQuestion}
            Previous Answer: {command.LastAnswer}
            
            Generate 3 concise, specific follow-up questions (one per line, no numbering).
            Each question should explore a different angle or dig deeper into the topic.
            """;

        var aiResult = await _aiService.AskQuestionAsync(prompt, string.Empty, cancellationToken);

        if (aiResult.IsFailure)
        {
            return Result.Failure<string[]>(aiResult.Error);
        }

        var questions = aiResult.Value.Content
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(q => !string.IsNullOrWhiteSpace(q))
            .Take(3)
            .ToArray();

        return Result.Success(questions);
    }
}
