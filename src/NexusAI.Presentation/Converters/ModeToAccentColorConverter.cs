using NexusAI.Domain.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NexusAI.Presentation.Converters;

public sealed class ModeToAccentColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not AppMode mode)
            return new SolidColorBrush(System.Windows.Media.Color.FromRgb(139, 92, 246));

        return mode switch
        {
            AppMode.Professional => new SolidColorBrush(System.Windows.Media.Color.FromRgb(139, 92, 246)), // Deep Purple #8B5CF6
            AppMode.Student => new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 217, 255)),      // Neon Blue #00D9FF
            _ => new SolidColorBrush(System.Windows.Media.Color.FromRgb(139, 92, 246))
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
