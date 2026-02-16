namespace NexusAI.Domain.Models;

public record Artifact(
    ArtifactId Id,
    ArtifactType Type,
    string Title,
    string Content,
    DateTime GeneratedAt,
    string[]? SourceNames = null
);
