namespace NexusAI.Domain.Models;

public record ScaffoldFile(
    string Path,
    string Content,
    string Language = "",
    bool IsDirectory = false
);
