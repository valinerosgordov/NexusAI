using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Chat;

/// <summary>
/// Railway Oriented Programming extensions for AskQuestion use case.
/// </summary>
public static class AskQuestionCommandExtensions
{
    /// <summary>
    /// Validates question before execution.
    /// </summary>
    public static Result<AskQuestionCommand> Validate(this AskQuestionCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Question))
            return Result.Failure<AskQuestionCommand>("Question cannot be empty");

        if (command.Question.Length > 10000)
            return Result.Failure<AskQuestionCommand>("Question is too long (max 10,000 characters)");

        if (command.IncludedSources.Length == 0)
            return Result.Failure<AskQuestionCommand>("No sources selected");

        return Result.Success(command);
    }

    /// <summary>
    /// Fluent execution with validation and logging.
    /// Example of Railway pattern in action.
    /// </summary>
    public static async Task<Result<ChatMessage>> ExecuteValidated(
        this AskQuestionHandler handler,
        AskQuestionCommand command,
        Action<string> logger,
        CancellationToken cancellationToken = default)
    {
        var validationResult = command.Validate();

        if (validationResult.IsFailure)
        {
            logger($"❌ Validation failed: {validationResult.Error}");
            return Result.Failure<ChatMessage>(validationResult.Error);
        }

        logger($"🤔 Processing question: {command.Question.Substring(0, Math.Min(50, command.Question.Length))}...");

        var result = await handler.HandleAsync(command, cancellationToken);

        return result
            .Map(tuple => tuple.Message)
            .OnSuccess(msg => logger($"✅ Response generated ({msg.Content.Length} chars)"))
            .OnFailure(error => logger($"❌ AI request failed: {error}"));
    }

    /// <summary>
    /// Extracts just the message from handler result.
    /// </summary>
    public static Result<ChatMessage> ExtractMessage(
        this Result<(ChatMessage Message, bool WasTruncated, int TokenCount)> result)
    {
        return result.Map(tuple => tuple.Message);
    }

    /// <summary>
    /// Ensures response is not truncated (quality check).
    /// </summary>
    public static Result<(ChatMessage Message, bool WasTruncated, int TokenCount)> EnsureNotTruncated(
        this Result<(ChatMessage Message, bool WasTruncated, int TokenCount)> result)
    {
        return result.Ensure(
            tuple => !tuple.WasTruncated,
            "Context was truncated - some sources were excluded from analysis"
        );
    }
}
