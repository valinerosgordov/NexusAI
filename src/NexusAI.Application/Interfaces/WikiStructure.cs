namespace NexusAI.Application.Interfaces;

public record WikiStructure(string Title, string Content, WikiStructure[] SubPages);
