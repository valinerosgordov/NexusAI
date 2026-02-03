using CommunityToolkit.Mvvm.ComponentModel;
using NexusAI.Domain.Models;
using System.Collections.ObjectModel;

namespace NexusAI.Presentation.ViewModels;

public sealed partial class ChatMessageViewModel : ObservableObject
{
    public ChatMessage Message { get; }

    [ObservableProperty] private string _contentWithoutSteps = string.Empty;

    public string Content => Message.Content;
    public string Role => Message.Role;
    public string TimeDisplay => Message.Timestamp.ToLocalTime().ToString("HH:mm:ss");
    public bool IsUser => Role == MessageRole.User;
    public bool IsAssistant => Role == MessageRole.Assistant;
    public bool HasThinkingSteps => ThinkingSteps.Count > 0;

    public string? Citations => Message.SourceCitations is { Length: > 0 }
        ? $"Sources: {string.Join(", ", Message.SourceCitations)}"
        : null;

    public ObservableCollection<string> ThinkingSteps { get; } = [];

    public ChatMessageViewModel(ChatMessage message)
    {
        Message = message;
        ContentWithoutSteps = message.Content;
        
        // Parse thinking steps if present
        ParseThinkingSteps(message.Content);
    }

    private void ParseThinkingSteps(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return;

        var lines = content.Split('\n');
        List<string> cleanedLines = [];

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("[STEP]", StringComparison.OrdinalIgnoreCase))
            {
                var stepContent = trimmed[6..].Trim();
                if (!string.IsNullOrWhiteSpace(stepContent))
                {
                    ThinkingSteps.Add(stepContent);
                }
            }
            else
            {
                cleanedLines.Add(line);
            }
        }

        ContentWithoutSteps = string.Join('\n', cleanedLines).Trim();
    }

    public void AddThinkingStep(string step)
    {
        if (!string.IsNullOrWhiteSpace(step))
        {
            ThinkingSteps.Add(step);
            OnPropertyChanged(nameof(HasThinkingSteps));
        }
    }

    public void UpdateContent(string content)
    {
        ContentWithoutSteps = content;
    }
}
