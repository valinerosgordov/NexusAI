namespace NexusAI.Domain.Models;

public record ScaffoldResult(
    string RootPath,
    ScaffoldFile[] Files,
    int CreatedFiles,
    int CreatedDirectories
);
