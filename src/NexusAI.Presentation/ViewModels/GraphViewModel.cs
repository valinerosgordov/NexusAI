using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NexusAI.Application.Services;
using NexusAI.Application.UseCases.Diagrams;
using NexusAI.Domain.Models;
using System.Collections.ObjectModel;

namespace NexusAI.Presentation.ViewModels;

public sealed partial class GraphViewModel(
    KnowledgeGraphService graphService,
    GenerateDiagramHandler generateDiagramHandler) : ObservableObject
{
    private readonly KnowledgeGraphService _graphService = graphService;
    private readonly GenerateDiagramHandler _generateDiagramHandler = generateDiagramHandler;

    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _currentMermaidDiagram = string.Empty;
    [ObservableProperty] private string _selectedDiagramType = "architecture";

    public ObservableCollection<KnowledgeGraphService.GraphNode> Nodes { get; } = [];
    public ObservableCollection<KnowledgeGraphService.GraphEdge> Edges { get; } = [];

    public event EventHandler<string>? StatusChanged;
    public event EventHandler<string>? ErrorOccurred;
    public Func<SourceDocument[]>? GetIncludedSources { get; set; }

    [RelayCommand]
    private void RefreshGraph()
    {
        var documents = GetIncludedSources?.Invoke() ?? [];

        var (nodes, edges) = _graphService.BuildGraph(documents);

        Nodes.Clear();
        Edges.Clear();

        foreach (var node in nodes)
            Nodes.Add(node);

        foreach (var edge in edges)
            Edges.Add(edge);

        OnStatusChanged($"Graph: {nodes.Length} nodes, {edges.Length} connections");
    }

    [RelayCommand]
    private async Task GenerateDiagramAsync()
    {
        IsBusy = true;
        OnStatusChanged("🎨 AI is generating diagram...");

        try
        {
            var projectContext = BuildProjectContext();
            var command = new GenerateDiagramCommand(projectContext, SelectedDiagramType);
            var result = await _generateDiagramHandler.HandleAsync(command);

            if (result.IsSuccess)
            {
                CurrentMermaidDiagram = result.Value;
                OnStatusChanged($"✅ {SelectedDiagramType} diagram generated");
            }
            else
            {
                OnErrorOccurred($"Failed to generate diagram: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            OnErrorOccurred($"Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private string BuildProjectContext()
    {
        var includedSources = GetIncludedSources?.Invoke() ?? [];
        
        if (includedSources.Length > 0)
        {
            var docNames = string.Join(", ", includedSources.Select(s => s.Name));
            return $"Project: NexusAI (Clean Architecture WPF app with AI capabilities). Documents: {docNames}";
        }

        return "Project: NexusAI - A Clean Architecture WPF application with AI-powered features including document analysis, project planning, code scaffolding, and knowledge graphs. Built with .NET 8, MaterialDesign, EF Core, and Gemini AI.";
    }

    private void OnStatusChanged(string message) => StatusChanged?.Invoke(this, message);
    private void OnErrorOccurred(string message) => ErrorOccurred?.Invoke(this, message);
}
