using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;
using DocumentFormat.OpenXml.Packaging;
using System.Text;

namespace NexusAI.Infrastructure.Parsers;

public sealed class WordParser : IDocumentParserWithMetadata
{
    public string[] SupportedExtensions => [".docx"];
    public string DisplayName => "Word Documents";

    public async Task<Result<SourceDocument>> ParseAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(filePath))
                return Result.Failure<SourceDocument>($"DOCX file not found: {filePath}");

            var content = await Task.Run(() => ExtractTextFromDocx(filePath), cancellationToken)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(content))
                return Result.Failure<SourceDocument>("DOCX appears to be empty or unreadable");

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
            return Result.Failure<SourceDocument>($"Failed to parse DOCX: {ex.Message}");
        }
    }

    private static string ExtractTextFromDocx(string filePath)
    {
        using var doc = WordprocessingDocument.Open(filePath, false);
        var body = doc.MainDocumentPart?.Document.Body;
        
        if (body is null)
            return string.Empty;

        var sb = new StringBuilder();
        
        foreach (var paragraph in body.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
        {
            var text = paragraph.InnerText;
            if (!string.IsNullOrWhiteSpace(text))
                sb.AppendLine(text);
        }

        return sb.ToString();
    }
}
