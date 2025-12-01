using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Ring.Converters
{
    /// <summary>
    /// Converts boolean values to color (Green for true, Gray for false)
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4EC9B0")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

