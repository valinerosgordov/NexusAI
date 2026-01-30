using NexusAI.Application.Interfaces;
using NexusAI.Domain;
using NexusAI.Domain.Models;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.IO;
using System.Text;

namespace NexusAI.Infrastructure.Services;

public sealed class PdfParsingService : IPdfParsingService
{
    // iText 7 — извлечение текста по страницам
    public async Task<Result<SourceDocument>> ParsePdfAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(filePath))
                return Result.Failure<SourceDocument>($"PDF file not found: {filePath}");

            if (!filePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                return Result.Failure<SourceDocument>("File must be a PDF");

            var content = await Task.Run(() => ExtractTextFromPdf(filePath), cancellationToken)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(content))
                return Result.Failure<SourceDocument>("PDF appears to be empty or unreadable");

            var document = new SourceDocument(
                Id: SourceDocumentId.NewId(),
                Name: Path.GetFileNameWithoutExtension(filePath),
                Content: content,
                Type: SourceType.Pdf,
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
