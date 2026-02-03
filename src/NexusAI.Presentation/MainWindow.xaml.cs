using System.ComponentModel;
using System.IO;
using System.Windows;
using NexusAI.Presentation.ViewModels;

namespace NexusAI.Presentation;

public partial class MainWindow : Window
{
    private bool _isWebViewInitialized;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnWindowLoaded;
    }

    private MainViewModel? ViewModel => DataContext as MainViewModel;

    private async void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        await InitializeWebView2Async();
        
        if (ViewModel is not null)
        {
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private async Task InitializeWebView2Async()
    {
        try
        {
            await DiagramWebView.EnsureCoreWebView2Async();
            
            var htmlPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Assets",
                "MermaidTemplate.html");

            if (File.Exists(htmlPath))
            {
                DiagramWebView.CoreWebView2.Navigate(htmlPath);
                _isWebViewInitialized = true;
            }
            else
            {
                // Fallback: use inline HTML
                var htmlContent = GetEmbeddedMermaidHtml();
                DiagramWebView.NavigateToString(htmlContent);
                _isWebViewInitialized = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to initialize WebView2: {ex.Message}", 
                "WebView2 Error", 
                MessageBoxButton.OK, 
                MessageBoxImage.Warning);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.CurrentMermaidDiagram) && _isWebViewInitialized)
        {
            RenderMermaidDiagram(ViewModel?.CurrentMermaidDiagram ?? string.Empty);
        }
    }

    private async void RenderMermaidDiagram(string mermaidCode)
    {
        if (!_isWebViewInitialized || string.IsNullOrWhiteSpace(mermaidCode))
            return;

        try
        {
            var escapedCode = mermaidCode
                .Replace("\\", "\\\\")
                .Replace("`", "\\`")
                .Replace("$", "\\$")
                .Replace("\r\n", "\\n")
                .Replace("\n", "\\n");

            await DiagramWebView.CoreWebView2.ExecuteScriptAsync(
                $"window.renderMermaid(`{escapedCode}`)");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to render diagram: {ex.Message}", 
                "Rendering Error", 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
        }
    }

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
        if (sender is System.Windows.Controls.RadioButton rb && 
            rb.Tag is string diagramType && 
            ViewModel is not null)
        {
            ViewModel.SelectedDiagramType = diagramType;
        }
    }
}
