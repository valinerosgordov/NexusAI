using NexusAI.Domain.Models;
using System.Globalization;
using System.Windows.Data;

namespace NexusAI.Presentation.Converters;

public sealed class ModeToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not AppMode mode)
            return "Briefcase";

        return mode switch
        {
            AppMode.Professional => "Briefcase",
            AppMode.Student => "SchoolOutline",
            _ => "Briefcase"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
