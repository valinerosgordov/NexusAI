using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NexusAI.Application.UseCases.Presentations;
using NexusAI.Domain.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace NexusAI.Presentation.ViewModels;

public sealed partial class PresentationViewModel : ObservableObject
{
    private readonly GeneratePresentationHandler _handler;
    private readonly Func<SourceDocument[]> _getSourceDocumentsFunc;

    [ObservableProperty] private string _topic = string.Empty;
    [ObservableProperty] private int _slideCount = 10;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private string? _generatedFilePath;

    public ObservableCollection<SlidePreview> Slides { get; } = [];

    public PresentationViewModel(
        GeneratePresentationHandler handler,
        Func<SourceDocument[]> getSourceDocumentsFunc)
    {
        _handler = handler;
        _getSourceDocumentsFunc = getSourceDocumentsFunc;
    }

    [RelayCommand]
    private async Task GeneratePresentationAsync()
    {
        if (string.IsNullOrWhiteSpace(Topic))
        {
            StatusMessage = "❌ Please enter a topic";
            return;
        }

        if (SlideCount < 3 || SlideCount > 20)
        {
            StatusMessage = "❌ Slide count must be between 3 and 20";
            return;
        }

        var sources = _getSourceDocumentsFunc();
        if (sources.Length == 0)
        {
            StatusMessage = "❌ Please add and include at least one document";
            return;
        }

        IsBusy = true;
        StatusMessage = "🧠 AI is analyzing documents and generating presentation structure...";
        Slides.Clear();
        GeneratedFilePath = null;

        try
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PowerPoint Presentation|*.pptx",
                FileName = $"{SanitizeFileName(Topic)}.pptx",
                Title = "Save Presentation"
            };

            if (dialog.ShowDialog() != true)
            {
                StatusMessage = "❌ Cancelled";
                return;
            }

            var command = new GeneratePresentationCommand(
                Topic,
                SlideCount,
                sources,
                dialog.FileName
            );

            StatusMessage = "📊 Creating PowerPoint file...";

            var result = await _handler.HandleAsync(command, CancellationToken.None)
                .ConfigureAwait(true);

            if (result.IsFailure)
            {
                StatusMessage = $"❌ {result.Error}";
                MessageBox.Show(result.Error, "Generation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            GeneratedFilePath = result.Value;
            StatusMessage = $"✅ Presentation created successfully!";

            MessageBox.Show(
                $"Presentation created successfully!\n\nFile: {result.Value}\n\nThe file is now ready to open in PowerPoint.",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Error: {ex.Message}";
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanOpenFile))]
    private void OpenGeneratedFile()
    {
        if (GeneratedFilePath == null || !File.Exists(GeneratedFilePath))
            return;

        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = GeneratedFilePath,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to open file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool CanOpenFile() => !string.IsNullOrEmpty(GeneratedFilePath) && File.Exists(GeneratedFilePath);

    partial void OnGeneratedFilePathChanged(string? value)
    {
        OpenGeneratedFileCommand.NotifyCanExecuteChanged();
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        return string.IsNullOrWhiteSpace(sanitized) ? "presentation" : sanitized;
    }
}

public sealed partial class SlidePreview : ObservableObject
{
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _preview = string.Empty;
    [ObservableProperty] private int _number;
}
