namespace NexusAI.Domain.Models;

public record ArtifactId(Guid Value)
{
    public static ArtifactId NewId() => new(Guid.NewGuid());
}
