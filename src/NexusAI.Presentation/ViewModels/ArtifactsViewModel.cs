using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NexusAI.Application.UseCases.Artifacts;
using NexusAI.Domain.Models;
using System.Collections.ObjectModel;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;

namespace NexusAI.Presentation.ViewModels;

public sealed partial class ArtifactsViewModel(GenerateArtifactHandler generateArtifactHandler) : ObservableObject
{
    private readonly GenerateArtifactHandler _generateArtifactHandler = generateArtifactHandler;

    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isThinking;
    [ObservableProperty] private Artifact? _selectedArtifact;

    public ObservableCollection<Artifact> Artifacts { get; } = [];

    public event EventHandler<MessageEventArgs>? StatusChanged;
    public Func<SourceDocument[]>? GetIncludedSources { get; set; }
    public Func<string>? GetApiKey { get; set; }

    [RelayCommand]
    private async Task GenerateFAQAsync() => await GenerateArtifactAsync(ArtifactType.FAQ).ConfigureAwait(true);

    [RelayCommand]
    private async Task GenerateStudyGuideAsync() => await GenerateArtifactAsync(ArtifactType.StudyGuide).ConfigureAwait(true);

    [RelayCommand]
    private async Task GeneratePodcastScriptAsync() => await GenerateArtifactAsync(ArtifactType.PodcastScript).ConfigureAwait(true);

    [RelayCommand]
    private async Task GenerateNotebookGuideAsync() => await GenerateArtifactAsync(ArtifactType.NotebookGuide).ConfigureAwait(true);

    [RelayCommand]
    private async Task GenerateSummaryAsync() => await GenerateArtifactAsync(ArtifactType.Summary).ConfigureAwait(true);

    [RelayCommand]
    private async Task GenerateOutlineAsync() => await GenerateArtifactAsync(ArtifactType.Outline).ConfigureAwait(true);

    private async Task GenerateArtifactAsync(ArtifactType type)
    {
        var apiKey = GetApiKey?.Invoke() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            MessageBox.Show("Please enter your Gemini API key in settings", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var includedSources = GetIncludedSources?.Invoke() ?? [];
        
        if (includedSources.Length == 0)
        {
            MessageBox.Show("Please select at least one source to analyze", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        IsBusy = true;
        IsThinking = true;
        OnStatusChanged($"Generating {type}...");

        try
        {
            var command = new GenerateArtifactCommand(type, includedSources);
            var result = await _generateArtifactHandler.HandleAsync(command).ConfigureAwait(true);

            if (result.IsSuccess)
            {
                Artifacts.Add(result.Value);
                SelectedArtifact = result.Value;
                OnStatusChanged($"✅ {type} generated successfully");
            }
            else
            {
                MessageBox.Show(result.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                OnStatusChanged($"Failed to generate {type}");
            }
        }
        finally
        {
            IsBusy = false;
            IsThinking = false;
        }
    }

    private void OnStatusChanged(string message) => StatusChanged?.Invoke(this, new MessageEventArgs(message));
}
