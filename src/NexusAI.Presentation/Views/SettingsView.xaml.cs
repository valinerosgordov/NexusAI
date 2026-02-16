using System.Windows;
using NexusAI.Presentation.ViewModels;

namespace NexusAI.Presentation.Views;

public partial class SettingsView : System.Windows.Controls.UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var mainWindow = Window.GetWindow(this);
        if (mainWindow?.DataContext is MainViewModel mainVm && !string.IsNullOrEmpty(mainVm.GeminiApiKey))
        {
            ApiKeyBox.Password = mainVm.GeminiApiKey;
        }
    }

    private void ApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        var mainWindow = Window.GetWindow(this);
        if (sender is System.Windows.Controls.PasswordBox pb && mainWindow?.DataContext is MainViewModel mainVm)
        {
            mainVm.GeminiApiKey = pb.Password;
        }
    }
}
