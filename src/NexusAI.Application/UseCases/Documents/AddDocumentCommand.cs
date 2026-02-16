#pragma warning disable MA0048
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Documents;

public sealed record AddDocumentCommand(string FilePath);

public sealed class AddDocumentHandler
{
    private readonly IDocumentParserFactory _parserFactory;

    public AddDocumentHandler(IDocumentParserFactory parserFactory)
    {
        _parserFactory = parserFactory;
    }

    public async Task<Result<SourceDocument>> HandleAsync(
        AddDocumentCommand command,
        CancellationToken cancellationToken = default)
    {
        var parser = _parserFactory.GetParser(command.FilePath);
        
        if (parser is null)
        {
            var extension = Path.GetExtension(command.FilePath);
            return Result.Failure<SourceDocument>($"Unsupported file type: {extension}");
        }

        return await parser.ParseAsync(command.FilePath, cancellationToken).ConfigureAwait(false);
    }
}
#pragma warning restore MA0048
