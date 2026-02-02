using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using NexusAI.Presentation.ViewModels;

namespace NexusAI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            MaximizeWindow(sender, e);
        }
        else
        {
            DragMove();
        }
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            ApiKeyBox.Password = viewModel.GeminiApiKey;
        }
    }

    private void ApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel && sender is PasswordBox passwordBox)
        {
            viewModel.GeminiApiKey = passwordBox.Password;
        }
    }

    #region Window Controls

    private void MinimizeWindow(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeWindow(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized 
            ? WindowState.Normal 
            : WindowState.Maximized;
    }

    private void CloseWindow(object sender, RoutedEventArgs e)
    {
        Close();
    }

    #endregion

    #region Tab Switching

    private void ShowArtifactsTab(object sender, RoutedEventArgs e)
    {
        if (ArtifactsTabContent != null && GraphTabContent != null)
        {
            ArtifactsTabContent.Visibility = Visibility.Visible;
            GraphTabContent.Visibility = Visibility.Collapsed;
        }
    }

    private void ShowGraphTab(object sender, RoutedEventArgs e)
    {
        if (ArtifactsTabContent != null && GraphTabContent != null)
        {
            ArtifactsTabContent.Visibility = Visibility.Collapsed;
            GraphTabContent.Visibility = Visibility.Visible;
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.RefreshGraphCommand.Execute(null);
                DrawGraph();
            }
        }
    }

    private void DrawGraph()
    {
        if (DataContext is not MainViewModel viewModel || GraphCanvas == null)
            return;

        GraphCanvas.Children.Clear();
        foreach (var edge in viewModel.GraphEdges)
        {
            var sourceNode = viewModel.GraphNodes.FirstOrDefault(n => n.DocumentId == edge.Source);
            var targetNode = viewModel.GraphNodes.FirstOrDefault(n => n.DocumentId == edge.Target);

            if (sourceNode != null && targetNode != null)
            {
                var line = new Line
                {
                    X1 = sourceNode.X,
                    Y1 = sourceNode.Y,
                    X2 = targetNode.X,
                    Y2 = targetNode.Y,
                    Stroke = new SolidColorBrush(Colors.Gray),
                    StrokeThickness = Math.Max(1, edge.SharedKeywords / 2.0),
                    Opacity = 0.4
                };
                GraphCanvas.Children.Add(line);
            }
        }
        foreach (var node in viewModel.GraphNodes)
        {
            var ellipse = new Ellipse
            {
                Width = 60,
                Height = 60,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7C3AED")),
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 2
            };

            Canvas.SetLeft(ellipse, node.X - 30);
            Canvas.SetTop(ellipse, node.Y - 30);

            var label = new TextBlock
            {
                Text = node.Name.Length > 12 ? node.Name.Substring(0, 12) + "..." : node.Name,
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Width = 60
            };

            Canvas.SetLeft(label, node.X - 30);
            Canvas.SetTop(label, node.Y + 35);

            GraphCanvas.Children.Add(ellipse);
            GraphCanvas.Children.Add(label);
        }
    }

    #endregion

    #region Image Drag & Drop

    private async void ChatInput_Drop(object sender, DragEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel)
            return;

        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var imageFiles = files
                .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                           f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                           f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (imageFiles.Length > 0)
            {
                var base64Images = new List<string>();
                foreach (var imagePath in imageFiles)
                {
                    try
                    {
                        var bytes = await File.ReadAllBytesAsync(imagePath);
                        var base64 = Convert.ToBase64String(bytes);
                        base64Images.Add(base64);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load image: {ex.Message}", "Error", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                if (base64Images.Count > 0)
                {

                    viewModel.PendingImages = base64Images.ToArray();
                    MessageBox.Show($"✅ {base64Images.Count} image(s) attached. Ask your question now.", 
                        "Images Attached", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }

    #endregion

    #region Drag-and-Drop with Premium Overlay

    private async void Sidebar_Drop(object sender, DragEventArgs e)
    {
        HideDropOverlay();

        if (DataContext is not MainViewModel viewModel)
            return;

        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            
            var supportedExtensions = new[] { ".pdf", ".docx", ".pptx", ".epub", ".txt", ".md" };
            var supportedFiles = files
                .Where(f => supportedExtensions.Any(ext => 
                    f.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            if (supportedFiles.Length > 0)
            {
                await viewModel.HandleDroppedFilesAsync(supportedFiles);
            }
            else
            {
                MessageBox.Show("Please drop supported document files (PDF, DOCX, PPTX, EPUB, TXT, MD).", 
                    "Invalid File Type", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    private void Sidebar_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var supportedExtensions = new[] { ".pdf", ".docx", ".pptx", ".epub", ".txt", ".md" };
            var hasSupported = files.Any(f => 
                supportedExtensions.Any(ext => f.EndsWith(ext, StringComparison.OrdinalIgnoreCase)));

            if (hasSupported)
            {
                e.Effects = DragDropEffects.Copy;
                ShowDropOverlay();
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
        
        e.Handled = true;
    }

    private void Sidebar_DragLeave(object sender, DragEventArgs e)
    {
        HideDropOverlay();
    }

    private void ShowDropOverlay()
    {
        if (DragDropOverlay != null)
        {
            DragDropOverlay.Visibility = Visibility.Visible;
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.2),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            
            DragDropOverlay.BeginAnimation(OpacityProperty, fadeIn);
        }
    }

    private void HideDropOverlay()
    {
        if (DragDropOverlay != null)
        {
            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.2),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };
            
            fadeOut.Completed += (s, e) =>
            {
                DragDropOverlay.Visibility = Visibility.Collapsed;
            };
            
            DragDropOverlay.BeginAnimation(OpacityProperty, fadeOut);
        }
    }

    #endregion
}

