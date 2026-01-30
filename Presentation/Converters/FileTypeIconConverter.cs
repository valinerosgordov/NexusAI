using System.Globalization;
using System.Windows.Data;
using PersonalNBV.Domain.Models;

namespace PersonalNBV.Presentation.Converters;

/// <summary>
/// Converts SourceType to appropriate file icon
/// </summary>
public sealed class FileTypeIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SourceType sourceType)
        {
            return sourceType switch
            {
                SourceType.Pdf => "📄",              // FileText icon (PDF)
                SourceType.ObsidianNote => "📝",    // PenLine icon (MD)
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
