namespace NexusAI.Domain.Models;

public record ArtifactId(Guid Value)
{
    public static ArtifactId NewId() => new(Guid.NewGuid());
}

public enum ArtifactType
{
    StudyGuide,
    FAQ,
    NotebookGuide,
    Timeline,
    ConceptMap,
    QuizQuestions,
    Summary,
    PodcastScript,
    Outline
}

public record Artifact(
    ArtifactId Id,
    ArtifactType Type,
    string Title,
    string Content,
    DateTime GeneratedAt,
    string[]? SourceNames = null
);
