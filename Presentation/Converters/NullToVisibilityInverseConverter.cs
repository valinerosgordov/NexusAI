using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NexusAI.Presentation.Converters;

/// <summary>
/// Returns Visible when value is null, Collapsed when non-null (opposite of NullToVisibilityConverter).
/// </summary>
public sealed class NullToVisibilityInverseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is null ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
