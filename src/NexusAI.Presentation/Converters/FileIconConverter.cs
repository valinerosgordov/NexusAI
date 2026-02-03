using System.Globalization;
using System.IO;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace NexusAI.Presentation.Converters;

public sealed class FileIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => PackIconKind.FilePdfBox,
                ".docx" or ".doc" => PackIconKind.FileWord,
                ".pptx" or ".ppt" => PackIconKind.FilePowerpoint,
                ".epub" => PackIconKind.BookOpen,
                ".txt" => PackIconKind.FileDocument,
                ".md" => PackIconKind.LanguageMarkdown,
                _ => PackIconKind.File
            };
        }
        return PackIconKind.File;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
