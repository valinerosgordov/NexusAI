namespace NexusAI.Domain.Models;

public record SlideContent(
    string Title,
    string[] BodyPoints,
    string? SpeakerNotes = null
);
