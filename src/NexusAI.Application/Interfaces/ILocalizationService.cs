using NexusAI.Domain.Common;
using System.Globalization;

namespace NexusAI.Application.Interfaces;

public interface ILocalizationService
{
    /// <summary>
    /// Gets the currently active culture
    /// </summary>
    CultureInfo CurrentCulture { get; }
    
    /// <summary>
    /// Gets all available languages
    /// </summary>
    IReadOnlyList<CultureInfo> AvailableLanguages { get; }
    
    /// <summary>
    /// Sets the application language at runtime
    /// </summary>
    /// <param name="culture">Target culture (e.g., en-US, ru-RU)</param>
    /// <returns>Result indicating success or failure</returns>
    Result<bool> SetLanguage(CultureInfo culture);
    
    /// <summary>
    /// Loads the saved language preference from settings
    /// </summary>
    /// <returns>Result indicating success or failure</returns>
    Result<bool> LoadSavedLanguage();
}
