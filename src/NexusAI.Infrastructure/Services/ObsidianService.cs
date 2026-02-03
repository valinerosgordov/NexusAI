using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;
using System.Text;

namespace NexusAI.Infrastructure.Services;

public sealed class ObsidianService : IObsidianService
{
    public async Task<Result<SourceDocument[]>> LoadNotesAsync(string vaultPath, string? subfolder = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Directory.Exists(vaultPath))
                return Result.Failure<SourceDocument[]>($"Vault path does not exist: {vaultPath}");

            var searchPath = string.IsNullOrWhiteSpace(subfolder) 
                ? vaultPath 
                : Path.Combine(vaultPath, subfolder);

            if (!Directory.Exists(searchPath))
                return Result.Failure<SourceDocument[]>($"Subfolder does not exist: {searchPath}");

            var mdFiles = Directory.GetFiles(searchPath, "*.md", SearchOption.AllDirectories);

            if (mdFiles.Length == 0)
                return Result.Success(Array.Empty<SourceDocument>());

            var documents = new List<SourceDocument>();

            foreach (var filePath in mdFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var content = await File.ReadAllTextAsync(filePath, cancellationToken)
                    .ConfigureAwait(false);
                var fileName = Path.GetFileNameWithoutExtension(filePath);

                var document = new SourceDocument(
                    Id: SourceDocumentId.NewId(),
                    Name: fileName,
                    Content: content,
                    Type: SourceType.ObsidianNote,
                    FilePath: filePath,
                    LoadedAt: DateTime.UtcNow,
                    IsIncluded: true
                );

                documents.Add(document);
            }

            return Result.Success(documents.ToArray());
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<SourceDocument[]>("Operation was cancelled");
        }
        catch (Exception ex)
        {
            return Result.Failure<SourceDocument[]>($"Failed to load Obsidian notes: {ex.Message}");
        }
    }

    public async Task<Result<string>> SaveNoteAsync(string vaultPath, string fileName, string content, string[] sourceLinks, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Directory.Exists(vaultPath))
                return Result.Failure<string>($"Vault path does not exist: {vaultPath}");
            var aiNotebookPath = Path.Combine(vaultPath, "AI_Notebook");
            Directory.CreateDirectory(aiNotebookPath);

            var sanitizedFileName = SanitizeFileName(fileName);
            var fullPath = Path.Combine(aiNotebookPath, $"{sanitizedFileName}.md");

            var yamlFrontmatter = CreateYamlFrontmatter(sourceLinks);
            var fullContent = sourceLinks is { Length: > 0 }
                ? $"{yamlFrontmatter}\n\n## Sources\n{string.Join("\n", sourceLinks)}\n\n---\n\n{content}"
                : $"{yamlFrontmatter}\n\n{content}";

            await File.WriteAllTextAsync(fullPath, fullContent, cancellationToken)
                .ConfigureAwait(false);

            return Result.Success(fullPath);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>($"Failed to save note: {ex.Message}");
        }
    }

    private static string CreateYamlFrontmatter(string[] sourceLinks)
    {
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        
        var yaml = new StringBuilder();
        yaml.AppendLine("---");
        yaml.AppendLine("type: ai-synthesis");
        yaml.AppendLine("tags:");
        yaml.AppendLine("  - ai-generated");
        yaml.AppendLine("  - knowledge-hub");
        yaml.AppendLine($"date: {today}");
        yaml.AppendLine($"created: {timestamp}");
        
        if (sourceLinks is { Length: > 0 })
        {
            yaml.AppendLine("sources:");
            foreach (var source in sourceLinks)
            {
                yaml.AppendLine($"  - {source.Trim('[', ']')}");
            }
            yaml.AppendLine("source_count: " + sourceLinks.Length);
        }
        
        yaml.AppendLine("---");
        
        return yaml.ToString();
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new StringBuilder();

        foreach (var c in fileName)
        {
            if (!invalidChars.Contains(c))
                sanitized.Append(c);
            else
                sanitized.Append('_');
        }

        return sanitized.ToString();
    }
}
