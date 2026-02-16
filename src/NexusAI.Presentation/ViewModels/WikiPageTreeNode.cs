using CommunityToolkit.Mvvm.ComponentModel;
using NexusAI.Domain.Models;
using System.Collections.ObjectModel;

namespace NexusAI.Presentation.ViewModels;

public sealed partial class WikiPageTreeNode : ObservableObject
{
    [ObservableProperty] private WikiPage _page = null!;

    public ObservableCollection<WikiPageTreeNode> Children { get; } = [];
}
