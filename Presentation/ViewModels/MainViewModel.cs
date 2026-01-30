using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalNBV.Application.Services;
using PersonalNBV.Domain;
using PersonalNBV.Domain.Models;
using PersonalNBV.Infrastructure;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace PersonalNBV.Presentation.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly KnowledgeHubService _knowledgeHub;
    private readonly ApiKeyHolder _apiKeyHolder;

    [ObservableProperty]
    private string _geminiApiKey = string.Empty;

    [ObservableProperty]
    private string _obsidianVaultPath = string.Empty;

    [ObservableProperty]
    private string _obsidianSubfolder = string.Empty;

    [ObservableProperty]
    private string _userQuestion = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private bool _isDarkTheme = false;

    [ObservableProperty]
    private bool _isThinking = false;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isErrorVisible;

    [ObservableProperty]
    private Artifact? _selectedArtifact;

    [ObservableProperty]
    private string[] _followUpQuestions = [];

    public ObservableCollection<SourceDocumentViewModel> Sources { get; } = [];
    public ObservableCollection<ChatMessageViewModel> ChatMessages { get; } = [];
    public ObservableCollection<Artifact> Artifacts { get; } = [];

    public MainViewModel(KnowledgeHubService knowledgeHub, ApiKeyHolder apiKeyHolder)
    {
        _knowledgeHub = knowledgeHub;
        _apiKeyHolder = apiKeyHolder;
    }

    [RelayCommand]
    private async Task AddPdfAsync()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "PDF Files (*.pdf)|*.pdf",
            Title = "Select PDF Document"
        };

        if (dialog.ShowDialog() != true)
            return;

        IsBusy = true;
        StatusMessage = "Loading PDF...";

        try
        {
            var result = await _knowledgeHub.AddPdfAsync(dialog.FileName);

            if (result.IsSuccess)
            {
                Sources.Add(new SourceDocumentViewModel(result.Value));
                StatusMessage = $"✅ Loaded: {result.Value.Name}";
                
                // Check context warning
                CheckContextLimits();
            }
            else
            {
                ShowError($"Failed to load PDF: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Unexpected error loading PDF: {ex.Message}");
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
            ShowError("Please enter Obsidian vault path");
            return;
        }

        IsBusy = true;
        var subfolder = string.IsNullOrWhiteSpace(ObsidianSubfolder) ? null : ObsidianSubfolder;
        StatusMessage = subfolder is null 
            ? "Loading Obsidian notes..." 
            : $"Loading notes from '{subfolder}'...";

        try
        {
            var result = await _knowledgeHub.LoadObsidianVaultAsync(ObsidianVaultPath, subfolder);

            if (result.IsSuccess)
            {
                foreach (var source in _knowledgeHub.Sources)
                {
                    if (source.Type == SourceType.ObsidianNote && !Sources.Any(s => s.Id == source.Id))
                    {
                        Sources.Add(new SourceDocumentViewModel(source));
                    }
                }
                var location = subfolder is null ? "vault" : $"'{subfolder}'";
                StatusMessage = $"✅ Loaded {result.Value} notes from {location}";
                
                // Check context warning
                CheckContextLimits();
            }
            else
            {
                ShowError($"Failed to load vault: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Unexpected error loading vault: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ToggleSourceInclusion(SourceDocumentViewModel source)
    {
        _knowledgeHub.ToggleSourceInclusion(source.Id);
        source.IsIncluded = !source.IsIncluded;
    }

    [RelayCommand]
    private void RemoveSource(SourceDocumentViewModel source)
    {
        _knowledgeHub.RemoveSource(source.Id);
        Sources.Remove(source);
        StatusMessage = $"Removed: {source.Name}";
    }

    [RelayCommand]
    private void ClearSources()
    {
        if (MessageBox.Show("Clear all sources?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        _knowledgeHub.ClearSources();
        Sources.Clear();
        StatusMessage = "✅ All sources cleared";
    }

    [RelayCommand(CanExecute = nameof(CanAskQuestion))]
    private async Task AskQuestionAsync()
    {
        if (string.IsNullOrWhiteSpace(GeminiApiKey))
        {
            MessageBox.Show("Please enter your Gemini API key in settings", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsBusy = true;
        IsThinking = true;
        StatusMessage = "Thinking...";

        try
        {
            var result = await _knowledgeHub.AskQuestionAsync(UserQuestion);

            if (result.IsSuccess)
            {
                RefreshChatHistory();
                
                // Generate follow-up questions
                await GenerateFollowUpQuestionsInternalAsync(UserQuestion, result.Value.Content);
                
                UserQuestion = string.Empty;
                StatusMessage = "Response received";
            }
            else
            {
                MessageBox.Show(result.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Failed to get response";
            }
        }
        finally
        {
            IsBusy = false;
            IsThinking = false;
        }
    }

    private bool CanAskQuestion() => !string.IsNullOrWhiteSpace(UserQuestion) && !IsBusy;

    [RelayCommand]
    private async Task GenerateFAQAsync()
    {
        await GenerateArtifactInternalAsync(ArtifactType.FAQ);
    }

    [RelayCommand]
    private async Task GenerateStudyGuideAsync()
    {
        await GenerateArtifactInternalAsync(ArtifactType.StudyGuide);
    }

    [RelayCommand]
    private async Task GeneratePodcastScriptAsync()
    {
        await GenerateArtifactInternalAsync(ArtifactType.PodcastScript);
    }

    [RelayCommand]
    private async Task GenerateNotebookGuideAsync()
    {
        await GenerateArtifactInternalAsync(ArtifactType.NotebookGuide);
    }

    [RelayCommand]
    private async Task GenerateSummaryAsync()
    {
        await GenerateArtifactInternalAsync(ArtifactType.Summary);
    }

    [RelayCommand]
    private async Task GenerateOutlineAsync()
    {
        await GenerateArtifactInternalAsync(ArtifactType.Outline);
    }

    private async Task GenerateArtifactInternalAsync(ArtifactType type)
    {
        if (string.IsNullOrWhiteSpace(GeminiApiKey))
        {
            MessageBox.Show("Please enter your Gemini API key in settings", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var includedCount = Sources.Count(s => s.IsIncluded);
        if (includedCount == 0)
        {
            MessageBox.Show("Please select at least one source to analyze", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        IsBusy = true;
        IsThinking = true;
        StatusMessage = $"Generating {type}...";

        try
        {
            var result = await _knowledgeHub.GenerateArtifactAsync(type);

            if (result.IsSuccess)
            {
                Artifacts.Add(result.Value);
                SelectedArtifact = result.Value;
                StatusMessage = $"✅ {type} generated successfully";
            }
            else
            {
                MessageBox.Show(result.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = $"Failed to generate {type}";
            }
        }
        finally
        {
            IsBusy = false;
            IsThinking = false;
        }
    }

    [RelayCommand]
    private async Task GenerateDeepDiveAsync()
    {
        if (string.IsNullOrWhiteSpace(GeminiApiKey))
        {
            MessageBox.Show("Please enter your Gemini API key in settings", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var includedCount = Sources.Count(s => s.IsIncluded);
        if (includedCount == 0)
        {
            MessageBox.Show("Please select at least one source to analyze", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        IsBusy = true;
        StatusMessage = $"🎬 Generating Deep Dive analysis of {includedCount} source(s)...";

        try
        {
            var result = await _knowledgeHub.GenerateDeepDiveAsync();

            if (result.IsSuccess)
            {
                RefreshChatHistory();
                StatusMessage = "✅ Deep Dive complete";
            }
            else
            {
                MessageBox.Show(result.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Deep Dive failed";
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ExportToObsidianAsync()
    {
        if (string.IsNullOrWhiteSpace(ObsidianVaultPath))
        {
            MessageBox.Show("Please set Obsidian vault path", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (ChatMessages.Count == 0)
        {
            MessageBox.Show("No chat history to export", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var title = $"AI_Chat_{DateTime.Now:yyyyMMdd_HHmmss}";
        var content = BuildChatExport();

        IsBusy = true;
        StatusMessage = "Exporting to Obsidian...";

        try
        {
            var result = await _knowledgeHub.ExportToObsidianAsync(ObsidianVaultPath, title, content);

            if (result.IsSuccess)
            {
                MessageBox.Show($"Exported to:\n{result.Value}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                StatusMessage = "Exported to Obsidian";
            }
            else
            {
                MessageBox.Show(result.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Export failed";
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ClearChat()
    {
        if (MessageBox.Show("Clear chat history?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        _knowledgeHub.ClearChatHistory();
        ChatMessages.Clear();
        StatusMessage = "Chat cleared";
    }

    private async Task GenerateFollowUpQuestionsInternalAsync(string question, string answer)
    {
        try
        {
            var result = await _knowledgeHub.GenerateFollowUpQuestionsAsync(question, answer);
            if (result.IsSuccess)
            {
                FollowUpQuestions = result.Value;
            }
        }
        catch
        {
            // Silently fail - follow-up questions are optional
            FollowUpQuestions = [];
        }
    }

    [RelayCommand]
    private void UseFollowUpQuestion(string question)
    {
        if (!string.IsNullOrWhiteSpace(question))
        {
            UserQuestion = question;
            FollowUpQuestions = []; // Clear suggestions
        }
    }

    private void ShowError(string message)
    {
        StatusMessage = $"❌ {message}";
        ErrorMessage = message;
        IsErrorVisible = true;
        
        // Auto-hide after 8 seconds
        Task.Delay(8000).ContinueWith(_ =>
        {
            IsErrorVisible = false;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void CheckContextLimits()
    {
        var warning = _knowledgeHub.GetContextWarning();
        if (warning != null)
        {
            StatusMessage = warning;
        }
    }

    public void HighlightSourceByName(string sourceName)
    {
        // Clear all highlights
        foreach (var source in Sources)
        {
            source.ClearHighlight();
        }

        // Highlight matching source
        var matchingSource = Sources.FirstOrDefault(s => 
            s.Name.Equals(sourceName, StringComparison.OrdinalIgnoreCase));

        if (matchingSource != null)
        {
            matchingSource.Highlight();
            StatusMessage = $"📍 Highlighted: {sourceName}";
        }
    }

    [RelayCommand]
    private async Task ExportArtifactToObsidianAsync()
    {
        if (SelectedArtifact == null)
        {
            MessageBox.Show("Please select an artifact to export", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (string.IsNullOrWhiteSpace(ObsidianVaultPath))
        {
            MessageBox.Show("Please set Obsidian vault path", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var title = $"{SelectedArtifact.Type}_{DateTime.Now:yyyyMMdd_HHmmss}";
        var content = BuildArtifactExport(SelectedArtifact);

        IsBusy = true;
        StatusMessage = "Exporting artifact to Obsidian...";

        try
        {
            var result = await _knowledgeHub.ExportToObsidianAsync(ObsidianVaultPath, title, content);

            if (result.IsSuccess)
            {
                MessageBox.Show($"Artifact exported to:\n{result.Value}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                StatusMessage = "Artifact exported to Obsidian";
            }
            else
            {
                MessageBox.Show(result.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Export failed";
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void BrowseVaultPath()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select Obsidian Vault Folder"
        };

        if (dialog.ShowDialog() == true)
        {
            ObsidianVaultPath = dialog.FolderName;
        }
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        IsDarkTheme = !IsDarkTheme;
        ApplyTheme(IsDarkTheme);
    }

    private static void ApplyTheme(bool isDark)
    {
        // Update BundledTheme in App.xaml resources
        var app = System.Windows.Application.Current;
        if (app?.Resources.MergedDictionaries.FirstOrDefault(
            d => d is MaterialDesignThemes.Wpf.BundledTheme) is MaterialDesignThemes.Wpf.BundledTheme bundledTheme)
        {
            bundledTheme.BaseTheme = isDark 
                ? MaterialDesignThemes.Wpf.BaseTheme.Dark
                : MaterialDesignThemes.Wpf.BaseTheme.Light;
        }
    }

    public async Task HandleDroppedFilesAsync(string[] files)
    {
        if (files == null || files.Length == 0)
            return;

        IsBusy = true;
        StatusMessage = "Processing dropped files...";

        try
        {
            var loadedCount = 0;
            foreach (var filePath in files)
            {
                var extension = Path.GetExtension(filePath).ToLowerInvariant();

                if (extension == ".pdf")
                {
                    var result = await _knowledgeHub.AddPdfAsync(filePath);
                    if (result.IsSuccess)
                    {
                        Sources.Add(new SourceDocumentViewModel(result.Value));
                        loadedCount++;
                    }
                }
                else if (extension == ".md")
                {
                    // Load single markdown file
                    var result = await LoadMarkdownFileAsync(filePath);
                    if (result.IsSuccess)
                    {
                        Sources.Add(new SourceDocumentViewModel(result.Value));
                        loadedCount++;
                    }
                }
            }

            StatusMessage = loadedCount > 0 
                ? $"Loaded {loadedCount} file(s)" 
                : "No supported files found";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<Result<SourceDocument>> LoadMarkdownFileAsync(string filePath)
    {
        try
        {
            var content = await File.ReadAllTextAsync(filePath);
            var document = new SourceDocument(
                Id: SourceDocumentId.NewId(),
                Name: Path.GetFileNameWithoutExtension(filePath),
                Content: content,
                Type: SourceType.ObsidianNote,
                FilePath: filePath,
                LoadedAt: DateTime.UtcNow,
                IsIncluded: true
            );
            return Result.Success(document);
        }
        catch (Exception ex)
        {
            return Result.Failure<SourceDocument>($"Failed to load markdown: {ex.Message}");
        }
    }

    partial void OnUserQuestionChanged(string value)
    {
        AskQuestionCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsBusyChanged(bool value)
    {
        AskQuestionCommand.NotifyCanExecuteChanged();
    }

    partial void OnGeminiApiKeyChanged(string value)
    {
        _apiKeyHolder.ApiKey = value;
    }

    private void RefreshChatHistory()
    {
        ChatMessages.Clear();
        foreach (var message in _knowledgeHub.ChatHistory)
        {
            ChatMessages.Add(new ChatMessageViewModel(message));
        }
    }

    private string BuildChatExport()
    {
        var lines = new List<string>
        {
            "# AI Chat Export",
            $"*Exported: {DateTime.Now:yyyy-MM-dd HH:mm:ss}*",
            "",
            "## Conversation",
            ""
        };

        foreach (var msg in ChatMessages)
        {
            var role = msg.IsUser ? "**You**" : "**AI**";
            lines.Add($"{role} ({msg.TimeDisplay}):");
            lines.Add(msg.Content);
            if (msg.Citations is not null)
            {
                lines.Add($"*{msg.Citations}*");
            }
            lines.Add("");
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static string BuildArtifactExport(Artifact artifact)
    {
        return $"""
               # {artifact.Title}
               
               **Type**: {artifact.Type}  
               **Generated**: {artifact.GeneratedAt:yyyy-MM-dd HH:mm:ss}  
               **Sources**: {string.Join(", ", artifact.SourceNames)}
               
               ---
               
               {artifact.Content}
               """;
    }
}
