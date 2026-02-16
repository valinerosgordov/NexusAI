using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NexusAI.Application.UseCases.Chat;
using NexusAI.Domain.Models;
using System.Collections.ObjectModel;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace NexusAI.Presentation.ViewModels;

public sealed partial class ChatViewModel(
    AskQuestionHandler askQuestionHandler,
    GenerateFollowUpQuestionsHandler generateFollowUpQuestionsHandler) : ObservableObject
{
    private readonly AskQuestionHandler _askQuestionHandler = askQuestionHandler;
    private readonly GenerateFollowUpQuestionsHandler _generateFollowUpQuestionsHandler = generateFollowUpQuestionsHandler;

    [ObservableProperty] private string _userQuestion = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isThinking;
    [ObservableProperty] private string[] _followUpQuestions = [];

    public bool HasFollowUpQuestions => FollowUpQuestions.Length > 0;

    public string[]? PendingImages { get; set; }
    public ObservableCollection<ChatMessageViewModel> Messages { get; } = [];

    public event EventHandler<MessageEventArgs>? StatusChanged;
    public Func<SourceDocument[]>? GetIncludedSources { get; set; }
    public Func<string>? GetApiKey { get; set; }
    public Func<AiProvider>? GetAiProvider { get; set; }

    [RelayCommand(CanExecute = nameof(CanAskQuestion))]
    private async Task AskQuestionAsync()
    {
        var apiKey = GetApiKey?.Invoke() ?? string.Empty;
        var provider = GetAiProvider?.Invoke() ?? AiProvider.Gemini;

        if (string.IsNullOrWhiteSpace(apiKey) && provider == AiProvider.Gemini)
        {
            MessageBox.Show("Please enter your Gemini API key in settings", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var includedSources = GetIncludedSources?.Invoke() ?? [];
        
        IsBusy = true;
        IsThinking = true;
        OnStatusChanged(PendingImages != null ? $"Analyzing {PendingImages.Length} image(s)..." : "Thinking...");

        try
        {
            var userMessage = new ChatMessage(
                Id: ChatMessageId.NewId(),
                Content: UserQuestion,
                Role: MessageRole.User,
                Timestamp: DateTime.UtcNow
            );

            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Messages.Add(new ChatMessageViewModel(userMessage));
            }).Task.ConfigureAwait(true);

            var command = new AskQuestionCommand(UserQuestion, includedSources, PendingImages);
            var result = await _askQuestionHandler.HandleAsync(command).ConfigureAwait(true);

            PendingImages = null;

            if (result.IsSuccess)
            {
                var (message, _, _) = result.Value;
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Messages.Add(new ChatMessageViewModel(message));
                }).Task.ConfigureAwait(true);

                await GenerateFollowUpQuestionsAsync(UserQuestion, message.Content).ConfigureAwait(true);
                UserQuestion = string.Empty;
                OnStatusChanged("Response received");
            }
            else
            {
                MessageBox.Show(result.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                OnStatusChanged("Failed to get response");
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
    private void ClearChat()
    {
        if (MessageBox.Show("Clear chat history?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        Messages.Clear();
        OnStatusChanged("Chat cleared");
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

    private async Task GenerateFollowUpQuestionsAsync(string question, string answer)
    {
        try
        {
            var command = new GenerateFollowUpQuestionsCommand(question, answer);
            var result = await _generateFollowUpQuestionsHandler.HandleAsync(command).ConfigureAwait(true);
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

    partial void OnUserQuestionChanged(string value) => AskQuestionCommand.NotifyCanExecuteChanged();
    partial void OnIsBusyChanged(bool value) => AskQuestionCommand.NotifyCanExecuteChanged();
    partial void OnFollowUpQuestionsChanged(string[] value) => OnPropertyChanged(nameof(HasFollowUpQuestions));

    private void OnStatusChanged(string message) => StatusChanged?.Invoke(this, new MessageEventArgs(message));
}
