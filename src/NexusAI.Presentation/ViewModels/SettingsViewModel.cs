using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NexusAI.Application.Interfaces;
using System.Collections.ObjectModel;
using System.Globalization;

namespace NexusAI.Presentation.ViewModels;

public sealed partial class SettingsViewModel(ILocalizationService localizationService) : ObservableObject
{
    private readonly ILocalizationService _localizationService = localizationService;

    [ObservableProperty] private LanguageItem? _selectedLanguage;
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private bool _isBusy;

    public ObservableCollection<LanguageItem> Languages { get; } = [];

    public void Initialize()
    {
        LoadAvailableLanguages();
    }

    private void LoadAvailableLanguages()
    {
        Languages.Clear();

        foreach (var culture in _localizationService.AvailableLanguages)
        {
            var langItem = new LanguageItem
            {
                Culture = culture,
                DisplayName = GetLanguageDisplayName(culture),
                NativeName = culture.NativeName
            };

            Languages.Add(langItem);

            // Set current language as selected
            if (culture.Name.Equals(_localizationService.CurrentCulture.Name, StringComparison.OrdinalIgnoreCase))
            {
                SelectedLanguage = langItem;
            }
        }
    }

    partial void OnSelectedLanguageChanged(LanguageItem? value)
    {
        if (value == null || IsBusy) return;

        _ = ChangeLanguageAsync(value.Culture);
    }

    private async Task ChangeLanguageAsync(CultureInfo culture)
    {
        IsBusy = true;
        StatusMessage = "Changing language...";

        try
        {
            await Task.Run(() =>
            {
                var result = _localizationService.SetLanguage(culture);

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    if (result.IsSuccess)
                    {
                        StatusMessage = $"✓ Language changed to {culture.DisplayName}";
                    }
                    else
                    {
                        StatusMessage = $"⚠ {result.Error}";
                    }
                });
            });
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;

            // Clear status message after 3 seconds
            await Task.Delay(3000);
            if (!IsBusy)
                StatusMessage = string.Empty;
        }
    }

    private static string GetLanguageDisplayName(CultureInfo culture)
    {
        return culture.Name switch
        {
            "en-US" => "English",
            "ru-RU" => "Русский",
            _ => culture.DisplayName
        };
    }
}

public sealed class LanguageItem
{
    public required CultureInfo Culture { get; init; }
    public required string DisplayName { get; init; }
    public required string NativeName { get; init; }

    public override string ToString() => DisplayName;
}
