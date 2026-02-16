using CommunityToolkit.Mvvm.ComponentModel;

namespace NexusAI.Presentation.ViewModels;

public sealed partial class SlidePreview : ObservableObject
{
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _preview = string.Empty;
    [ObservableProperty] private int _number;
}
