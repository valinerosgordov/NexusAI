using NexusAI.Application.Interfaces;
using NexusAI.Domain;
using NexusAI.Domain.Models;
using System.IO;
using System.Text;
using VersOne.Epub;

namespace NexusAI.Infrastructure.Services;

public sealed class EpubParser : IDocumentParser
{
    public bool CanParse(string extension) =>
        extension.Equals(".epub", StringComparison.OrdinalIgnoreCase);

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

    private static string ExtractTextFromEpub(string filePath)
    {
        var book = EpubReader.ReadBook(filePath);
        var sb = new StringBuilder();
        foreach (var chapter in book.ReadingOrder)
        {
            var htmlContent = chapter.Content;
            
            if (!string.IsNullOrWhiteSpace(htmlContent))
            {

                var text = System.Text.RegularExpressions.Regex.Replace(
                    htmlContent, 
                    @"<[^>]+>", 
                    string.Empty
                );
                

                text = System.Net.WebUtility.HtmlDecode(text);
                
                if (!string.IsNullOrWhiteSpace(text))
                {
                    sb.AppendLine(text);
                    sb.AppendLine();
                }
            }
        }

        return sb.ToString();
    }
}
