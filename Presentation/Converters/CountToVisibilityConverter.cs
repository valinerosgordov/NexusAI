using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PersonalNBV.Presentation.Converters;

/// <summary>
/// Converts count (int) to Visibility. Parameter "Inverted" = Visible when count is 0.
/// </summary>
public sealed class CountToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var count = value switch
        {
            int i => i,
            long l => (int)l,
            _ => 0
        };

        var inverted = string.Equals(parameter?.ToString(), "Inverted", StringComparison.OrdinalIgnoreCase);
        var visible = inverted ? count == 0 : count > 0;
        return visible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
