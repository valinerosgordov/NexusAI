using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using System.Runtime.Versioning;
using System.Speech.Synthesis;

namespace NexusAI.Infrastructure.Services;

[SupportedOSPlatform("windows")]
public sealed class SpeechSynthesisService : IAudioService, IDisposable
{
    private readonly SpeechSynthesizer _synthesizer;
    private bool _isPaused;

    public bool IsSpeaking => _synthesizer.State == SynthesizerState.Speaking;
    public bool IsPaused => _isPaused;

    public event EventHandler<SpeakProgressEventArgs>? PositionChanged;

    public SpeechSynthesisService()
    {
        _synthesizer = new SpeechSynthesizer();
        _synthesizer.SetOutputToDefaultAudioDevice();
        _synthesizer.Rate = 0;
        _synthesizer.Volume = 100;
        _synthesizer.SpeakProgress += OnSpeakProgress;
    }

    public async Task<Result<bool>> SpeakAsync(string text, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(text))
                return Result.Failure<bool>("Text cannot be empty");

            Stop();
            _isPaused = false;

            await Task.Run(() => _synthesizer.Speak(text), cancellationToken).ConfigureAwait(false);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Speech synthesis failed: {ex.Message}");
        }
    }

    public void Pause()
    {
        if (IsSpeaking && !_isPaused)
        {
            _synthesizer.Pause();
            _isPaused = true;
        }
    }

    public void Resume()
    {
        if (_isPaused)
        {
            _synthesizer.Resume();
            _isPaused = false;
        }
    }

    public void Stop()
    {
        _synthesizer.SpeakAsyncCancelAll();
        _isPaused = false;
    }

    private void OnSpeakProgress(object? sender, SpeakProgressEventArgs e)
    {
        PositionChanged?.Invoke(this, e);
    }

    public void Dispose()
    {
        _synthesizer?.Dispose();
    }
}
