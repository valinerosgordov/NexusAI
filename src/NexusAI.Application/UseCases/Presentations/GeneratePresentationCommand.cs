using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Presentations;

public sealed record GeneratePresentationCommand(
    string Topic,
    int SlideCount,
    SourceDocument[] Sources,
    string OutputPath
);

public sealed class GeneratePresentationHandler(
    IAiService aiService,
    IPresentationService presentationService)
{
    public async Task<Result<string>> HandleAsync(
        GeneratePresentationCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Topic))
            return Result.Failure<string>("Topic cannot be empty");

        if (command.SlideCount < 3 || command.SlideCount > 20)
            return Result.Failure<string>("Slide count must be between 3 and 20");

        if (command.Sources.Length == 0)
            return Result.Failure<string>("At least one source document is required");

        var pathValidation = presentationService.ValidateOutputPath(command.OutputPath);
        if (pathValidation.IsFailure)
            return Result.Failure<string>(pathValidation.Error);

        var aiResult = await aiService.GeneratePresentationStructureAsync(
            command.Topic,
            command.SlideCount,
            command.Sources,
            cancellationToken);

        if (aiResult.IsFailure)
            return Result.Failure<string>($"AI generation failed: {aiResult.Error}");

        var deck = new SlideDeck(command.Topic, aiResult.Value);

        var createResult = await presentationService.CreatePresentationAsync(
            deck,
            command.OutputPath,
            cancellationToken);

        if (createResult.IsFailure)
            return Result.Failure<string>($"Failed to create presentation: {createResult.Error}");

        return Result.Success(createResult.Value);
    }
}
