using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NexusAI.Application.UseCases.Wiki;
using NexusAI.Domain.Models;
using System.Collections.ObjectModel;

namespace NexusAI.Presentation.ViewModels;

public sealed partial class WikiViewModel : ObservableObject
{
    private readonly GenerateWikiHandler _generateWikiHandler;
    private readonly GetWikiTreeHandler _getWikiTreeHandler;
    private readonly UpdateWikiPageHandler _updateWikiPageHandler;
    private readonly DeleteWikiPageHandler _deleteWikiPageHandler;
    private readonly Func<SourceDocument[]> _getSourceDocumentsFunc;

    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _statusMessage = "Ready";
    [ObservableProperty] private WikiPageTreeNode? _selectedNode;
    [ObservableProperty] private string _currentPageContent = string.Empty;
    [ObservableProperty] private string _currentPageTitle = string.Empty;

    public ObservableCollection<WikiPageTreeNode> RootNodes { get; } = [];

    public WikiViewModel(
        GenerateWikiHandler generateWikiHandler,
        GetWikiTreeHandler getWikiTreeHandler,
        UpdateWikiPageHandler updateWikiPageHandler,
        DeleteWikiPageHandler deleteWikiPageHandler,
        Func<SourceDocument[]> getSourceDocumentsFunc)
    {
        _generateWikiHandler = generateWikiHandler;
        _getWikiTreeHandler = getWikiTreeHandler;
        _updateWikiPageHandler = updateWikiPageHandler;
        _deleteWikiPageHandler = deleteWikiPageHandler;
        _getSourceDocumentsFunc = getSourceDocumentsFunc;

        _ = LoadWikiTreeAsync();
    }

    public SourceDocument[] GetSourceDocuments() => _getSourceDocumentsFunc();

    [RelayCommand]
    private async Task GenerateKnowledgeBaseAsync((string Topic, SourceDocument[] Sources) parameters)
    {
        var (topic, sources) = parameters;

        IsBusy = true;
        StatusMessage = $"🤖 AI is generating wiki for: {topic}...";

        try
        {
            var command = new GenerateWikiCommand(topic, sources);
            var result = await _generateWikiHandler.HandleAsync(command);

            if (result.IsSuccess)
            {
                RootNodes.Clear();
                foreach (var node in result.Value)
                {
                    RootNodes.Add(ConvertToTreeNode(node));
                }

                StatusMessage = $"✅ Wiki generated with {result.Value.Length} root pages";
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
    private async Task LoadWikiTreeAsync()
    {
        IsBusy = true;
        StatusMessage = "Loading wiki...";

        try
        {
            var query = new GetWikiTreeQuery();
            var result = await _getWikiTreeHandler.HandleAsync(query);

            if (result.IsSuccess)
            {
                RootNodes.Clear();
                foreach (var node in result.Value)
                {
                    RootNodes.Add(ConvertToTreeNode(node));
                }

                StatusMessage = result.Value.Length > 0
                    ? $"Loaded {result.Value.Length} root pages"
                    : "No wiki pages yet. Generate knowledge base to begin.";
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
    private async Task SaveCurrentPageAsync()
    {
        if (SelectedNode is null)
            return;

        IsBusy = true;
        StatusMessage = "Saving changes...";

        try
        {
            var command = new UpdateWikiPageCommand(
                SelectedNode.Page.Id,
                CurrentPageTitle,
                CurrentPageContent,
                SelectedNode.Page.Tags);

            var result = await _updateWikiPageHandler.HandleAsync(command);

            if (result.IsSuccess)
            {
                SelectedNode.Page = result.Value;
                StatusMessage = "✅ Page saved";
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
    private async Task DeleteCurrentPageAsync()
    {
        if (SelectedNode is null)
            return;

        IsBusy = true;
        StatusMessage = "Deleting page...";

        try
        {
            var command = new DeleteWikiPageCommand(SelectedNode.Page.Id);
            var result = await _deleteWikiPageHandler.HandleAsync(command);

            if (result.IsSuccess)
            {
                await LoadWikiTreeAsync();
                SelectedNode = null;
                StatusMessage = "✅ Page deleted";
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

    partial void OnSelectedNodeChanged(WikiPageTreeNode? value)
    {
        if (value is not null)
        {
            CurrentPageTitle = value.Page.Title;
            CurrentPageContent = value.Page.Content;
        }
        else
        {
            CurrentPageTitle = string.Empty;
            CurrentPageContent = string.Empty;
        }
    }

    private static WikiPageTreeNode ConvertToTreeNode(WikiPageNode node)
    {
        var treeNode = new WikiPageTreeNode
        {
            Page = node.Page
        };

        foreach (var child in node.Children)
        {
            treeNode.Children.Add(ConvertToTreeNode(child));
        }

        return treeNode;
    }
}

public sealed partial class WikiPageTreeNode : ObservableObject
{
    [ObservableProperty] private WikiPage _page = null!;

    public ObservableCollection<WikiPageTreeNode> Children { get; } = [];
}
