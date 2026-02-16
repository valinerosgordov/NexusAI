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
        MainViewModel? mainViewModel = null;

        ConfigureServices(services, () => mainViewModel?.GeminiApiKey ?? string.Empty);

        _serviceProvider = services.BuildServiceProvider();
        Services = _serviceProvider;

        InitializeDatabase();
        InitializeLocalization();

        mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.Project = _serviceProvider.GetRequiredService<ProjectViewModel>();
        mainViewModel.Wiki = _serviceProvider.GetRequiredService<WikiViewModel>();
        mainViewModel.Presentation = _serviceProvider.GetRequiredService<PresentationViewModel>();
        mainViewModel.Settings = _serviceProvider.GetRequiredService<SettingsViewModel>();

        var mainWindow = new MainWindow { DataContext = mainViewModel };
        mainWindow.Show();
    }

    private static void ConfigureServices(ServiceCollection services, Func<string> apiKeyProvider)
    {
        services.AddInfrastructure(apiKeyProvider);
        services.AddApplication();

        // Core ViewModels
        services.AddSingleton<DocumentsViewModel>();
        services.AddSingleton<ChatViewModel>();
        services.AddSingleton<ArtifactsViewModel>();
        services.AddSingleton<GraphViewModel>();
        services.AddSingleton<MainViewModel>();

        // Localization
        services.AddSingleton<NexusAI.Application.Interfaces.ILocalizationService, NexusAI.Presentation.Services.LocalizationService>();

        RegisterComplexViewModels(services);
    }

    private static void RegisterComplexViewModels(ServiceCollection services)
    {
        services.AddSingleton<ProjectViewModel>(sp =>
        {
            var vm = new ProjectViewModel(
                sp.GetRequiredService<NexusAI.Application.UseCases.Projects.GenerateProjectPlanHandler>(),
                sp.GetRequiredService<NexusAI.Application.UseCases.Projects.GetUserProjectsHandler>(),
                sp.GetRequiredService<NexusAI.Application.UseCases.Projects.UpdateTaskStatusHandler>(),
                sp.GetRequiredService<NexusAI.Application.UseCases.Scaffold.GenerateScaffoldHandler>(),
                sp.GetRequiredService<NexusAI.Application.Services.SessionContext>());
            vm.Initialize();
            return vm;
        });

        services.AddSingleton<WikiViewModel>(sp =>
        {
            var mainVm = sp.GetRequiredService<MainViewModel>();
            return new WikiViewModel(
                sp.GetRequiredService<NexusAI.Application.UseCases.Wiki.GenerateWikiHandler>(),
                sp.GetRequiredService<NexusAI.Application.UseCases.Wiki.GetWikiTreeHandler>(),
                sp.GetRequiredService<NexusAI.Application.UseCases.Wiki.UpdateWikiPageHandler>(),
                sp.GetRequiredService<NexusAI.Application.UseCases.Wiki.DeleteWikiPageHandler>(),
                () => mainVm.Sources.Where(s => s.IsIncluded).Select(s => s.Document).ToArray());
        });

        services.AddSingleton<PresentationViewModel>(sp =>
        {
            var mainVm = sp.GetRequiredService<MainViewModel>();
            return new PresentationViewModel(
                sp.GetRequiredService<NexusAI.Application.UseCases.Presentations.GeneratePresentationHandler>(),
                () => mainVm.Sources.Where(s => s.IsIncluded).Select(s => s.Document).ToArray());
        });

        services.AddSingleton<SettingsViewModel>(sp =>
        {
            var vm = new SettingsViewModel(sp.GetRequiredService<NexusAI.Application.Interfaces.ILocalizationService>());
            vm.Initialize();
            return vm;
        });
    }

    private static void InitializeDatabase()
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
