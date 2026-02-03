using CommunityToolkit.Mvvm.ComponentModel;
using NexusAI.Domain.Models;

namespace NexusAI.Presentation.ViewModels;

public sealed partial class ChatMessageViewModel : ObservableObject
{
    public ChatMessage Message { get; }

    public string Content => Message.Content;
    public MessageRole Role => Message.Role;
    public string TimeDisplay => Message.Timestamp.ToLocalTime().ToString("HH:mm:ss");
    public bool IsUser => Role == MessageRole.User;
    public bool IsAssistant => Role == MessageRole.Assistant;

    public string? Citations => Message.SourceCitations is { Length: > 0 }
        ? $"Sources: {string.Join(", ", Message.SourceCitations)}"
        : null;

    public ChatMessageViewModel(ChatMessage message)
    {
        Message = message;
    }
}
