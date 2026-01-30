using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace NexusAI.Presentation.Converters;

public sealed partial class CitationClickableConverter : IValueConverter
{
    private static readonly Brush CitationBrush = new SolidColorBrush(Color.FromRgb(37, 99, 235)); // Blue
    private static readonly Brush NormalBrush = new SolidColorBrush(Color.FromRgb(15, 23, 42)); // Dark text

    [GeneratedRegex(@"\[([^\]]+)\]")]
    private static partial Regex CitationRegex();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string text)
            return new TextBlock { Text = string.Empty };

        var textBlock = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            Foreground = NormalBrush
        };

        var matches = CitationRegex().Matches(text);
        var lastIndex = 0;

        foreach (Match match in matches)
        {
            if (match.Index > lastIndex)
            {
                textBlock.Inlines.Add(new Run(text.Substring(lastIndex, match.Index - lastIndex)));
            }

            var sourceName = match.Groups[1].Value;
            var hyperlink = new Hyperlink(new Run(match.Value))
            {
                Foreground = CitationBrush,
                FontWeight = FontWeights.SemiBold,
                ToolTip = $"Click to highlight: {sourceName}",
                TextDecorations = null,
                Tag = sourceName
            };

            hyperlink.MouseEnter += (s, e) => hyperlink.TextDecorations = TextDecorations.Underline;
            hyperlink.MouseLeave += (s, e) => hyperlink.TextDecorations = null;
            
            hyperlink.Click += (s, e) =>
            {
                e.Handled = true;
                OnCitationClicked(sourceName);
            };

            textBlock.Inlines.Add(hyperlink);

            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < text.Length)
        {
            textBlock.Inlines.Add(new Run(text.Substring(lastIndex)));
        }

        return textBlock;
    }

    private static void OnCitationClicked(string sourceName)
    {
        var mainWindow = System.Windows.Application.Current.MainWindow;
        if (mainWindow?.DataContext is Presentation.ViewModels.MainViewModel viewModel)
        {
            viewModel.HighlightSourceByName(sourceName);
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
