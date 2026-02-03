using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;
using System.Text;

namespace NexusAI.Infrastructure.Parsers;

public sealed class TextParser : IDocumentParserWithMetadata
{
    public string[] SupportedExtensions => [".txt", ".md"];
    public string DisplayName => "Text Files";

    public async Task<Result<SourceDocument>> ParseAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(filePath))
                return Result.Failure<SourceDocument>($"File not found: {filePath}");

            var content = await File.ReadAllTextAsync(filePath, Encoding.UTF8, cancellationToken)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(content))
                return Result.Failure<SourceDocument>("File appears to be empty");

            var document = new SourceDocument(
                Id: SourceDocumentId.NewId(),
                Name: Path.GetFileNameWithoutExtension(filePath),
                Content: content,
                Type: SourceType.Document,
                FilePath: filePath,
                LoadedAt: DateTime.UtcNow,
                IsIncluded: true
            );

            return Result.Success(document);
        }
        catch (Exception ex)
        {
            return Result.Failure<SourceDocument>($"Failed to parse text file: {ex.Message}");
        }
    }
}
