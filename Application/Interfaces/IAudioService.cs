using NexusAI.Domain;

namespace NexusAI.Application.Interfaces;

public interface IAudioService
{
    Task<Result<bool>> SpeakAsync(string text, CancellationToken cancellationToken = default);
    void Pause();
    void Resume();
    void Stop();
    bool IsSpeaking { get; }
    bool IsPaused { get; }
    event EventHandler<int>? PositionChanged;
}
