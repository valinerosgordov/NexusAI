namespace NexusAI.Domain.Models;

public record SlideDeck(
    string Topic,
    SlideContent[] Slides
);
