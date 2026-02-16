using CommunityToolkit.Mvvm.ComponentModel;

namespace NexusAI.Presentation.ViewModels;

public sealed partial class TaskRoleDistribution : ObservableObject
{
    [ObservableProperty] private string _role = string.Empty;
    [ObservableProperty] private int _count;
    [ObservableProperty] private string _color = "#8E8E93";

    public double Percentage { get; set; }
}
