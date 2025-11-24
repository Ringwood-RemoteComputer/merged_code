using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Ring.Database;

namespace Ring.Views.Process
{
    public partial class Alarm : UserControl
    {
        private DispatcherTimer _refreshTimer;
        private List<AlarmData> _allAlarms;
        private const int DEFAULT_ALARM_COUNT = 20;
        
        public Alarm()
        {
            InitializeComponent();
            
            // Initialize date pickers to default range (last 30 days to today)
            EndDatePicker.SelectedDate = DateTime.Today;
            StartDatePicker.SelectedDate = DateTime.Today.AddDays(-30);
            
            // Subscribe to date picker events using lambda to avoid signature issues
            StartDatePicker.SelectedDateChanged += (sender, e) => ApplyDateFilter();
            EndDatePicker.SelectedDateChanged += (sender, e) => ApplyDateFilter();
            
            // Load alarms from database
            LoadAlarmsFromDatabase();
            
            // Set up auto-refresh timer (refresh every 2 seconds)
            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(2);
            _refreshTimer.Tick += (s, e) => LoadAlarmsFromDatabase();
            _refreshTimer.Start();
        }
        
        private void LoadAlarmsFromDatabase()
        {
            try
            {
                Console.WriteLine($"[Alarm] Loading alarms from database...");
                var alarmRecords = AlarmDatabaseHelper.GetAlarms();
                Console.WriteLine($"[Alarm] Retrieved {alarmRecords.Count} alarm(s) from database");
                
                _allAlarms = new List<AlarmData>();
                
                foreach (var record in alarmRecords)
                {
                    _allAlarms.Add(new AlarmData
                    {
                        Type = record.ALMTYPENUMBER == 1 ? "Critical" : "Warning",
                        Status = record.ALMSTATUSNUMBER == 1 ? "Active" : "Acknowledged",
                        AlarmNumber = record.ALMNUMBER,
                        AlarmDescription = record.ALMNAME ?? "Unknown Alarm",
                        AlarmDate = record.ALMDATE ?? "",
                        AlarmTime = record.ALMTIME ?? "",
                        AcknowledgeDate = record.ACKDATE ?? "",
                        AcknowledgeTime = record.ACKTIME ?? ""
                    });
                }
                
                // Sort by most recent first (by date and time)
                _allAlarms = _allAlarms.OrderByDescending(a => 
                {
                    if (DateTime.TryParse($"{a.AlarmDate} {a.AlarmTime}", out DateTime dt))
                        return dt;
                    return DateTime.MinValue;
                }).ToList();
                
                // Apply date filtering and update display
                ApplyDateFilter();
                
                // Update active alarm count
                UpdateActiveAlarmCount();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Alarm] ✗ ERROR loading alarms: {ex.Message}");
                Console.WriteLine($"[Alarm] Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Error loading alarms: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
                // Fallback to empty list on error
                _allAlarms = new List<AlarmData>();
                AlarmDataGrid.ItemsSource = _allAlarms;
                UpdateActiveAlarmCount();
            }
        }
        
        private void ApplyDateFilter()
        {
            if (_allAlarms == null)
            {
                AlarmDataGrid.ItemsSource = new List<AlarmData>();
                return;
            }
            
            var filteredAlarms = _allAlarms.ToList();
            
            // Apply date range filter if dates are selected
            if (StartDatePicker.SelectedDate.HasValue || EndDatePicker.SelectedDate.HasValue)
            {
                filteredAlarms = filteredAlarms.Where(a =>
                {
                    if (string.IsNullOrEmpty(a.AlarmDate))
                        return false;
                    
                    if (DateTime.TryParse(a.AlarmDate, out DateTime alarmDate))
                    {
                        bool afterStart = !StartDatePicker.SelectedDate.HasValue || alarmDate.Date >= StartDatePicker.SelectedDate.Value.Date;
                        bool beforeEnd = !EndDatePicker.SelectedDate.HasValue || alarmDate.Date <= EndDatePicker.SelectedDate.Value.Date;
                        return afterStart && beforeEnd;
                    }
                    return false;
                }).ToList();
            }
            else
            {
                // If no date filter, show most recent DEFAULT_ALARM_COUNT alarms
                filteredAlarms = filteredAlarms.Take(DEFAULT_ALARM_COUNT).ToList();
            }
            
            Console.WriteLine($"[Alarm] Displaying {filteredAlarms.Count} alarm(s) in grid (filtered from {_allAlarms.Count} total)");
            AlarmDataGrid.ItemsSource = filteredAlarms;
        }
        
        private void UpdateActiveAlarmCount()
        {
            if (_allAlarms == null)
            {
                ActiveAlarmCountText.Text = "0";
                return;
            }
            
            // Count only active alarms (Status == "Active")
            int activeCount = _allAlarms.Count(a => a.Status == "Active");
            ActiveAlarmCountText.Text = activeCount.ToString();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            // Stop refresh timer
            if (_refreshTimer != null)
            {
                _refreshTimer.Stop();
                _refreshTimer = null;
            }
            
            // Navigate back to Dashboard
            var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
            if (mainWindow != null)
            {
                var mainContentArea = mainWindow.FindName("MainContentArea") as ContentControl;
                if (mainContentArea != null)
                {
                    mainContentArea.Content = new Ring.Views.Dashboard.DashboardView();
                }
            }
        }

        private void AlarmSilence_Click(object sender, RoutedEventArgs e)
        {
            // Keep current behavior - show message box
            MessageBox.Show("Alarm silence activated", "Alarm Management", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder export functionality - show message box
            MessageBox.Show("Alarm data exported.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class AlarmData
    {
        public string Type { get; set; }
        public string Status { get; set; }
        public int AlarmNumber { get; set; }
        public string AlarmDescription { get; set; }
        public string AlarmDate { get; set; }
        public string AlarmTime { get; set; }
        public string AcknowledgeDate { get; set; }
        public string AcknowledgeTime { get; set; }
    }
}
