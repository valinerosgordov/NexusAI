using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using PersonalNBV.Presentation.ViewModels;

namespace PersonalNBV;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
        
        // Enable window dragging
        MouseDown += (s, e) =>
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        };
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

    #region Tab Switching with Animations

    private void ShowChatTab(object sender, RoutedEventArgs e)
    {
        SwitchTab(ChatTab, ArtifactsTab);
        ChatTabButton.Style = (Style)FindResource("PremiumButton");
        ArtifactsTabButton.Style = (Style)FindResource("GhostButton");
    }

    private void ShowArtifactsTab(object sender, RoutedEventArgs e)
    {
        SwitchTab(ArtifactsTab, ChatTab);
        ChatTabButton.Style = (Style)FindResource("GhostButton");
        ArtifactsTabButton.Style = (Style)FindResource("PremiumButton");
    }

    private void SwitchTab(UIElement showTab, UIElement hideTab)
    {
        // Fade out old tab
        var fadeOut = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromSeconds(0.15),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
        };

        fadeOut.Completed += (s, e) =>
        {
            hideTab.Visibility = Visibility.Collapsed;
            showTab.Visibility = Visibility.Visible;
            
            // Fade in new tab
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            
            showTab.BeginAnimation(OpacityProperty, fadeIn);
        };

        hideTab.BeginAnimation(OpacityProperty, fadeOut);
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
            
            var supportedFiles = files
                .Where(f => f.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) || 
                           f.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (supportedFiles.Length > 0)
            {
                await viewModel.HandleDroppedFilesAsync(supportedFiles);
            }
            else
            {
                MessageBox.Show("Please drop PDF or MD files only.", "Invalid File Type", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    private void Sidebar_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var hasSupported = files.Any(f => 
                f.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) || 
                f.EndsWith(".md", StringComparison.OrdinalIgnoreCase));

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
            
            // Fade in animation
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

