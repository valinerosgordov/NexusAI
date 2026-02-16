using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NexusAI.Application.Services;
using NexusAI.Application.UseCases.Projects;
using NexusAI.Application.UseCases.Scaffold;
using NexusAI.Domain.Models;
using System.Collections.ObjectModel;

namespace NexusAI.Presentation.ViewModels;

public sealed partial class ProjectViewModel(
    GenerateProjectPlanHandler generatePlanHandler,
    GetUserProjectsHandler getProjectsHandler,
    UpdateTaskStatusHandler updateTaskStatusHandler,
    GenerateScaffoldHandler generateScaffoldHandler,
    SessionContext sessionContext) : ObservableObject
{
    private readonly GenerateProjectPlanHandler _generatePlanHandler = generatePlanHandler;
    private readonly GetUserProjectsHandler _getProjectsHandler = getProjectsHandler;
    private readonly UpdateTaskStatusHandler _updateTaskStatusHandler = updateTaskStatusHandler;
    private readonly GenerateScaffoldHandler _generateScaffoldHandler = generateScaffoldHandler;
    private readonly SessionContext _sessionContext = sessionContext;
    private List<Project> _allProjects = [];

    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _statusMessage = "Ready";
    [ObservableProperty] private Project? _selectedProject;
    [ObservableProperty] private double _completionPercentage;
    [ObservableProperty] private int _totalTasks;
    [ObservableProperty] private int _completedTasks;
    [ObservableProperty] private bool _filterByMode = true;

    public ObservableCollection<Project> Projects { get; } = [];
    public ObservableCollection<ProjectTaskViewModel> TodoTasks { get; } = [];
    public ObservableCollection<ProjectTaskViewModel> InProgressTasks { get; } = [];
    public ObservableCollection<ProjectTaskViewModel> DoneTasks { get; } = [];
    public ObservableCollection<TaskRoleDistribution> RoleDistribution { get; } = [];

    private UserId CurrentUserId { get; } = new(Guid.NewGuid());

    public void Initialize()
    {
        _sessionContext.PropertyChanged += (s, e) =>
        {
            if (string.Equals(e.PropertyName, nameof(SessionContext.CurrentMode), StringComparison.Ordinal))
            {
                ApplyProjectFilter();
            }
        };

        _ = LoadProjectsAsync();
    }

    [RelayCommand]
    private async Task GenerateProjectFromIdeaAsync(string idea)
    {
        if (string.IsNullOrWhiteSpace(idea))
        {
            StatusMessage = "❌ Project idea cannot be empty";
            return;
        }

        IsBusy = true;
        StatusMessage = "🤖 AI is generating project plan...";

        try
        {
            var title = ExtractTitleFromIdea(idea);
            var command = new GenerateProjectPlanCommand(idea, title, CurrentUserId);
            
            var result = await _generatePlanHandler.HandleAsync(command).ConfigureAwait(true);

            if (result.IsSuccess)
            {
                var (project, tasks) = result.Value;
                Projects.Add(project);
                SelectedProject = project;
                LoadTasksIntoKanban(tasks);
                StatusMessage = $"✅ Generated project: {project.Title} ({tasks.Length} tasks)";
            }
            else
            {
                StatusMessage = $"❌ {result.Error}";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadProjectsAsync()
    {
        IsBusy = true;
        StatusMessage = "Loading projects...";

        try
        {
            var query = new GetUserProjectsQuery(CurrentUserId);
            var result = await _getProjectsHandler.HandleAsync(query).ConfigureAwait(true);

            if (result.IsSuccess)
            {
                _allProjects = result.Value.ToList();
                ApplyProjectFilter();
                StatusMessage = $"Loaded {Projects.Count} project(s)";
            }
            else
            {
                StatusMessage = $"❌ {result.Error}";
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplyProjectFilter()
    {
        Projects.Clear();

        var filtered = FilterByMode
            ? _allProjects.Where(p => IsProjectRelevantForCurrentMode(p))
            : _allProjects;

        foreach (var project in filtered)
        {
            Projects.Add(project);
        }
    }

    private bool IsProjectRelevantForCurrentMode(Project project)
    {
        return _sessionContext.CurrentMode switch
        {
            AppMode.Professional => project.Category == ProjectCategory.Work || 
                                   project.Category == ProjectCategory.Personal,
            AppMode.Student => project.Category == ProjectCategory.Education || 
                              project.Category == ProjectCategory.Personal,
            _ => true
        };
    }

    [RelayCommand]
    private void ToggleFilter()
    {
        FilterByMode = !FilterByMode;
        ApplyProjectFilter();
    }

    [RelayCommand]
    private async Task MoveTaskAsync((ProjectTaskViewModel task, Domain.Models.TaskStatus newStatus) parameters)
    {
        var (task, newStatus) = parameters;

        var command = new UpdateTaskStatusCommand(task.Id, newStatus);
        var result = await _updateTaskStatusHandler.HandleAsync(command).ConfigureAwait(true);

        if (result.IsSuccess)
        {
            // Remove from old column
            TodoTasks.Remove(task);
            InProgressTasks.Remove(task);
            DoneTasks.Remove(task);

            // Add to new column
            task.UpdateStatus(newStatus);
            var targetCollection = newStatus switch
            {
                Domain.Models.TaskStatus.Pending => TodoTasks,
                Domain.Models.TaskStatus.InProgress => InProgressTasks,
                Domain.Models.TaskStatus.Completed => DoneTasks,
                _ => TodoTasks
            };
            targetCollection.Add(task);

            StatusMessage = $"✅ Task moved to {newStatus}";
        }
        else
        {
            StatusMessage = $"❌ {result.Error}";
        }
    }

    partial void OnSelectedProjectChanged(Project? value)
    {
        if (value is not null)
        {
            _ = LoadProjectTasksAsync();
        }
    }

    private async Task LoadProjectTasksAsync()
    {
        IsBusy = true;

        try
        {
            await Task.CompletedTask.ConfigureAwait(true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void LoadTasksIntoKanban(ProjectTask[] tasks)
    {
        TodoTasks.Clear();
        InProgressTasks.Clear();
        DoneTasks.Clear();

        foreach (var task in tasks)
        {
            var vm = new ProjectTaskViewModel(task);
            var targetCollection = task.Status switch
            {
                Domain.Models.TaskStatus.Pending => TodoTasks,
                Domain.Models.TaskStatus.InProgress => InProgressTasks,
                Domain.Models.TaskStatus.Completed or Domain.Models.TaskStatus.Cancelled => DoneTasks,
                _ => TodoTasks
            };
            targetCollection.Add(vm);
        }

        UpdateAnalytics();
    }

    private void UpdateAnalytics()
    {
        var allTasks = TodoTasks.Concat(InProgressTasks).Concat(DoneTasks).ToList();
        
        TotalTasks = allTasks.Count;
        CompletedTasks = DoneTasks.Count;
        CompletionPercentage = TotalTasks > 0 ? (double)CompletedTasks / TotalTasks * 100 : 0;

        // Calculate role distribution
        RoleDistribution.Clear();
        var roleGroups = allTasks
            .Where(t => !string.IsNullOrWhiteSpace(t.Role))
            .GroupBy(t => t.Role!, StringComparer.Ordinal)
            .Select(g => new TaskRoleDistribution
            {
                Role = g.Key,
                Count = g.Count(),
                Color = g.First().RoleBadgeColor
            })
            .OrderByDescending(r => r.Count);

        foreach (var role in roleGroups)
        {
            RoleDistribution.Add(role);
        }
    }

    [RelayCommand]
    private async Task<ScaffoldResult?> GenerateScaffoldAsync(
        (string Description, string[] Technologies, string TargetPath) parameters)
    {
        var (description, technologies, targetPath) = parameters;

        IsBusy = true;
        StatusMessage = "🚀 AI is generating project structure...";

        try
        {
            var command = new GenerateScaffoldCommand(description, technologies, targetPath);
            var result = await _generateScaffoldHandler.HandleAsync(command).ConfigureAwait(true);

            if (result.IsSuccess)
            {
                StatusMessage = $"✅ Scaffolding complete! Created {result.Value.CreatedFiles} files";
                return result.Value;
            }
            else
            {
                StatusMessage = $"❌ {result.Error}";
                return null;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Error: {ex.Message}";
            return null;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static string ExtractTitleFromIdea(string idea)
    {
        // Simple heuristic: take first sentence or first 50 chars
        var firstLine = idea.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? idea;
        return firstLine.Length > 50 ? firstLine[..47] + "..." : firstLine;
    }
}
