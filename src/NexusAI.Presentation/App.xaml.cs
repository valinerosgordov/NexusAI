using Microsoft.Extensions.DependencyInjection;
using NexusAI.Application;
using NexusAI.Infrastructure;
using NexusAI.Presentation.ViewModels;
using System.Windows;

namespace NexusAI.Presentation;

public partial class App : System.Windows.Application
{
    private IServiceProvider? _serviceProvider;

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var services = new ServiceCollection();

        // Infrastructure layer (external dependencies)
        // API key will be set via ViewModel property binding
        services.AddInfrastructure(sp => string.Empty);

        // Application layer (use cases)
        services.AddApplication();

        // Presentation layer (ViewModels)
        services.AddSingleton<MainViewModel>();

        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = new MainWindow
        {
            DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
        };

        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        base.OnExit(e);
    }
}
