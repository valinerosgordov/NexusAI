namespace NexusAI.Domain.Models;

public record ScaffoldFile(
    string Path,
    string Content,
    string Language = "",
    bool IsDirectory = false
);

public record ScaffoldResult(
    string RootPath,
    ScaffoldFile[] Files,
    int CreatedFiles,
    int CreatedDirectories
);
