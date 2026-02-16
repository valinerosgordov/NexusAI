using NexusAI.Domain.Common;

namespace NexusAI.Application.Interfaces;

#pragma warning disable CA1716
public interface IAudioService
{
    Task<Result<bool>> SpeakAsync(string text, CancellationToken cancellationToken = default);
    void Pause();
    void Resume();
    void Stop();
}
#pragma warning restore CA1716
