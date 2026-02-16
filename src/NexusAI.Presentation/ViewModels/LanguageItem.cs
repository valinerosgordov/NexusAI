using System.Globalization;

namespace NexusAI.Presentation.ViewModels;

public sealed class LanguageItem
{
    public required CultureInfo Culture { get; init; }
    public required string DisplayName { get; init; }
    public required string NativeName { get; init; }

    public override string ToString() => DisplayName;
}
