using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

namespace NexusAI.Infrastructure.Services;

public sealed class PresentationService : IPresentationService
{
    public Result<bool> ValidateOutputPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return Result.Failure<bool>("Output path cannot be empty");

        if (!path.EndsWith(".pptx", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<bool>("Output file must have .pptx extension");

        try
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                return Result.Failure<bool>($"Directory does not exist: {directory}");
            }

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Invalid path: {ex.Message}");
        }
    }

    public async Task<Result<string>> CreatePresentationAsync(
        SlideDeck deck,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.Run(() =>
            {
                using var presentationDocument = PresentationDocument.Create(outputPath, PresentationDocumentType.Presentation);
                
                var presentationPart = presentationDocument.AddPresentationPart();
                presentationPart.Presentation = new P.Presentation();

                CreatePresentationParts(presentationPart);

                var slideIdList = new SlideIdList();
                uint slideId = 256;

                foreach (var (slideContent, index) in deck.Slides.Select((s, i) => (s, i)))
                {
                    var slidePart = CreateSlidePart(presentationPart);
                    
                    if (index == 0)
                    {
                        CreateTitleSlide(slidePart, slideContent);
                    }
                    else
                    {
                        CreateContentSlide(slidePart, slideContent);
                    }

                    var slideId1 = new SlideId { Id = slideId, RelationshipId = presentationPart.GetIdOfPart(slidePart) };
                    slideIdList.Append(slideId1);
                    slideId++;
                }

                presentationPart.Presentation.SlideIdList = slideIdList;
                presentationPart.Presentation.Save();
                
            }, cancellationToken).ConfigureAwait(false);

            return Result.Success(outputPath);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<string>("Presentation creation was cancelled");
        }
        catch (Exception ex)
        {
            return Result.Failure<string>($"Failed to create presentation: {ex.Message}");
        }
    }

    private static void CreatePresentationParts(PresentationPart presentationPart)
    {
        var slideMasterIdList = new SlideMasterIdList(new SlideMasterId { Id = 2147483648U, RelationshipId = "rId1" });
        var slideSize = new SlideSize { Cx = 9144000, Cy = 6858000, Type = SlideSizeValues.Screen16x9 };
        var notesSize = new NotesSize { Cx = 6858000, Cy = 9144000 };
        var defaultTextStyle = new DefaultTextStyle();

        presentationPart.Presentation.Append(slideMasterIdList, slideSize, notesSize, defaultTextStyle);

        var slideMasterPart = presentationPart.AddNewPart<SlideMasterPart>("rId1");
        var slideLayoutPart = slideMasterPart.AddNewPart<SlideLayoutPart>("rId1");
        var slideLayoutPart2 = slideMasterPart.AddNewPart<SlideLayoutPart>("rId2");

        slideMasterPart.SlideMaster = CreateSlideMaster();
        slideLayoutPart.SlideLayout = CreateTitleSlideLayout();
        slideLayoutPart2.SlideLayout = CreateContentSlideLayout();
    }

    private static SlideMaster CreateSlideMaster()
    {
        var slideMaster = new SlideMaster(
            new P.CommonSlideData(new P.ShapeTree()),
            new P.ColorMap
            {
                Background1 = A.ColorSchemeIndexValues.Light1,
                Text1 = A.ColorSchemeIndexValues.Dark1,
                Background2 = A.ColorSchemeIndexValues.Light2,
                Text2 = A.ColorSchemeIndexValues.Dark2,
                Accent1 = A.ColorSchemeIndexValues.Accent1,
                Accent2 = A.ColorSchemeIndexValues.Accent2,
                Accent3 = A.ColorSchemeIndexValues.Accent3,
                Accent4 = A.ColorSchemeIndexValues.Accent4,
                Accent5 = A.ColorSchemeIndexValues.Accent5,
                Accent6 = A.ColorSchemeIndexValues.Accent6,
                Hyperlink = A.ColorSchemeIndexValues.Hyperlink,
                FollowedHyperlink = A.ColorSchemeIndexValues.FollowedHyperlink
            },
            new SlideLayoutIdList(
                new SlideLayoutId { Id = 2147483649U, RelationshipId = "rId1" },
                new SlideLayoutId { Id = 2147483650U, RelationshipId = "rId2" }
            )
        );
        return slideMaster;
    }

    private static SlideLayout CreateTitleSlideLayout()
    {
        return new SlideLayout(
            new P.CommonSlideData(new P.ShapeTree()),
            new P.ColorMapOverride(new A.MasterColorMapping())
        ) { Type = SlideLayoutValues.Title };
    }

    private static SlideLayout CreateContentSlideLayout()
    {
        return new SlideLayout(
            new P.CommonSlideData(new P.ShapeTree()),
            new P.ColorMapOverride(new A.MasterColorMapping())
        ) { Type = SlideLayoutValues.Object };
    }

    private static SlidePart CreateSlidePart(PresentationPart presentationPart)
    {
        var slidePart = presentationPart.AddNewPart<SlidePart>();
        slidePart.Slide = new P.Slide(
            new P.CommonSlideData(new P.ShapeTree()),
            new P.ColorMapOverride(new A.MasterColorMapping())
        );
        return slidePart;
    }

    private static void CreateTitleSlide(SlidePart slidePart, SlideContent content)
    {
        var shapeTree = slidePart.Slide.CommonSlideData!.ShapeTree!;

        var titleShape = CreateTextBox(1524000, 1143000, 6858000, 1600000, content.Title, 44, true);
        var subtitleShape = CreateTextBox(1524000, 3048000, 6858000, 1800000,
            content.BodyPoints.Length > 0 ? string.Join("\n", content.BodyPoints) : "Generated by NexusAI",
            28, false);

        shapeTree.Append(titleShape, subtitleShape);
    }

    private static void CreateContentSlide(SlidePart slidePart, SlideContent content)
    {
        var shapeTree = slidePart.Slide.CommonSlideData!.ShapeTree!;

        var titleShape = CreateTextBox(457200, 274638, 8229600, 1143000, content.Title, 32, true);
        
        var bulletText = string.Join("\n\n", content.BodyPoints.Select(p => $"• {p}"));
        var contentShape = CreateTextBox(457200, 1600000, 8229600, 4572000, bulletText, 20, false);

        shapeTree.Append(titleShape, contentShape);

        if (!string.IsNullOrWhiteSpace(content.SpeakerNotes))
        {
            var notesSlidePart = slidePart.AddNewPart<NotesSlidePart>();
            notesSlidePart.NotesSlide = new NotesSlide(
                new P.CommonSlideData(new P.ShapeTree(
                    CreateTextBox(0, 0, 6858000, 5000000, content.SpeakerNotes, 14, false)
                ))
            );
        }
    }

    private static P.Shape CreateTextBox(long x, long y, long cx, long cy, string text, int fontSize, bool isBold)
    {
        var shape = new P.Shape();

        var nonVisualShapeProperties = new P.NonVisualShapeProperties(
            new P.NonVisualDrawingProperties { Id = 1U, Name = "TextBox" },
            new P.NonVisualShapeDrawingProperties(new A.ShapeLocks { NoGrouping = true }),
            new P.ApplicationNonVisualDrawingProperties(new P.PlaceholderShape())
        );

        var shapeProperties = new P.ShapeProperties();
        var transform2D = new A.Transform2D();
        transform2D.Append(new A.Offset { X = x, Y = y });
        transform2D.Append(new A.Extents { Cx = cx, Cy = cy });
        shapeProperties.Append(transform2D);
        shapeProperties.Append(new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle });

        var textBody = new P.TextBody();
        textBody.Append(new A.BodyProperties());
        textBody.Append(new A.ListStyle());

        var paragraph = new A.Paragraph();
        var run = new A.Run();
        run.Append(new A.RunProperties { FontSize = fontSize * 100, Bold = isBold, Language = "en-US" });
        run.Append(new A.Text(text));
        paragraph.Append(run);
        textBody.Append(paragraph);

        shape.Append(nonVisualShapeProperties);
        shape.Append(shapeProperties);
        shape.Append(textBody);

        return shape;
    }
}
