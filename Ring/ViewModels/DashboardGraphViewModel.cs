using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;

namespace Ring.ViewModels
{
    /// <summary>
    /// ViewModel for dashboard graphs with placeholder data
    /// </summary>
    public class DashboardGraphViewModel : INotifyPropertyChanged
    {
        #region Private Fields
        private SeriesCollection _batchesPerShiftSeries;
        private SeriesCollection _starchUsagePerDaySeries;
        private string[] _shiftLabels;
        private string[] _dayLabels;
        private Random _random;
        #endregion

        #region Constructor
        public DashboardGraphViewModel()
        {
            _random = new Random();
            InitializeBatchesPerShiftChart();
            InitializeStarchUsagePerDayChart();
        }
        #endregion

        #region Properties
        public SeriesCollection BatchesPerShiftSeries
        {
            get => _batchesPerShiftSeries;
            set => SetProperty(ref _batchesPerShiftSeries, value);
        }

        public SeriesCollection StarchUsagePerDaySeries
        {
            get => _starchUsagePerDaySeries;
            set => SetProperty(ref _starchUsagePerDaySeries, value);
        }

        public string[] ShiftLabels
        {
            get => _shiftLabels;
            set => SetProperty(ref _shiftLabels, value);
        }

        public string[] DayLabels
        {
            get => _dayLabels;
            set => SetProperty(ref _dayLabels, value);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize the Batches Per Shift bar chart with placeholder data
        /// </summary>
        private void InitializeBatchesPerShiftChart()
        {
            // Create shift labels for 2 days (6 shifts total)
            string[] days = { "Mon", "Tue" };
            var labels = new List<string>();
            for (int day = 0; day < 2; day++)
            {
                for (int shift = 1; shift <= 3; shift++)
                {
                    labels.Add($"Shift {shift}\n{days[day]}");
                }
            }
            ShiftLabels = labels.ToArray();

            // Create column series with placeholder data
            var values = new ChartValues<double>();
            for (int i = 0; i < 6; i++)
            {
                int batchCount = _random.Next(6, 13); // 6-12 inclusive
                values.Add((double)batchCount);
            }

            // Create vibrant blue gradient brush for bars
            var blueGradient = new System.Windows.Media.LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0, 0),
                EndPoint = new System.Windows.Point(0, 1)
            };
            blueGradient.GradientStops.Add(new System.Windows.Media.GradientStop(
                System.Windows.Media.Color.FromRgb(70, 130, 255), 0.0)); // Bright blue
            blueGradient.GradientStops.Add(new System.Windows.Media.GradientStop(
                System.Windows.Media.Color.FromRgb(30, 80, 200), 1.0)); // Darker blue

            BatchesPerShiftSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Batches",
                    Values = values,
                    Fill = blueGradient,
                    Stroke = System.Windows.Media.Brushes.White,
                    StrokeThickness = 2,
                    MaxColumnWidth = 60
                }
            };
        }

        /// <summary>
        /// Initialize the Starch Usage Per Day line chart with placeholder data
        /// </summary>
        private void InitializeStarchUsagePerDayChart()
        {
            // Create day labels
            DayLabels = new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

            // Create line series with placeholder data
            var values = new ChartValues<double>();
            for (int day = 0; day < 7; day++)
            {
                double starchUsage = 0;

                // Sunday (0) and Monday (1) are closed - 0 usage
                if (day == 0 || day == 1)
                {
                    starchUsage = 0;
                }
                else
                {
                    // Tuesday through Saturday - 3 shifts per day
                    for (int shift = 0; shift < 3; shift++)
                    {
                        int batchCount = _random.Next(6, 13); // 6-12 batches per shift
                        double starchPerBatch = _random.Next(75, 101); // 75-100 lbs per batch
                        starchUsage += batchCount * starchPerBatch;
                    }
                }

                values.Add(starchUsage);
            }

            // Create vibrant teal/cyan gradient for line chart
            var tealGradient = new System.Windows.Media.LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0, 0),
                EndPoint = new System.Windows.Point(0, 1)
            };
            tealGradient.GradientStops.Add(new System.Windows.Media.GradientStop(
                System.Windows.Media.Color.FromRgb(0, 200, 220), 0.0)); // Bright teal
            tealGradient.GradientStops.Add(new System.Windows.Media.GradientStop(
                System.Windows.Media.Color.FromRgb(0, 150, 180), 1.0)); // Darker teal

            // Create area fill with transparency
            var areaFill = new System.Windows.Media.LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0, 0),
                EndPoint = new System.Windows.Point(0, 1)
            };
            areaFill.GradientStops.Add(new System.Windows.Media.GradientStop(
                System.Windows.Media.Color.FromArgb(100, 0, 200, 220), 0.0));
            areaFill.GradientStops.Add(new System.Windows.Media.GradientStop(
                System.Windows.Media.Color.FromArgb(50, 0, 150, 180), 1.0));

            StarchUsagePerDaySeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Starch Usage",
                    Values = values,
                    Fill = areaFill,
                    Stroke = System.Windows.Media.Brushes.Cyan,
                    StrokeThickness = 4,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 12,
                    PointForeground = System.Windows.Media.Brushes.Cyan
                }
            };
        }

        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}
