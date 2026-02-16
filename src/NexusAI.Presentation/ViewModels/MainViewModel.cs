using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NexusAI.Application.Interfaces;
using NexusAI.Application.Services;
using NexusAI.Domain.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace NexusAI.Presentation.ViewModels;

/// <summary>
/// Main coordinator ViewModel - delegates responsibilities to focused sub-ViewModels
/// </summary>
public sealed partial class MainViewModel : ObservableObject
{
    private readonly IAiServiceFactory _aiServiceFactory;
    private readonly SessionContext _sessionContext;

    public DocumentsViewModel Documents { get; }
    public ChatViewModel Chat { get; }
    public ArtifactsViewModel Artifacts { get; }
    public GraphViewModel Graph { get; }

    // Sub-ViewModels for navigation (set after construction to avoid circular DI)
    public ProjectViewModel? Project { get; set; }
    public WikiViewModel? Wiki { get; set; }
    public PresentationViewModel? Presentation { get; set; }
    public SettingsViewModel? Settings { get; set; }

    // Navigation
    [ObservableProperty] private AppView _selectedView = AppView.Chat;

    [ObservableProperty] private string _geminiApiKey = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _statusMessage = "Ready";
    [ObservableProperty] private bool _isDarkTheme;
    [ObservableProperty] private bool _isThinking;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isErrorVisible;
    [ObservableProperty] private AiProvider _selectedAiProvider = AiProvider.Gemini;
    [ObservableProperty] private string? _selectedOllamaModel;
    [ObservableProperty] private bool _isOllamaAvailable;
    [ObservableProperty] private bool _isPlayingAudio;
    [ObservableProperty] private string? _currentlySpeakingText;

    public ObservableCollection<string> AvailableOllamaModels { get; } = [];

    public MainViewModel(
        DocumentsViewModel documentsViewModel,
        ChatViewModel chatViewModel,
        ArtifactsViewModel artifactsViewModel,
        GraphViewModel graphViewModel,
        IAiServiceFactory aiServiceFactory,
        SessionContext sessionContext)
    {
        Documents = documentsViewModel;
        Chat = chatViewModel;
        Artifacts = artifactsViewModel;
        Graph = graphViewModel;
        _aiServiceFactory = aiServiceFactory;
        _sessionContext = sessionContext;

        LoadSettings();
        WireUpEvents();
        _ = CheckOllamaAvailabilityAsync();
    }

    public SessionContext SessionContext => _sessionContext;

    private void WireUpEvents()
    {
        Documents.StatusChanged += (_, e) => StatusMessage = e.Message;
        Documents.ErrorOccurred += (_, e) => ShowError(e.Message);
        Documents.PropertyChanged += (_, e) =>
        {
            if (string.Equals(e.PropertyName, nameof(Documents.IsBusy), StringComparison.Ordinal))
                IsBusy = Documents.IsBusy;
        };

        Chat.StatusChanged += (_, e) => StatusMessage = e.Message;
        Chat.GetIncludedSources = () => Documents.GetIncludedSources();
        Chat.GetApiKey = () => GeminiApiKey;
        Chat.GetAiProvider = () => SelectedAiProvider;
        Chat.PropertyChanged += (_, e) =>
        {
            if (string.Equals(e.PropertyName, nameof(Chat.IsBusy), StringComparison.Ordinal))
            {
                IsBusy = Chat.IsBusy;
                IsThinking = Chat.IsThinking;
            }
        };

        Artifacts.StatusChanged += (_, e) => StatusMessage = e.Message;
        Artifacts.GetIncludedSources = () => Documents.GetIncludedSources();
        Artifacts.GetApiKey = () => GeminiApiKey;
        Artifacts.PropertyChanged += (_, e) =>
        {
            if (string.Equals(e.PropertyName, nameof(Artifacts.IsBusy), StringComparison.Ordinal))
            {
                IsBusy = Artifacts.IsBusy;
                IsThinking = Artifacts.IsThinking;
            }
        };

        Graph.StatusChanged += (_, e) => StatusMessage = e.Message;
        Graph.ErrorOccurred += (_, e) => ShowError(e.Message);
        Graph.GetIncludedSources = () => Documents.GetIncludedSources();
        Graph.PropertyChanged += (_, e) =>
        {
            if (string.Equals(e.PropertyName, nameof(Graph.IsBusy), StringComparison.Ordinal))
                IsBusy = Graph.IsBusy;
        };
    }

    #region Facade Properties (for XAML binding compatibility)

    public ObservableCollection<SourceDocumentViewModel> Sources => Documents.Sources;
    public ObservableCollection<ChatMessageViewModel> ChatMessages => Chat.Messages;
    public ObservableCollection<Artifact> ArtifactsList => Artifacts.Artifacts;
    public ObservableCollection<KnowledgeGraphService.GraphNode> GraphNodes => Graph.Nodes;
    public ObservableCollection<KnowledgeGraphService.GraphEdge> GraphEdges => Graph.Edges;

    public string UserQuestion
    {
        get => Chat.UserQuestion;
        set => Chat.UserQuestion = value;
    }

    public string[] FollowUpQuestions
    {
        get => Chat.FollowUpQuestions;
        set => Chat.FollowUpQuestions = value;
    }

    public string[]? PendingImages
    {
        get => Chat.PendingImages;
        set => Chat.PendingImages = value;
    }

    public Artifact? SelectedArtifact
    {
        get => Artifacts.SelectedArtifact;
        set => Artifacts.SelectedArtifact = value;
    }

