using System;
using System.Globalization;
using System.Windows.Data;

namespace Ring.Converters
{
    /// <summary>
    /// Converts boolean values to string representation (0/1 or True/False)
    /// </summary>
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // Return "1" for true, "0" for false (PLC style)
                return boolValue ? "1" : "0";
            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                return strValue == "1" || strValue.ToLower() == "true";
            }
            return false;
        }
    }
}

