using Microsoft.Extensions.DependencyInjection;
using NexusAI.Application;
using NexusAI.Infrastructure;
using NexusAI.Presentation.ViewModels;
using System.Windows;

namespace NexusAI.Presentation;

public partial class App : System.Windows.Application
{
    private IServiceProvider? _serviceProvider;
    
    public static IServiceProvider? Services { get; private set; }

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
        
        services.AddSingleton<ProjectViewModel>(sp =>
        {
            var generatePlanHandler = sp.GetRequiredService<NexusAI.Application.UseCases.Projects.GenerateProjectPlanHandler>();
            var getProjectsHandler = sp.GetRequiredService<NexusAI.Application.UseCases.Projects.GetUserProjectsHandler>();
            var updateTaskStatusHandler = sp.GetRequiredService<NexusAI.Application.UseCases.Projects.UpdateTaskStatusHandler>();
            var generateScaffoldHandler = sp.GetRequiredService<NexusAI.Application.UseCases.Scaffold.GenerateScaffoldHandler>();
            var aiServiceFactory = sp.GetRequiredService<NexusAI.Application.Interfaces.IAiServiceFactory>();
            var sessionContext = sp.GetRequiredService<NexusAI.Application.Services.SessionContext>();
            
            return new ProjectViewModel(
                generatePlanHandler,
                getProjectsHandler,
                updateTaskStatusHandler,
                generateScaffoldHandler,
                aiServiceFactory,
                sessionContext);
        });
        
        services.AddSingleton<WikiViewModel>(sp =>
        {
            var generateWikiHandler = sp.GetRequiredService<NexusAI.Application.UseCases.Wiki.GenerateWikiHandler>();
            var getWikiTreeHandler = sp.GetRequiredService<NexusAI.Application.UseCases.Wiki.GetWikiTreeHandler>();
            var updateWikiPageHandler = sp.GetRequiredService<NexusAI.Application.UseCases.Wiki.UpdateWikiPageHandler>();
            var deleteWikiPageHandler = sp.GetRequiredService<NexusAI.Application.UseCases.Wiki.DeleteWikiPageHandler>();
            var mainViewModel = sp.GetRequiredService<MainViewModel>();
            
            return new WikiViewModel(
                generateWikiHandler,
                getWikiTreeHandler,
                updateWikiPageHandler,
                deleteWikiPageHandler,
                () => mainViewModel.Sources
                    .Where(s => s.IsIncluded)
                    .Select(s => s.Document)
                    .ToArray());
        });

        services.AddSingleton<PresentationViewModel>(sp =>
        {
            var handler = sp.GetRequiredService<NexusAI.Application.UseCases.Presentations.GeneratePresentationHandler>();
            var mainViewModel = sp.GetRequiredService<MainViewModel>();
            
            return new PresentationViewModel(
                handler,
                () => mainViewModel.Sources
                    .Where(s => s.IsIncluded)
                    .Select(s => s.Document)
                    .ToArray());
        });
        
        services.AddSingleton<SettingsViewModel>(sp => 
        {
            var localizationService = sp.GetRequiredService<NexusAI.Application.Interfaces.ILocalizationService>();
            return new SettingsViewModel(localizationService);
        });
        
        // Localization (Presentation layer service due to WPF dependencies)
        services.AddSingleton<NexusAI.Application.Interfaces.ILocalizationService, NexusAI.Presentation.Services.LocalizationService>();

        _serviceProvider = services.BuildServiceProvider();
        Services = _serviceProvider;

        // Initialize database
        InitializeDatabase();
        
        // Initialize localization - load saved language or default
        InitializeLocalization();

        var mainWindow = new MainWindow
        {
            DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
        };

        mainWindow.Show();
    }

    private void InitializeDatabase()
    {
        // DB placeholder
    }
    
    private void InitializeLocalization()
    {
        try
        {
            var localizationService = _serviceProvider!.GetRequiredService<NexusAI.Application.Interfaces.ILocalizationService>();
            var result = localizationService.LoadSavedLanguage();
            
            if (!result.IsSuccess)
            {
                System.Diagnostics.Debug.WriteLine($"Localization initialization: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to initialize localization: {ex.Message}");
        }
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
