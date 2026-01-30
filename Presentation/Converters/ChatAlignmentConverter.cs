using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NexusAI.Presentation.Converters;

/// <summary>
/// Converts IsUser (bool) to HorizontalAlignment: Right for user, Left for AI.
/// </summary>
public sealed class ChatAlignmentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isUser)
            return isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        return HorizontalAlignment.Left;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
