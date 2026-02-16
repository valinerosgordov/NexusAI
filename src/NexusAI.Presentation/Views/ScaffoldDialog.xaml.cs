using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace NexusAI.Presentation.Views;

public partial class ScaffoldDialog : Window
{
    public string ProjectDescription { get; private set; } = string.Empty;
    public string[] Technologies { get; private set; } = [];
    public string TargetPath { get; private set; } = string.Empty;

    public ScaffoldDialog()
    {
        InitializeComponent();
        DescriptionTextBox.Focus();
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        using var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "Select target folder for project scaffolding",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = true
        };

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            TargetPathTextBox.Text = dialog.SelectedPath;
        }
    }

    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        ProjectDescription = DescriptionTextBox.Text?.Trim() ?? string.Empty;
        TargetPath = TargetPathTextBox.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(ProjectDescription))
        {
            MessageBox.Show(
                "Please describe your project.",
                "Input Required",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(TargetPath))
        {
            MessageBox.Show(
                "Please select a target folder.",
                "Folder Required",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        // Parse technologies (comma-separated)
        var techText = TechnologiesTextBox.Text?.Trim() ?? string.Empty;
        Technologies = string.IsNullOrWhiteSpace(techText)
            ? []
            : techText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => CancelButton_Click(sender, e);
}
