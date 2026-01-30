using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PersonalNBV.Presentation.Converters;

/// <summary>
/// Converts IsUser boolean to appropriate chat bubble style
/// </summary>
public sealed class ChatBubbleStyleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isUser)
        {
            var styleKey = isUser ? "UserChatBubble" : "AIChatBubble";
            return System.Windows.Application.Current.TryFindResource(styleKey) as Style 
                   ?? throw new InvalidOperationException($"Style '{styleKey}' not found");
        }
        
        throw new InvalidOperationException("Value must be boolean");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
