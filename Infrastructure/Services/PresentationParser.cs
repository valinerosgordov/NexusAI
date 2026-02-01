using NexusAI.Application.Interfaces;
using NexusAI.Domain;
using NexusAI.Domain.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using System.IO;
using System.Linq;
using System.Text;

namespace NexusAI.Infrastructure.Services;

public sealed class PresentationParser : IDocumentParser
{
    public bool CanParse(string extension) =>
        extension.Equals(".pptx", StringComparison.OrdinalIgnoreCase);

    public async Task<Result<SourceDocument>> ParseAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(filePath))
                return Result.Failure<SourceDocument>($"PPTX file not found: {filePath}");

            var content = await Task.Run(() => ExtractTextFromPptx(filePath), cancellationToken)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(content))
                return Result.Failure<SourceDocument>("PPTX appears to be empty or unreadable");

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
            return Result.Failure<SourceDocument>($"Failed to parse PPTX: {ex.Message}");
        }
    }

    private static string ExtractTextFromPptx(string filePath)
    {
        using var presentation = PresentationDocument.Open(filePath, false);
        var presentationPart = presentation.PresentationPart;
        
        if (presentationPart?.Presentation is null)
            return string.Empty;

        var sb = new StringBuilder();
        var slideIdList = presentationPart.Presentation.SlideIdList;

        if (slideIdList is null)
            return string.Empty;

        var slideNumber = 1;
        foreach (var slideId in slideIdList.Elements<SlideId>())
        {
            var slidePart = (SlidePart)presentationPart.GetPartById(slideId.RelationshipId!);
            sb.AppendLine($"--- Slide {slideNumber} ---");
            sb.AppendLine(GetSlideText(slidePart));
            sb.AppendLine();
            slideNumber++;
        }

        return sb.ToString();
    }

    private static string GetSlideText(SlidePart slidePart)
    {
        var sb = new StringBuilder();
        var slide = slidePart.Slide;

        if (slide is null)
            return string.Empty;

        var textElements = slide.Descendants<DocumentFormat.OpenXml.Drawing.Text>();
        
        foreach (var text in textElements)
        {
            if (!string.IsNullOrWhiteSpace(text.Text))
                sb.AppendLine(text.Text);
        }

        return sb.ToString();
    }
}
