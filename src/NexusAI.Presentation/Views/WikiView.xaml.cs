using NexusAI.Presentation.ViewModels;

namespace NexusAI.Presentation.Views;

public partial class WikiView : System.Windows.Controls.UserControl
{
    public WikiView()
    {
        InitializeComponent();
    }

    private WikiViewModel? ViewModel => DataContext as WikiViewModel;

    private void WikiTreeView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
    {
        if (e.NewValue is WikiPageTreeNode node && ViewModel is not null)
        {
            ViewModel.SelectedNode = node;
        }
    }

    private void GenerateButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var dialog = new WikiGeneratorDialog
        {
            Owner = System.Windows.Window.GetWindow(this)
        };

        if (dialog.ShowDialog() == true)
        {
            var sources = ViewModel?.GetSourceDocuments() ?? [];
            _ = ViewModel?.GenerateKnowledgeBaseCommand.ExecuteAsync((dialog.Topic, sources));
        }
    }
}
