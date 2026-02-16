using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Documents;

/// <summary>
/// Example of Railway Oriented Programming in action.
/// Demonstrates fluent Result<T> usage.
/// </summary>
public static class AddDocumentCommandExtensions
{
    /// <summary>
    /// Fluent execution of AddDocument with automatic error handling.
    /// </summary>
    public static async Task<Result<SourceDocument>> ExecuteWithLogging(
        this AddDocumentHandler handler,
        AddDocumentCommand command,
        Action<string> logger,
        CancellationToken cancellationToken = default)
    {
        logger($"Starting document load: {command.FilePath}");

        var result = await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

        return result
            .OnSuccess(doc => logger($"✅ Successfully loaded: {doc.Name} ({doc.Content.Length} chars)"))
            .OnFailure(error => logger($"❌ Failed to load document: {error}"));
    }

    /// <summary>
    /// Validates document size after loading.
    /// </summary>
    public static Result<SourceDocument> EnsureNotEmpty(this Result<SourceDocument> result)
    {
        return result.Ensure(
            doc => !string.IsNullOrWhiteSpace(doc.Content),
            "Document content is empty"
        );
    }

    /// <summary>
    /// Validates document size is within limits.
    /// </summary>
    public static Result<SourceDocument> EnsureMaxSize(
        this Result<SourceDocument> result,
        int maxSizeInChars)
    {
        return result.Ensure(
            doc => doc.Content.Length <= maxSizeInChars,
            $"Document exceeds maximum size of {maxSizeInChars} characters"
        );
    }

    /// <summary>
    /// Complete pipeline example: Load → Validate → Transform.
    /// </summary>
    public static async Task<Result<SourceDocument>> ExecutePipeline(
        this AddDocumentHandler handler,
        string filePath,
        Action<string> logger,
        CancellationToken cancellationToken = default)
    {
        var command = new AddDocumentCommand(filePath);

        var result = await handler.ExecuteWithLogging(command, logger, cancellationToken).ConfigureAwait(false);
        result = result.EnsureNotEmpty();
        result = result.EnsureMaxSize(10_000_000); // 10MB limit
        
        return result;
    }
}
