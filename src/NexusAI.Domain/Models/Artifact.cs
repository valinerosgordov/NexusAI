namespace NexusAI.Domain.Models;

public enum ArtifactType
{
    FAQ,
    StudyGuide,
    PodcastScript,
    Summary,
    Outline,
    NotebookGuide
}

public record Artifact(
    ArtifactId Id,
    ArtifactType Type,
    string Title,
    string Content,
    DateTime GeneratedAt,
    string[] SourceNames
);

public readonly record struct ArtifactId(Guid Value)
{
    public static ArtifactId NewId() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
