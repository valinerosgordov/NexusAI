using CommunityToolkit.Mvvm.ComponentModel;
using NexusAI.Domain.Models;

namespace NexusAI.Presentation.ViewModels;

public sealed partial class ProjectTaskViewModel : ObservableObject
{
    public ProjectTaskId Id { get; }

    [ObservableProperty] private string _title;
    [ObservableProperty] private string? _role;
    [ObservableProperty] private decimal _hours;
    [ObservableProperty] private Domain.Models.TaskStatus _status;
    [ObservableProperty] private TaskPriority _priority;
    [ObservableProperty] private string? _assignee;
    [ObservableProperty] private Guid? _sourceDocumentId;

    public string Initials => GetInitials(Assignee);
    public string PriorityColor => Priority switch
    {
        TaskPriority.High => "#FF3B30",
        TaskPriority.Medium => "#00D9FF",
        TaskPriority.Low => "#34C759",
        _ => "#98989D"
    };
    public string RoleBadgeColor => Role?.ToLowerInvariant() switch
    {
        "backend" or "frontend" => "#007AFF",
        "design" or "ui/ux" => "#AF52DE",
        "testing" or "qa" => "#00D9FF",
        "devops" or "infrastructure" => "#5856D6",
        "marketing" or "content" => "#FF2D55",
        _ => "#8E8E93"
    };

    public ProjectTaskViewModel(ProjectTask task)
    {
        Id = task.Id;
        _title = task.Title;
        _role = task.Role;
        _hours = task.Hours;
        _status = task.Status;
        _priority = task.Priority;
        _assignee = task.Assignee;
        _sourceDocumentId = task.SourceDocumentId;
    }

    public void UpdateStatus(Domain.Models.TaskStatus newStatus)
    {
        Status = newStatus;
    }

    private static string GetInitials(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "AI";

        var parts = name.Split([' ', '.'], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{parts[0][0]}{parts[1][0]}".ToUpperInvariant();

        return parts.Length == 1 && parts[0].Length >= 2
            ? parts[0][..2].ToUpperInvariant()
            : parts[0][0].ToString().ToUpperInvariant();
    }
}
