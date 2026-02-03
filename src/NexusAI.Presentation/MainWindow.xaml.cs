using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using NexusAI.Presentation.ViewModels;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using DataFormats = System.Windows.DataFormats;
using RadioButton = System.Windows.Controls.RadioButton;

namespace NexusAI.Presentation;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private MainViewModel? ViewModel => DataContext as MainViewModel;


    private static string GetEmbeddedMermaidHtml()
    {
        return """
<!DOCTYPE html>
<html>
<head>
    <style>
        body { margin: 0; padding: 20px; background: #0D0D0D; color: #F5F5F7; }
        #diagram-container { background: #1C1C1E; border-radius: 12px; padding: 32px; }
        .mermaid { display: flex; justify-content: center; }
    </style>
    <script type="module">
        import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@11/dist/mermaid.esm.min.mjs';
        mermaid.initialize({ 
            startOnLoad: true, 
            theme: 'dark',
            themeVariables: { primaryColor: '#8B5CF6', lineColor: '#10B981' }
        });
        window.renderMermaid = function(code) {
            document.getElementById('diagram-container').innerHTML = `<div class="mermaid">${code}</div>`;
            mermaid.run({ querySelector: '.mermaid' });
        };
    </script>
</head>
<body>
    <div id="diagram-container">⏳ Waiting for diagram...</div>
</body>
</html>
""";
    }

    private void OnDrop(object sender, System.Windows.DragEventArgs e)
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

    private void DiagramType_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton rb && 
            rb.Tag is string diagramType && 
            ViewModel is not null)
        {
            ViewModel.SelectedDiagramType = diagramType;
        }
    }

    // Window Chrome handlers
    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        else
            DragMove();
    }

    private void MinimizeWindow(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void MaximizeWindow(object sender, RoutedEventArgs e) =>
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void CloseWindow(object sender, RoutedEventArgs e) => Close();

    // Drag & Drop handlers
    private void Sidebar_Drop(object sender, System.Windows.DragEventArgs e) => OnDrop(sender, e);

    private void Sidebar_DragEnter(object sender, System.Windows.DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) 
            ? System.Windows.DragDropEffects.Copy 
            : System.Windows.DragDropEffects.None;
    }

    private void Sidebar_DragLeave(object sender, System.Windows.DragEventArgs e)
    {
        // Visual feedback when leaving drop zone
    }

    private void ChatInput_Drop(object sender, System.Windows.DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop) && 
            e.Data.GetData(DataFormats.FileDrop) is string[] files &&
            ViewModel is not null)
        {
            var imageFiles = files.Where(f => 
                f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".webp", StringComparison.OrdinalIgnoreCase)).ToArray();

            if (imageFiles.Length > 0)
            {
                ViewModel.PendingImages = imageFiles;
            }
        }
    }

    // Password handler
    private void ApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.PasswordBox pb && ViewModel is not null)
        {
            ViewModel.GeminiApiKey = pb.Password;
        }
    }

    // Tab switching
    private void ShowArtifactsTab(object sender, RoutedEventArgs e)
    {
        // Handled by bindings
    }

    private void ShowGraphTab(object sender, RoutedEventArgs e)
    {
        // Handled by bindings
    }
}
