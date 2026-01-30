using System.Globalization;
using System.Windows.Data;
using NexusAI.Domain.Models;

namespace NexusAI.Presentation.Converters;

public sealed class FileTypeIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SourceType sourceType)
        {
            return sourceType switch
            {
                SourceType.Pdf => "📄",
                SourceType.ObsidianNote => "📝",
                _ => "📁"
            };
        }
        return "📁";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
