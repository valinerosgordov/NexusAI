using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using NexusAI.Application.Interfaces;
using NexusAI.Application.Services;
using NexusAI.Application.UseCases.Artifacts;
using NexusAI.Application.UseCases.Chat;
using NexusAI.Application.UseCases.Diagrams;
using NexusAI.Application.UseCases.Documents;
using NexusAI.Application.UseCases.Obsidian;
using NexusAI.Domain.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace NexusAI.Presentation.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly AddDocumentHandler _addDocumentHandler;
    private readonly AskQuestionHandler _askQuestionHandler;
    private readonly GenerateArtifactHandler _generateArtifactHandler;
    private readonly LoadObsidianVaultHandler _loadObsidianVaultHandler;
    private readonly ExportToObsidianHandler _exportToObsidianHandler;
    private readonly GenerateFollowUpQuestionsHandler _generateFollowUpQuestionsHandler;
    private readonly GenerateDiagramHandler _generateDiagramHandler;
    private readonly IDocumentParserFactory _parserFactory;
    private readonly KnowledgeGraphService _graphService;
    private readonly IAudioService _audioService;
    private readonly IAiServiceFactory _aiServiceFactory;
    private readonly SessionContext _sessionContext;

    [ObservableProperty] private string _geminiApiKey = string.Empty;
    [ObservableProperty] private string _obsidianVaultPath = string.Empty;
    [ObservableProperty] private string _obsidianSubfolder = string.Empty;
    [ObservableProperty] private string _userQuestion = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _statusMessage = "Ready";
    [ObservableProperty] private bool _isDarkTheme = false;
    [ObservableProperty] private bool _isThinking = false;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isErrorVisible;
    [ObservableProperty] private Artifact? _selectedArtifact;
    [ObservableProperty] private string[] _followUpQuestions = [];
    [ObservableProperty] private AiProvider _selectedAiProvider = AiProvider.Gemini;
    [ObservableProperty] private string? _selectedOllamaModel;
    [ObservableProperty] private bool _isOllamaAvailable;
    [ObservableProperty] private bool _isPlayingAudio;
    [ObservableProperty] private string? _currentlySpeakingText;
    [ObservableProperty] private string _currentMermaidDiagram = string.Empty;
    [ObservableProperty] private string _selectedDiagramType = "architecture";

    public string[]? PendingImages { get; set; }
    public ObservableCollection<SourceDocumentViewModel> Sources { get; } = [];
    public ObservableCollection<ChatMessageViewModel> ChatMessages { get; } = [];
    public ObservableCollection<Artifact> Artifacts { get; } = [];
    public ObservableCollection<string> AvailableOllamaModels { get; } = [];
    public ObservableCollection<KnowledgeGraphService.GraphNode> GraphNodes { get; } = [];
    public ObservableCollection<KnowledgeGraphService.GraphEdge> GraphEdges { get; } = [];

    public MainViewModel(
        AddDocumentHandler addDocumentHandler,
        AskQuestionHandler askQuestionHandler,
        GenerateArtifactHandler generateArtifactHandler,
        LoadObsidianVaultHandler loadObsidianVaultHandler,
        ExportToObsidianHandler exportToObsidianHandler,
        GenerateFollowUpQuestionsHandler generateFollowUpQuestionsHandler,
        GenerateDiagramHandler generateDiagramHandler,
        IDocumentParserFactory parserFactory,
        KnowledgeGraphService graphService,
        IAudioService audioService,
        IAiServiceFactory aiServiceFactory,
        SessionContext sessionContext)
    {
        _addDocumentHandler = addDocumentHandler;
        _askQuestionHandler = askQuestionHandler;
        _generateArtifactHandler = generateArtifactHandler;
        _loadObsidianVaultHandler = loadObsidianVaultHandler;
        _exportToObsidianHandler = exportToObsidianHandler;
        _generateFollowUpQuestionsHandler = generateFollowUpQuestionsHandler;
        _generateDiagramHandler = generateDiagramHandler;
        _parserFactory = parserFactory;
        _graphService = graphService;
        _audioService = audioService;
        _aiServiceFactory = aiServiceFactory;
        _sessionContext = sessionContext;
        
        _ = CheckOllamaAvailabilityAsync();
    }

    public SessionContext SessionContext => _sessionContext;

    [RelayCommand]
    private void SwitchAppMode()
    {
        _sessionContext.SwitchMode();
        StatusMessage = $"🔄 Switched to {_sessionContext.ModeDisplayName} mode";
    }

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
        StatusMessage = "Loading document...";
        
        try
        {
            var command = new AddDocumentCommand(dialog.FileName);
            var result = await _addDocumentHandler.HandleAsync(command);
            
            if (result.IsSuccess)
            {
                Sources.Add(new SourceDocumentViewModel(result.Value));
                StatusMessage = $"✅ Loaded: {result.Value.Name}";
            }
            else
            {
                ShowError($"Failed to load document: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Unexpected error loading document: {ex.Message}");
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
                StatusMessage = $"✅ Loaded {result.Value.Length} notes from {location}";
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
        source.IsIncluded = !source.IsIncluded;
    }

    [RelayCommand]
    private void RemoveSource(SourceDocumentViewModel source)
    {
        Sources.Remove(source);
        StatusMessage = $"Removed: {source.Name}";
    }

    [RelayCommand]
    private void ClearSources()
    {
        if (MessageBox.Show("Clear all sources?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        Sources.Clear();
        StatusMessage = "✅ All sources cleared";
    }

    [RelayCommand(CanExecute = nameof(CanAskQuestion))]
    private async Task AskQuestionAsync()
    {
        if (string.IsNullOrWhiteSpace(GeminiApiKey) && SelectedAiProvider == AiProvider.Gemini)
        {
            MessageBox.Show("Please enter your Gemini API key in settings", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var includedSources = Sources.Where(s => s.IsIncluded).Select(s => s.Document).ToArray();
        
        IsBusy = true;
        IsThinking = true;
        StatusMessage = PendingImages != null ? $"Analyzing {PendingImages.Length} image(s)..." : "Thinking...";

        try
        {
            var userMessage = new ChatMessage(
                Id: ChatMessageId.NewId(),
                Content: UserQuestion,
                Role: MessageRole.User,
                Timestamp: DateTime.UtcNow
            );
            ChatMessages.Add(new ChatMessageViewModel(userMessage));

            var command = new AskQuestionCommand(UserQuestion, includedSources, PendingImages);
            var result = await _askQuestionHandler.HandleAsync(command);
            
            PendingImages = null;

            if (result.IsSuccess)
            {
                var (message, _, _) = result.Value;
                ChatMessages.Add(new ChatMessageViewModel(message));
                
                await GenerateFollowUpQuestionsInternalAsync(UserQuestion, message.Content);
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
    private async Task GenerateFAQAsync() => await GenerateArtifactInternalAsync(ArtifactType.FAQ);

    [RelayCommand]
    private async Task GenerateStudyGuideAsync() => await GenerateArtifactInternalAsync(ArtifactType.StudyGuide);

    [RelayCommand]
    private async Task GeneratePodcastScriptAsync() => await GenerateArtifactInternalAsync(ArtifactType.PodcastScript);

    [RelayCommand]
    private async Task GenerateNotebookGuideAsync() => await GenerateArtifactInternalAsync(ArtifactType.NotebookGuide);

    [RelayCommand]
    private async Task GenerateSummaryAsync() => await GenerateArtifactInternalAsync(ArtifactType.Summary);

    [RelayCommand]
    private async Task GenerateOutlineAsync() => await GenerateArtifactInternalAsync(ArtifactType.Outline);

    private async Task GenerateArtifactInternalAsync(ArtifactType type)
    {
        if (string.IsNullOrWhiteSpace(GeminiApiKey))
        {
            MessageBox.Show("Please enter your Gemini API key in settings", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var includedSources = Sources.Where(s => s.IsIncluded).Select(s => s.Document).ToArray();
        
        if (includedSources.Length == 0)
        {
            MessageBox.Show("Please select at least one source to analyze", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        IsBusy = true;
        IsThinking = true;
        StatusMessage = $"Generating {type}...";

        try
        {
            var command = new GenerateArtifactCommand(type, includedSources);
            var result = await _generateArtifactHandler.HandleAsync(command);

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
    private void ClearChat()
    {
        if (MessageBox.Show("Clear chat history?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        ChatMessages.Clear();
        StatusMessage = "Chat cleared";
    }

    [RelayCommand]
    private void UseFollowUpQuestion(string question)
    {
        if (!string.IsNullOrWhiteSpace(question))
        {
            UserQuestion = question;
            FollowUpQuestions = [];
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

    [RelayCommand]
    private void RefreshGraph()
    {
        var documents = Sources
            .Where(s => s.IsIncluded)
            .Select(s => s.Document)
            .ToArray();

        var (nodes, edges) = _graphService.BuildGraph(documents);

        GraphNodes.Clear();
        GraphEdges.Clear();

        foreach (var node in nodes)
            GraphNodes.Add(node);

        foreach (var edge in edges)
            GraphEdges.Add(edge);

        StatusMessage = $"Graph: {nodes.Length} nodes, {edges.Length} connections";
    }

    [RelayCommand]
    private async Task GenerateDiagramAsync()
    {
        IsBusy = true;
        StatusMessage = "🎨 AI is generating diagram...";

        try
        {
            var projectContext = BuildProjectContext();
            var command = new GenerateDiagramCommand(projectContext, SelectedDiagramType);
            var result = await _generateDiagramHandler.HandleAsync(command);

            if (result.IsSuccess)
            {
                CurrentMermaidDiagram = result.Value;
                StatusMessage = $"✅ {SelectedDiagramType} diagram generated";
            }
            else
            {
                ShowError($"Failed to generate diagram: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private string BuildProjectContext()
    {
        var includedSources = Sources.Where(s => s.IsIncluded).ToArray();
        
        if (includedSources.Length > 0)
        {
            var docNames = string.Join(", ", includedSources.Select(s => s.Document.FileName));
            return $"Project: NexusAI (Clean Architecture WPF app with AI capabilities). Documents: {docNames}";
        }

        return "Project: NexusAI - A Clean Architecture WPF application with AI-powered features including document analysis, project planning, code scaffolding, and knowledge graphs. Built with .NET 8, MaterialDesign, EF Core, and Gemini AI.";
    }

    private async Task GenerateFollowUpQuestionsInternalAsync(string question, string answer)
    {
        try
        {
            var command = new GenerateFollowUpQuestionsCommand(question, answer);
            var result = await _generateFollowUpQuestionsHandler.HandleAsync(command);
            if (result.IsSuccess)
            {
                FollowUpQuestions = result.Value;
            }
        }
        catch
        {
            FollowUpQuestions = [];
        }
    }

    private void ShowError(string message)
    {
        StatusMessage = $"❌ {message}";
        ErrorMessage = message;
        IsErrorVisible = true;

        Task.Delay(8000).ContinueWith(_ =>
        {
            IsErrorVisible = false;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    partial void OnUserQuestionChanged(string value) => AskQuestionCommand.NotifyCanExecuteChanged();
    partial void OnIsBusyChanged(bool value) => AskQuestionCommand.NotifyCanExecuteChanged();

    private static void ApplyTheme(bool isDark)
    {
        var app = Application.Current;
        if (app?.Resources.MergedDictionaries.FirstOrDefault(
            d => d is MaterialDesignThemes.Wpf.BundledTheme) is MaterialDesignThemes.Wpf.BundledTheme bundledTheme)
        {
            bundledTheme.BaseTheme = isDark 
                ? MaterialDesignThemes.Wpf.BaseTheme.Dark
                : MaterialDesignThemes.Wpf.BaseTheme.Light;
        }
    }

    private async Task CheckOllamaAvailabilityAsync()
    {
        IsOllamaAvailable = await _aiServiceFactory.IsServiceAvailableAsync(AiProvider.Ollama);
        
        if (IsOllamaAvailable)
        {
            await LoadOllamaModelsAsync();
        }
    }

    [RelayCommand]
    private async Task LoadOllamaModelsAsync()
    {
        var result = await _aiServiceFactory.GetAvailableModelsAsync(AiProvider.Ollama);
        
        if (result.IsSuccess)
        {
            AvailableOllamaModels.Clear();
            foreach (var model in result.Value)
            {
                AvailableOllamaModels.Add(model);
            }

            if (AvailableOllamaModels.Count > 0 && string.IsNullOrEmpty(SelectedOllamaModel))
            {
                SelectedOllamaModel = AvailableOllamaModels[0];
            }

            IsOllamaAvailable = true;
            StatusMessage = $"✅ Found {AvailableOllamaModels.Count} Ollama model(s)";
        }
        else
        {
            IsOllamaAvailable = false;
            StatusMessage = "⚠️ Ollama not available";
        }
    }

    partial void OnSelectedOllamaModelChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _aiServiceFactory.ConfigureModel(AiProvider.Ollama, value);
            StatusMessage = $"Ollama model: {value}";
        }
    }
}
