using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.Interfaces;

/// <summary>
/// Factory for creating and managing AI service instances.
/// Implements Strategy pattern to allow runtime AI provider switching.
/// </summary>
public interface IAiServiceFactory
{
    /// <summary>
    /// Gets the AI service for the specified provider.
    /// </summary>
    IAiService GetService(AiProvider provider);
    
    /// <summary>
    /// Checks if the specified AI provider is available and configured.
    /// </summary>
    Task<bool> IsServiceAvailableAsync(AiProvider provider, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets available models for the specified provider (e.g., Ollama models).
    /// </summary>
    Task<Result<string[]>> GetAvailableModelsAsync(AiProvider provider, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Configures the AI service for the specified provider (e.g., set selected model).
    /// </summary>
    void ConfigureModel(AiProvider provider, string modelName);
}
