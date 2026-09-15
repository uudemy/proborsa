using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StockPro.App.Converters;

public sealed class BooleanToPositiveBrushConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (value is bool isPositive && isPositive)
        {
            return new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#22C55E"));
        }

        return new SolidColorBrush(
            (Color)ColorConverter.ConvertFromString("#EF4444"));
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}