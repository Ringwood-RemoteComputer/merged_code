using System;
using System.Globalization;
using System.Windows.Data;

namespace Ring.Converters
{
    /// <summary>
    /// Converts a percentage value to a height value for tank fill level visualization
    /// </summary>
    public class PercentageToHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return 0;

            // Parse the percentage value
            if (!double.TryParse(value.ToString(), out double percentage))
                return 0;

            // Parse the parameter (tank height)
            if (!double.TryParse(parameter.ToString(), out double tankHeight))
                return 0;

            // Ensure percentage is between 0 and 100
            percentage = Math.Max(0, Math.Min(100, percentage));

            // Calculate the height based on percentage
            double height = (percentage / 100.0) * tankHeight;

            return height;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This converter is one-way only, so ConvertBack is not implemented
            throw new NotImplementedException();
        }
    }
}

