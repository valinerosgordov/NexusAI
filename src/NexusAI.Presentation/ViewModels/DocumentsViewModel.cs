using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NexusAI.Application.Interfaces;
using NexusAI.Application.UseCases.Documents;
using NexusAI.Application.UseCases.Obsidian;
using NexusAI.Domain.Models;
using System.Collections.ObjectModel;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace NexusAI.Presentation.ViewModels;

public sealed partial class DocumentsViewModel(
    AddDocumentHandler addDocumentHandler,
    LoadObsidianVaultHandler loadObsidianVaultHandler,
    IDocumentParserFactory parserFactory) : ObservableObject
{
    private readonly AddDocumentHandler _addDocumentHandler = addDocumentHandler;
    private readonly LoadObsidianVaultHandler _loadObsidianVaultHandler = loadObsidianVaultHandler;
    private readonly IDocumentParserFactory _parserFactory = parserFactory;

    [ObservableProperty] private string _obsidianVaultPath = string.Empty;
    [ObservableProperty] private string _obsidianSubfolder = string.Empty;
    [ObservableProperty] private bool _isBusy;

    public ObservableCollection<SourceDocumentViewModel> Sources { get; } = [];

    public event EventHandler<string>? StatusChanged;
    public event EventHandler<string>? ErrorOccurred;

    [RelayCommand]
    private async Task AddDocumentAsync()
    {
        OpenFileDialog dialog = new()
        {
            Filter = _parserFactory.GetFileDialogFilter(),
            Title = "Select Document"
        };
        
        if (dialog.ShowDialog() != true)
            return;

        IsBusy = true;
        OnStatusChanged("Loading document...");
        
        try
        {
            var command = new AddDocumentCommand(dialog.FileName);
            var result = await _addDocumentHandler.HandleAsync(command);
            
            if (result.IsSuccess)
            {
                Sources.Add(new SourceDocumentViewModel(result.Value));
                OnStatusChanged($"✅ Loaded: {result.Value.Name}");
            }
            else
            {
                OnErrorOccurred($"Failed to load document: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"Unexpected error loading document: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadObsidianVaultAsync()
    {
        if (string.IsNullOrWhiteSpace(ObsidianVaultPath))
        {
            OnErrorOccurred("Please enter Obsidian vault path");
            return;
        }

        IsBusy = true;
        var subfolder = string.IsNullOrWhiteSpace(ObsidianSubfolder) ? null : ObsidianSubfolder;
        OnStatusChanged(subfolder is null 
            ? "Loading Obsidian notes..." 
            : $"Loading notes from '{subfolder}'...");

        try
        {
            var command = new LoadObsidianVaultCommand(ObsidianVaultPath, subfolder);
            var result = await _loadObsidianVaultHandler.HandleAsync(command);

            if (result.IsSuccess)
            {
                foreach (var doc in result.Value)
                {
                    if (!Sources.Any(s => s.Id == doc.Id))
                    {
                        Sources.Add(new SourceDocumentViewModel(doc));
                    }
                }
                var location = subfolder is null ? "vault" : $"'{subfolder}'";
                OnStatusChanged($"✅ Loaded {result.Value.Length} notes from {location}");
            }
            else
            {
                OnErrorOccurred($"Failed to load vault: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"Unexpected error loading vault: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ToggleSourceInclusion(SourceDocumentViewModel source)
    {
        source.IsIncluded = !source.IsIncluded;
    }

    [RelayCommand]
    private void RemoveSource(SourceDocumentViewModel source)
    {
        Sources.Remove(source);
        OnStatusChanged($"Removed: {source.Name}");
    }

    [RelayCommand]
    private void ClearSources()
    {
        if (MessageBox.Show("Clear all sources?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        Sources.Clear();
        OnStatusChanged("✅ All sources cleared");
    }

    [RelayCommand]
    private void BrowseVaultPath()
    {
        var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "Select Obsidian Vault Folder"
        };

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            ObsidianVaultPath = dialog.SelectedPath ?? string.Empty;
        }
    }

    public SourceDocument[] GetIncludedSources() =>
        Sources.Where(s => s.IsIncluded).Select(s => s.Document).ToArray();

    private void OnStatusChanged(string message) => StatusChanged?.Invoke(this, message);
    private void OnErrorOccurred(string message) => ErrorOccurred?.Invoke(this, message);
}
