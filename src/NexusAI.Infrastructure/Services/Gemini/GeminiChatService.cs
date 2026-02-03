using NexusAI.Application.Services;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Infrastructure.Services.Gemini;

internal sealed class GeminiChatService(
    GeminiHttpClient httpClient,
    SessionContext sessionContext)
{
    public async Task<Result<AiResponse>> AskQuestionAsync(
        string question,
        string context,
        CancellationToken cancellationToken = default)
    {
        return await AskQuestionWithImagesAsync(question, context, [], cancellationToken);
    }

    public async Task<Result<AiResponse>> AskQuestionWithImagesAsync(
        string question,
        string context,
        string[] base64Images,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(question))
            return Result.Failure<AiResponse>("Question cannot be empty");

        var body = CreateRequestBody(question, context, base64Images);
        var result = await httpClient.SendRequestAsync(body, cancellationToken);

        if (!result.IsSuccess)
            return Result.Failure<AiResponse>(result.Error);

        var geminiResponse = result.Value;

        if (geminiResponse.Candidates is not { Length: > 0 } candidates)
            return Result.Failure<AiResponse>("No candidates in API response");

        var firstCandidate = candidates[0];
        if (firstCandidate?.Content?.Parts is not { Length: > 0 } parts)
            return Result.Failure<AiResponse>("No content parts in API response");

        var content = parts[0]?.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(content))
            return Result.Failure<AiResponse>("Empty response from AI");

        var sourceCitations = ExtractSourceCitations(content);
        var tokensUsed = geminiResponse.UsageMetadata?.TotalTokenCount ?? 0;

        var aiResponse = new AiResponse(
            Content: content,
            SourcesCited: sourceCitations,
            TokensUsed: tokensUsed
        );

        return Result.Success(aiResponse);
    }

    private object CreateRequestBody(string question, string context, string[] base64Images)
    {
        var fullPrompt = string.IsNullOrWhiteSpace(context)
            ? question
            : $"""
              CONTEXT:
              {context}

              USER QUESTION:
              {question}
              """;

        List<object> parts = [new { text = fullPrompt }];
        
        if (base64Images is not null && base64Images.Length > 0)
        {
            foreach (var imageData in base64Images)
            {
                parts.Add(new
                {
                    inlineData = new
                    {
                        mimeType = "image/jpeg",
                        data = imageData
                    }
                });
            }
        }

        return new
        {
            systemInstruction = new
            {
                parts = new[] { new { text = GetDynamicSystemPrompt() } }
            },
            contents = new[]
            {
                new
                {
                    parts = parts.ToArray()
                }
            },
            generationConfig = new
            {
                temperature = 0.3,
                topK = 40,
                topP = 0.95,
                maxOutputTokens = 2048
            }
        };
    }

    private string GetDynamicSystemPrompt()
    {
        var modePrefix = sessionContext.CurrentMode switch
        {
            AppMode.Professional => """
                You are an Executive Assistant for a professional. Be concise, professional, and focus on business value.
                Prioritize actionable insights, efficiency, and clear recommendations.
                """,
            AppMode.Student => """
                You are a Socratic Tutor helping a student learn. Explain concepts simply, use analogies, and help with study planning.
                Do not just give answers—teach the underlying principles. Ask guiding questions when appropriate.
                Break down complex topics into digestible parts.
                """,
            _ => string.Empty
        };

        return $"""
            {modePrefix}
            
            You are NexusAI, an expert research assistant with transparent reasoning. Answer strictly based on the provided Context.
            
            CRITICAL - SHOW YOUR THINKING:
            Before answering, document your reasoning process using [STEP] markers:
            - [STEP] Reading document X to find information about Y...
            - [STEP] Found relevant section discussing Z...
            - [STEP] Cross-referencing with document W...
            - [STEP] Synthesizing findings...
            
            ANSWER RULES:
            1. GROUNDING: Derive answers ONLY from the context.
            2. CITATIONS: Cite sources using [Filename] format at the end of statements.
            3. HONESTY: If information is missing, state "I cannot find this in the documents."
            4. FORMATTING: Use Markdown.
            
            Example structure:
            [STEP] Analyzing document1.pdf for information about clean architecture...
            [STEP] Found definition on page 3...
            [STEP] Comparing with examples from document2.pdf...
            [STEP] Concluding based on evidence from both sources...
            
            # Answer
            [Your detailed answer with citations here]
            """;
    }

    private static string[] ExtractSourceCitations(string content)
    {
        List<string> list = [];
        int idx = 0;
        while ((idx = content.IndexOf('[', idx)) != -1)
        {
            var end = content.IndexOf(']', idx);
            if (end == -1) break;
            var cit = content.Substring(idx + 1, end - idx - 1);
            if (!string.IsNullOrWhiteSpace(cit) && !list.Contains(cit))
                list.Add(cit);
            idx = end + 1;
        }
        return [.. list];
    }
}
