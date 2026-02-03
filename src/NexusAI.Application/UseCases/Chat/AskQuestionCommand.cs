using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Chat;

public sealed record AskQuestionCommand(
    string Question,
    SourceDocument[] IncludedSources,
    string[]? Base64Images = null
);

public sealed class AskQuestionHandler
{
    private readonly IAiService _aiService;
    private const int MaxInputTokens = 1_000_000;
    private const int CharsPerToken = 4;
    private const int MaxContextChars = MaxInputTokens * CharsPerToken;

    public AskQuestionHandler(IAiService aiService)
    {
        _aiService = aiService;
    }

    public async Task<Result<(ChatMessage Message, bool WasTruncated, int TokenCount)>> HandleAsync(
        AskQuestionCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Question))
            return Result.Failure<(ChatMessage, bool, int)>("Question cannot be empty");

        if (command.IncludedSources.Length == 0)
            return Result.Failure<(ChatMessage, bool, int)>("No sources are currently included. Please add and include at least one document.");

        var (context, wasTruncated, tokenCount) = AggregateContext(command.IncludedSources);

        Result<AiResponse> aiResult;
        
        if (command.Base64Images is not null && command.Base64Images.Length > 0)
        {
            aiResult = await _aiService.AskQuestionWithImagesAsync(
                command.Question, 
                context, 
                command.Base64Images, 
                cancellationToken);
        }
        else
        {
            aiResult = await _aiService.AskQuestionAsync(
                command.Question, 
                context, 
                cancellationToken);
        }

        if (aiResult.IsFailure)
        {
            return Result.Failure<(ChatMessage, bool, int)>(aiResult.Error);
        }

        var assistantMessage = new ChatMessage(
            Id: ChatMessageId.NewId(),
            Content: aiResult.Value.Content,
            Role: MessageRole.Assistant,
            Timestamp: DateTime.UtcNow,
            SourceCitations: aiResult.Value.SourcesCited
        );

        return Result.Success((assistantMessage, wasTruncated, tokenCount));
    }

    private static (string Context, bool WasTruncated, int TokenCount) AggregateContext(SourceDocument[] sources)
    {
        var sb = new System.Text.StringBuilder();
        int totalChars = 0;
        int added = 0;
        bool wasTruncated = false;

        foreach (var source in sources)
        {
            var block = $"""
                <Source filename="{source.Name}">
                {source.Content}
                </Source>

                """;
            
            if (totalChars + block.Length > MaxContextChars)
            {
                wasTruncated = true;
                break;
            }
            sb.Append(block);
            totalChars += block.Length;
            added++;
        }

        if (wasTruncated)
        {
            sb.AppendLine($"\n⚠️ Context truncated. {sources.Length - added} source(s) omitted due to token limit.");
        }

        int tokenCount = totalChars / CharsPerToken;

        return (sb.ToString(), wasTruncated, tokenCount);
    }
}