    public string CurrentMermaidDiagram
    {
        get => Graph.CurrentMermaidDiagram;
        set => Graph.CurrentMermaidDiagram = value;
    }

    public string SelectedDiagramType
    {
        get => Graph.SelectedDiagramType;
        set => Graph.SelectedDiagramType = value;
    }

    public string ObsidianVaultPath
    {
        get => Documents.ObsidianVaultPath;
        set => Documents.ObsidianVaultPath = value;
    }

    public string ObsidianSubfolder
    {
        get => Documents.ObsidianSubfolder;
        set => Documents.ObsidianSubfolder = value;
    }

    #endregion

    #region Delegated Commands

    [RelayCommand]
    private Task AddDocumentAsync() => Documents.AddDocumentCommand.ExecuteAsync(null);

    [RelayCommand]
    private Task LoadObsidianVaultAsync() => Documents.LoadObsidianVaultCommand.ExecuteAsync(null);

    [RelayCommand]
    private void ToggleSourceInclusion(SourceDocumentViewModel source) =>
        Documents.ToggleSourceInclusionCommand.Execute(source);

    [RelayCommand]
    private void RemoveSource(SourceDocumentViewModel source) =>
        Documents.RemoveSourceCommand.Execute(source);

    [RelayCommand]
    private void ClearSources() => Documents.ClearSourcesCommand.Execute(null);

    [RelayCommand]
    private void BrowseVaultPath() => Documents.BrowseVaultPathCommand.Execute(null);

    [RelayCommand]
    private Task AskQuestionAsync() => Chat.AskQuestionCommand.ExecuteAsync(null);

    [RelayCommand]
    private void ClearChat() => Chat.ClearChatCommand.Execute(null);

    [RelayCommand]
    private void UseFollowUpQuestion(string question) =>
        Chat.UseFollowUpQuestionCommand.Execute(question);

    [RelayCommand]
    private Task GenerateFAQAsync() => Artifacts.GenerateFAQCommand.ExecuteAsync(null);

    [RelayCommand]
    private Task GenerateStudyGuideAsync() => Artifacts.GenerateStudyGuideCommand.ExecuteAsync(null);

    [RelayCommand]
    private Task GeneratePodcastScriptAsync() => Artifacts.GeneratePodcastScriptCommand.ExecuteAsync(null);

    [RelayCommand]
    private Task GenerateNotebookGuideAsync() => Artifacts.GenerateNotebookGuideCommand.ExecuteAsync(null);

    [RelayCommand]
    private Task GenerateSummaryAsync() => Artifacts.GenerateSummaryCommand.ExecuteAsync(null);

    [RelayCommand]
    private Task GenerateOutlineAsync() => Artifacts.GenerateOutlineCommand.ExecuteAsync(null);

    [RelayCommand]
    private void RefreshGraph() => Graph.RefreshGraphCommand.Execute(null);

    [RelayCommand]
    private Task GenerateDiagramAsync() => Graph.GenerateDiagramCommand.ExecuteAsync(null);

    #endregion

    #region App-Level Commands

    [RelayCommand]
    private void NavigateTo(AppView view) => SelectedView = view;

    [RelayCommand]
    private void SwitchAppMode()
    {
        _sessionContext.SwitchMode();
        StatusMessage = $"🔄 Switched to {_sessionContext.ModeDisplayName} mode";
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        IsDarkTheme = !IsDarkTheme;
        ApplyTheme(IsDarkTheme);
    }

    [RelayCommand]
    private async Task LoadOllamaModelsAsync()
    {
        var result = await _aiServiceFactory.GetAvailableModelsAsync(AiProvider.Ollama).ConfigureAwait(true);
        
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

    #endregion

    private async Task CheckOllamaAvailabilityAsync()
    {
        IsOllamaAvailable = await _aiServiceFactory.IsServiceAvailableAsync(AiProvider.Ollama).ConfigureAwait(true);

        if (IsOllamaAvailable)
        {
            await LoadOllamaModelsAsync().ConfigureAwait(true);
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

    partial void OnGeminiApiKeyChanged(string value)
    {
        if (NexusAI.Presentation.Properties.Settings.Default.GeminiApiKey != value)
        {
            NexusAI.Presentation.Properties.Settings.Default.GeminiApiKey = value;
            NexusAI.Presentation.Properties.Settings.Default.Save();
        }
    }

    private void LoadSettings()
    {
        var savedApiKey = NexusAI.Presentation.Properties.Settings.Default.GeminiApiKey;
        if (!string.IsNullOrEmpty(savedApiKey))
            GeminiApiKey = savedApiKey;
    }

    private void ShowError(string message)
    {
        StatusMessage = $"❌ {message}";
        ErrorMessage = message;
        IsErrorVisible = true;

        _ = Task.Delay(8000).ContinueWith(_ =>
        {
            IsErrorVisible = false;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private static void ApplyTheme(bool isDark)
    {
        var app = System.Windows.Application.Current;
        if (app?.Resources.MergedDictionaries.FirstOrDefault(
            d => d is MaterialDesignThemes.Wpf.BundledTheme) is MaterialDesignThemes.Wpf.BundledTheme bundledTheme)
        {
            bundledTheme.BaseTheme = isDark 
                ? MaterialDesignThemes.Wpf.BaseTheme.Dark
                : MaterialDesignThemes.Wpf.BaseTheme.Light;
        }
    }
}

