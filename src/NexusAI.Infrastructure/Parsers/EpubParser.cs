using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;
using System.Text;
using VersOne.Epub;

namespace NexusAI.Infrastructure.Parsers;

public sealed class EpubParser : IDocumentParserWithMetadata
{
    public string[] SupportedExtensions => [".epub"];
    public string DisplayName => "EPUB Books";

    public async Task<Result<SourceDocument>> ParseAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(filePath))
                return Result.Failure<SourceDocument>($"EPUB file not found: {filePath}");

            var content = await Task.Run(() => ExtractTextFromEpub(filePath), cancellationToken)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(content))
                return Result.Failure<SourceDocument>("EPUB appears to be empty or unreadable");

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
            return Result.Failure<SourceDocument>($"Failed to parse EPUB: {ex.Message}");
        }
    }

    private static readonly System.Text.RegularExpressions.Regex HtmlTagRegex =
        new(@"<[^>]+>", System.Text.RegularExpressions.RegexOptions.None, TimeSpan.FromSeconds(1));

    private static string ExtractTextFromEpub(string filePath)
    {
        var book = EpubReader.ReadBook(filePath);
        var sb = new StringBuilder();
        foreach (var htmlContent in book.ReadingOrder.Select(chapter => chapter.Content).Where(c => !string.IsNullOrWhiteSpace(c)))
        {
            var text = HtmlTagRegex.Replace(htmlContent, string.Empty);

            text = System.Net.WebUtility.HtmlDecode(text);

            if (!string.IsNullOrWhiteSpace(text))
            {
                sb.AppendLine(text);
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }
}
