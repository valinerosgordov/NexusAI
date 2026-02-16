using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Infrastructure.Services;

public sealed class AiServiceFactory : IAiServiceFactory
{
    private readonly GeminiAiService _geminiService;
    private readonly OllamaService _ollamaService;

    public AiServiceFactory(GeminiAiService geminiService, OllamaService ollamaService)
    {
        _geminiService = geminiService;
        _ollamaService = ollamaService;
    }

    public IAiService GetService(AiProvider provider) => provider switch
    {
        AiProvider.Gemini => _geminiService,
        AiProvider.Ollama => _ollamaService,
        _ => _geminiService
    };

    public async Task<bool> IsServiceAvailableAsync(AiProvider provider, CancellationToken cancellationToken = default)
    {
        return provider switch
        {
            AiProvider.Ollama => await _ollamaService.IsOllamaRunningAsync(cancellationToken).ConfigureAwait(false),
            AiProvider.Gemini => true, // Always available if API key is set
            _ => false
        };
    }

    public async Task<Result<string[]>> GetAvailableModelsAsync(AiProvider provider, CancellationToken cancellationToken = default)
    {
        if (provider == AiProvider.Ollama)
        {
            return await _ollamaService.GetAvailableModelsAsync(cancellationToken).ConfigureAwait(false);
        }
        
        return Result.Success(Array.Empty<string>());
    }

    public void ConfigureModel(AiProvider provider, string modelName)
    {
        if (provider == AiProvider.Ollama)
        {
            _ollamaService.SelectedModel = modelName;
        }
    }
}
