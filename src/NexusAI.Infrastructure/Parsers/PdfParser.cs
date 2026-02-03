using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Text;

namespace NexusAI.Infrastructure.Parsers;

public sealed class PdfParser : IDocumentParserWithMetadata
{
    public string[] SupportedExtensions => [".pdf"];
    public string DisplayName => "PDF Documents";

    public async Task<Result<SourceDocument>> ParseAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(filePath))
                return Result.Failure<SourceDocument>($"PDF file not found: {filePath}");

            var content = await Task.Run(() => ExtractTextFromPdf(filePath), cancellationToken)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(content))
                return Result.Failure<SourceDocument>("PDF appears to be empty or unreadable");

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
            return Result.Failure<SourceDocument>($"Failed to parse PDF: {ex.Message}");
        }
    }

    private static string ExtractTextFromPdf(string filePath)
    {
        using var pdfReader = new PdfReader(filePath);
        using var pdfDocument = new PdfDocument(pdfReader);
        
        var sb = new StringBuilder();
        var pageCount = pdfDocument.GetNumberOfPages();

        for (var i = 1; i <= pageCount; i++)
        {
            var page = pdfDocument.GetPage(i);
            var strategy = new SimpleTextExtractionStrategy();
            var pageText = PdfTextExtractor.GetTextFromPage(page, strategy);
            sb.AppendLine(pageText);
        }

        return sb.ToString();
    }
}
