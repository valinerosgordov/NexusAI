using System.Windows;

namespace NexusAI.Presentation.Views;

public partial class WikiGeneratorDialog : Window
{
    public string Topic { get; private set; } = string.Empty;

    public WikiGeneratorDialog()
    {
        InitializeComponent();
        TopicTextBox.Focus();
    }

    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        Topic = TopicTextBox.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(Topic))
        {
            System.Windows.MessageBox.Show(
                "Please enter a topic for the wiki.",
                "Topic Required",
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

    private void CloseButton_Click(object sender, RoutedEventArgs e) => CancelButton_Click(sender, e);
}
