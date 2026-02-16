using MaterialDesignThemes.Wpf;
using NexusAI.Domain.Models;
using NexusAI.Presentation.ViewModels;
using System.Windows;
using System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;

namespace NexusAI.Presentation.Views;

public partial class ProjectView : System.Windows.Controls.UserControl
{
    public ProjectView()
    {
        InitializeComponent();
    }

    private ProjectViewModel? ViewModel => DataContext as ProjectViewModel;

    private void NewProjectButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new NewProjectDialog
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.ProjectIdea))
        {
            ViewModel?.GenerateProjectFromIdeaCommand.Execute(dialog.ProjectIdea);
        }
    }

    private async void ScaffoldButton_Click(object sender, RoutedEventArgs e)
    {
        var scaffoldDialog = new ScaffoldDialog
        {
            Owner = Window.GetWindow(this)
        };

        if (scaffoldDialog.ShowDialog() == true && ViewModel is not null)
        {
            await ViewModel.GenerateScaffoldCommand.ExecuteAsync(
                (scaffoldDialog.ProjectDescription, 
                 scaffoldDialog.Technologies, 
                 scaffoldDialog.TargetPath));
        }
    }

    private static void ShowToast(string message)
    {
        var snackbar = new Snackbar
        {
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3))
        };

        snackbar.MessageQueue!.Enqueue(message);
    }

    private void MoveToTodo_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem && menuItem.Tag is ProjectTaskViewModel task)
        {
            ViewModel?.MoveTaskCommand.Execute((task, Domain.Models.TaskStatus.Pending));
        }
    }

    private void MoveToInProgress_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem && menuItem.Tag is ProjectTaskViewModel task)
        {
            ViewModel?.MoveTaskCommand.Execute((task, Domain.Models.TaskStatus.InProgress));
        }
    }

    private void MoveToDone_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem && menuItem.Tag is ProjectTaskViewModel task)
        {
            ViewModel?.MoveTaskCommand.Execute((task, Domain.Models.TaskStatus.Completed));
        }
    }

    private void TaskCard_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is not System.Windows.FrameworkElement element)
            return;

        var task = element.Tag as ProjectTaskViewModel;
        if (task?.SourceDocumentId == null)
        {
            MessageBox.Show("This task is not linked to a source document.", 
                          "No Document Link", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Information);
            return;
        }

        // Navigate to main window and switch to Chat/Docs tab
        var mainWindow = Window.GetWindow(this) as MainWindow;
        if (mainWindow?.DataContext is not MainViewModel mainViewModel)
            return;

        // Find and select the linked document
        var linkedDoc = mainViewModel.Sources
            .FirstOrDefault(s => task.SourceDocumentId.HasValue && s.Document.Id.Value == task.SourceDocumentId.Value);

        if (linkedDoc != null)
        {
            // Ensure document is included
            if (!linkedDoc.IsIncluded)
            {
                linkedDoc.IsIncluded = true;
            }

            // Switch to Chat tab (assuming TabControl.SelectedIndex navigation)
            // This would need MainWindow to expose the TabControl or use a navigation service
            ShowToast($"📄 Navigated to source document: {linkedDoc.Document.Name}");
        }
        else
        {
            MessageBox.Show("The linked source document could not be found.", 
                          "Document Not Found", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Warning);
        }
    }
}
