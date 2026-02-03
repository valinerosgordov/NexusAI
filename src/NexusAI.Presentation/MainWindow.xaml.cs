using System.Windows;
using NexusAI.Presentation.ViewModels;

namespace NexusAI.Presentation;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private MainViewModel? ViewModel => DataContext as MainViewModel;

    private void OnDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop) && 
            e.Data.GetData(DataFormats.FileDrop) is string[] files)
        {
            _ = HandleDroppedFilesAsync(files);
        }
    }

    private async Task HandleDroppedFilesAsync(string[] files)
    {
        if (ViewModel is null || files.Length == 0)
            return;

        foreach (var filePath in files)
        {
            await ViewModel.AddDocumentCommand.ExecuteAsync(null);
        }
    }
}
