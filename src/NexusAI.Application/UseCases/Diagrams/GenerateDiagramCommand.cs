#pragma warning disable MA0048
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;

namespace NexusAI.Application.UseCases.Diagrams;

public record GenerateDiagramCommand(string ProjectContext, string DiagramType);

public class GenerateDiagramHandler(IAiService aiService)
{
    public async Task<Result<string>> HandleAsync(
        GenerateDiagramCommand command,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.ProjectContext))
            return Result.Failure<string>("Project context is required");

        if (string.IsNullOrWhiteSpace(command.DiagramType))
            return Result.Failure<string>("Diagram type is required");

        var result = await aiService.GenerateMermaidDiagramAsync(
            command.ProjectContext,
            command.DiagramType,
            ct).ConfigureAwait(false);

        return result;
    }
}
#pragma warning restore MA0048
