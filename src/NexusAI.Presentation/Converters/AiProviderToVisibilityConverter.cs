using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NexusAI.Domain.Models;

namespace NexusAI.Presentation.Converters;

public sealed class AiProviderToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is AiProvider provider && parameter is string targetProvider)
        {
            var isMatch = targetProvider.Equals(provider.ToString(), StringComparison.OrdinalIgnoreCase);
            return isMatch ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
