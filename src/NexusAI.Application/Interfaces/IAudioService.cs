using NexusAI.Domain.Common;

namespace NexusAI.Application.Interfaces;

public interface IAudioService
{
    Task<Result<bool>> SpeakAsync(string text, CancellationToken cancellationToken = default);
    void Pause();
    void Resume();
    void Stop();
}
