using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NexusAI.Presentation.Converters;

public sealed class AppViewToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is AppView currentView && parameter is string targetViewName
            && Enum.TryParse<AppView>(targetViewName, out var targetView))
        {
            return currentView == targetView ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
