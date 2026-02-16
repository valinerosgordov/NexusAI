using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Infrastructure.Services;

public class ScaffoldingService : IScaffoldingService
{
    public Result<bool> ValidatePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return Result.Failure<bool>("Path cannot be empty");

        try
        {
            var fullPath = Path.GetFullPath(path);
            
            if (!Directory.Exists(fullPath))
                return Result.Failure<bool>("Directory does not exist");

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Invalid path: {ex.Message}");
        }
    }

#pragma warning disable MA0051
    public async Task<Result<ScaffoldResult>> CreateStructureAsync(
        string rootPath,
        ScaffoldFile[] files,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
            return Result.Failure<ScaffoldResult>("Root path cannot be empty");

        if (files is not { Length: > 0 })
            return Result.Failure<ScaffoldResult>("No files to create");

        var validationResult = ValidatePath(rootPath);
        if (!validationResult.IsSuccess)
            return Result.Failure<ScaffoldResult>(validationResult.Error);

        try
        {
            var createdFiles = 0;
            var createdDirectories = 0;

            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var fullPath = Path.Combine(rootPath, file.Path.TrimStart('/', '\\'));

                if (file.IsDirectory)
                {
                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                        createdDirectories++;
                    }
                }
                else
                {
                    // Ensure parent directory exists
                    var directory = Path.GetDirectoryName(fullPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                        createdDirectories++;
                    }

                    // Write file content
                    await File.WriteAllTextAsync(fullPath, file.Content, cancellationToken)
                        .ConfigureAwait(false);
                    createdFiles++;
                }
            }

            var result = new ScaffoldResult(
                rootPath,
                files,
                createdFiles,
                createdDirectories
            );

            return Result.Success(result);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<ScaffoldResult>("Operation was cancelled");
        }
        catch (UnauthorizedAccessException)
        {
            return Result.Failure<ScaffoldResult>("Access denied. Please check folder permissions");
        }
        catch (IOException ex)
        {
            return Result.Failure<ScaffoldResult>($"File system error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result.Failure<ScaffoldResult>($"Unexpected error: {ex.Message}");
        }
    }
#pragma warning restore MA0051
}
