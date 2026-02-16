namespace NexusAI.Presentation.ViewModels;

public sealed class MessageEventArgs(string message) : EventArgs
{
    public string Message { get; } = message;
}
