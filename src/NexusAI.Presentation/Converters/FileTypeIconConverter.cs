using System.Globalization;
using System.Windows.Data;
using NexusAI.Domain.Models;

namespace NexusAI.Presentation.Converters;

public sealed class FileTypeIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string sourceType)
        {
            return sourceType switch
            {
                SourceType.Document => "📄",
                SourceType.ObsidianNote => "📝",
                SourceType.PDF => "📕",
                SourceType.DOCX => "📘",
                SourceType.PPTX => "📊",
                SourceType.EPUB => "📖",
                SourceType.TXT => "📝",
                SourceType.MD => "📝",
                SourceType.Obsidian => "📝",
                _ => "📁"
            };
        }
        return "📁";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
