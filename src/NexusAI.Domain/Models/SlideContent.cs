namespace NexusAI.Domain.Models;

public record SlideContent(
    string Title,
    string[] BodyPoints,
    string? SpeakerNotes = null
);

public record SlideDeck(
    string Topic,
    SlideContent[] Slides
);
