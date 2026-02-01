using NexusAI.Application.Interfaces;
using NexusAI.Domain;
using System.Speech.Synthesis;

namespace NexusAI.Infrastructure.Services;

public sealed class SpeechSynthesisService : IAudioService, IDisposable
{
    private readonly SpeechSynthesizer _synthesizer;
    private bool _isPaused;

    public bool IsSpeaking => _synthesizer.State == SynthesizerState.Speaking;
    public bool IsPaused => _isPaused;

    public event EventHandler<int>? PositionChanged;

    public SpeechSynthesisService()
    {
        _synthesizer = new SpeechSynthesizer();
        _synthesizer.SetOutputToDefaultAudioDevice();
        _synthesizer.Rate = 0; // Normal speed
        _synthesizer.Volume = 100; // Max volume
        _synthesizer.SpeakProgress += OnSpeakProgress;
    }

    public async Task<Result<bool>> SpeakAsync(string text, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(text))
                return Result.Failure<bool>("Text cannot be empty");

            Stop(); // Stop any current speech
            _isPaused = false;

            await Task.Run(() => _synthesizer.Speak(text), cancellationToken);
            
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
        PositionChanged?.Invoke(this, e.CharacterPosition);
    }

    public void Dispose()
    {
        _synthesizer?.Dispose();
    }
}
