using NexusAI.Domain.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NexusAI.Presentation.Converters;

public sealed class PriorityToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TaskPriority priority || parameter is not string targetPriority)
            return Visibility.Collapsed;

        return priority.ToString().Equals(targetPriority, StringComparison.OrdinalIgnoreCase)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
