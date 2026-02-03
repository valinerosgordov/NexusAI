using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Scaffold;

public record GenerateScaffoldCommand(
    string ProjectDescription,
    string[] Technologies,
    string TargetPath
);

public class GenerateScaffoldHandler(
    IAiService aiService,
    IScaffoldingService scaffoldingService)
{
    public async Task<Result<ScaffoldResult>> HandleAsync(
        GenerateScaffoldCommand command,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.ProjectDescription))
            return Result.Failure<ScaffoldResult>("Project description is required");

        if (string.IsNullOrWhiteSpace(command.TargetPath))
            return Result.Failure<ScaffoldResult>("Target path is required");

        // Validate target path
        var pathValidation = scaffoldingService.ValidatePath(command.TargetPath);
        if (!pathValidation.IsSuccess)
            return Result.Failure<ScaffoldResult>(pathValidation.Error);

        // Generate file structure using AI
        var scaffoldResult = await aiService.GenerateScaffoldAsync(
            command.ProjectDescription,
            command.Technologies,
            ct);

        if (!scaffoldResult.IsSuccess)
            return Result.Failure<ScaffoldResult>($"AI generation failed: {scaffoldResult.Error}");

        // Create physical files/folders
        var createResult = await scaffoldingService.CreateStructureAsync(
            command.TargetPath,
            scaffoldResult.Value,
            ct);

        return createResult;
    }
}
