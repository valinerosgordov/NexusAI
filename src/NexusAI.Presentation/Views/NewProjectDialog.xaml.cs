using System.Windows;

namespace NexusAI.Presentation.Views;

public partial class NewProjectDialog : Window
{
    public string ProjectIdea { get; private set; } = string.Empty;

    public NewProjectDialog()
    {
        InitializeComponent();
        IdeaTextBox.Focus();
    }

    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        ProjectIdea = IdeaTextBox.Text?.Trim() ?? string.Empty;
        
        if (string.IsNullOrWhiteSpace(ProjectIdea))
        {
            MessageBox.Show(
                "Please describe your project idea.",
                "Input Required",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
