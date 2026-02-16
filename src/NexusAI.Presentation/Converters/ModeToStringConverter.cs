using NexusAI.Domain.Models;
using System.Globalization;
using System.Windows.Data;

namespace NexusAI.Presentation.Converters;

public sealed class ModeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not AppMode mode || parameter is not string param)
            return string.Empty;

        var options = param.Split('/');
        if (options.Length != 2)
            return param;

        return mode switch
        {
            AppMode.Professional => options[0].Trim(),
            AppMode.Student => options[1].Trim(),
            _ => options[0].Trim()
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
