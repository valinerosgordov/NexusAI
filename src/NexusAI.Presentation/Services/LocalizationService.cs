using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using System.Globalization;
using System.IO;
using System.Windows;

namespace NexusAI.Presentation.Services;

public sealed class LocalizationService : ILocalizationService
{
    private const string DefaultLanguage = "en-US";
    private const string SettingsFileName = "settings.json";
    private static readonly System.Text.Json.JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    
    private static readonly IReadOnlyList<CultureInfo> _availableLanguages =
    [
        new CultureInfo("en-US"),
        new CultureInfo("ru-RU")
    ];

    public CultureInfo CurrentCulture { get; private set; } = new CultureInfo(DefaultLanguage);
    
    public IReadOnlyList<CultureInfo> AvailableLanguages => _availableLanguages;

    public Result<bool> SetLanguage(CultureInfo culture)
    {
        try
        {
            if (culture == null)
                return Result<bool>.Failure("Culture cannot be null");

            // Validate language is supported
            if (!_availableLanguages.Any(c => c.Name.Equals(culture.Name, StringComparison.OrdinalIgnoreCase)))
                return Result<bool>.Failure($"Language '{culture.Name}' is not supported. Falling back to {DefaultLanguage}.");

            // Remove current language dictionary
            RemoveCurrentLanguageDictionary();

            // Load new language dictionary
            var loadResult = LoadLanguageDictionary(culture.Name);
            if (!loadResult.IsSuccess)
            {
                // Fallback to default language
                LoadLanguageDictionary(DefaultLanguage);
                return Result<bool>.Failure($"Failed to load '{culture.Name}'. Fallback to {DefaultLanguage}: {loadResult.Error}");
            }

            // Update current culture
            CurrentCulture = culture;
            
            // Set thread culture for proper formatting (dates, numbers, etc.)
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // Persist choice to settings
            SaveLanguagePreference(culture.Name);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error setting language: {ex.Message}");
        }
    }

    public Result<bool> LoadSavedLanguage()
    {
        try
        {
            var savedLanguage = LoadLanguagePreference();
            if (string.IsNullOrEmpty(savedLanguage))
            {
                // No saved preference, use default
                return SetLanguage(new CultureInfo(DefaultLanguage));
            }

            return SetLanguage(new CultureInfo(savedLanguage));
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error loading saved language: {ex.Message}");
        }
    }

    private static void RemoveCurrentLanguageDictionary()
    {
        try
        {
            var app = System.Windows.Application.Current;
            if (app == null) return;

            // Find and remove existing language dictionaries
            var languageDictionaries = app.Resources.MergedDictionaries
                .Where(d => d.Source != null && 
                           d.Source.OriginalString.Contains("/Resources/Languages/"))
                .ToList();

            foreach (var dict in languageDictionaries)
            {
                app.Resources.MergedDictionaries.Remove(dict);
            }
        }
        catch (Exception ex)
        {
            // Log but don't throw - this is cleanup
            System.Diagnostics.Debug.WriteLine($"Error removing language dictionary: {ex.Message}");
        }
    }

    private static Result<bool> LoadLanguageDictionary(string cultureName)
    {
        try
        {
            var app = System.Windows.Application.Current;
            if (app == null)
                return Result<bool>.Failure("Application.Current is null");

            var relativePath = string.Create(System.Globalization.CultureInfo.InvariantCulture, $"/NexusAI;component/Resources/Languages/{cultureName}.xaml");
            var uri = new Uri(relativePath, UriKind.Relative);

            // Try to load the resource dictionary
            var dictionary = new ResourceDictionary { Source = uri };
            
            // Add to merged dictionaries
            app.Resources.MergedDictionaries.Add(dictionary);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to load language dictionary for '{cultureName}': {ex.Message}");
        }
    }

    private static void SaveLanguagePreference(string cultureName)
    {
        try
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NexusAI");
            
            Directory.CreateDirectory(appDataPath);
            
            var settingsFile = Path.Combine(appDataPath, SettingsFileName);
            var settings = new { PreferredLanguage = cultureName };
            
            var json = System.Text.Json.JsonSerializer.Serialize(settings, _jsonOptions);
            
            File.WriteAllText(settingsFile, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving language preference: {ex.Message}");
        }
    }

    private static string? LoadLanguagePreference()
    {
        try
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NexusAI");
            
            var settingsFile = Path.Combine(appDataPath, SettingsFileName);
            
            if (!File.Exists(settingsFile))
                return null;
            
            var json = File.ReadAllText(settingsFile);
            var settings = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            
            return settings?.TryGetValue("PreferredLanguage", out var language) == true 
                ? language 
                : null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading language preference: {ex.Message}");
            return null;
        }
    }
}
