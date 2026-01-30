using CommunityToolkit.Mvvm.ComponentModel;
using NexusAI.Domain.Models;

namespace NexusAI.Presentation.ViewModels;

public sealed partial class SourceDocumentViewModel : ObservableObject
{
    public SourceDocument Document { get; }

    [ObservableProperty]
    private bool _isIncluded;

    [ObservableProperty]
    private bool _isHighlighted;

    public SourceDocumentId Id => Document.Id;
    public string Name => Document.Name;
    public string TypeDisplay => Document.Type.ToString();
    public string LoadedAtDisplay => Document.LoadedAt.ToLocalTime().ToString("g");
    public string TypeIcon => Document.Type == SourceType.Pdf ? "📄" : "📝";

    public SourceDocumentViewModel(SourceDocument document)
    {
        Document = document;
        _isIncluded = document.IsIncluded;
        _isHighlighted = false;
    }

    public void Highlight()
    {
        IsHighlighted = true;
    }

    public void ClearHighlight()
    {
        IsHighlighted = false;
    }
}
