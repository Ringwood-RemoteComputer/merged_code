using System;
using System.Globalization;
using System.Windows.Data;

namespace Ring.Converters
{
    /// <summary>
    /// Converts ProgressBar value to percentage text
    /// </summary>
    public class ProgressBarValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is double value && values[1] is double maximum)
            {
                if (maximum == 0)
                    return "0%";
                
                double percentage = (value / maximum) * 100.0;
                return $"{percentage:F1}%";
            }
            
            return "0%";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts ProgressBar value and maximum to width for the indicator
    /// </summary>
    public class ProgressBarWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3 && 
                values[0] is double value && 
                values[1] is double maximum && 
                values[2] is double actualWidth)
            {
                if (maximum == 0 || actualWidth == 0)
                    return 0.0;
                
                double percentage = value / maximum;
                return actualWidth * percentage;
            }
            
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

