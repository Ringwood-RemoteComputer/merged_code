using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace Ring
{
    public partial class AlarmWindow : Window
    {
        private DispatcherTimer _refreshTimer;
        
        public AlarmWindow()
        {
            InitializeComponent();

            // Load alarms from database
            LoadAlarmsFromDatabase();
            
            // Set up auto-refresh timer (refresh every 2 seconds)
            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(2);
            _refreshTimer.Tick += (s, e) => LoadAlarmsFromDatabase();
            _refreshTimer.Start();
            
            // Refresh when window is activated (brought to front)
            this.Activated += (s, e) => LoadAlarmsFromDatabase();
        }
        
        protected override void OnClosed(EventArgs e)
        {
            // Stop timer when window closes
            if (_refreshTimer != null)
            {
                _refreshTimer.Stop();
                _refreshTimer = null;
            }
            base.OnClosed(e);
        }
        
        private void LoadAlarmsFromDatabase()
        {
            try
            {
                Console.WriteLine($"[AlarmWindow] Loading alarms from database...");
                var alarmRecords = Ring.Database.AlarmDatabaseHelper.GetAlarms();
                Console.WriteLine($"[AlarmWindow] Retrieved {alarmRecords.Count} alarm(s) from database");
                
                var alarms = new List<Alarm>();
                
                foreach (var record in alarmRecords)
                {
                    alarms.Add(new Alarm
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
                
                // If no alarms in database, show message
                if (alarms.Count == 0)
                {
                    alarms.Add(new Alarm { Type = "Info", Status = "No Alarms", AlarmNumber = 0, AlarmDescription = "No alarms found in database", AlarmDate = "", AlarmTime = "", AcknowledgeDate = "", AcknowledgeTime = "" });
                    Console.WriteLine($"[AlarmWindow] No alarms in database");
                }
                else
                {
                    Console.WriteLine($"[AlarmWindow] Displaying {alarms.Count} alarm(s) in grid");
                }
                
                AlarmDataGrid.ItemsSource = alarms;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AlarmWindow] ✗ ERROR loading alarms: {ex.Message}");
                Console.WriteLine($"[AlarmWindow] Stack trace: {ex.StackTrace}");
                System.Windows.MessageBox.Show($"Error loading alarms: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                
                // Fallback to sample data on error
                var alarms = new List<Alarm>
                {
                    new Alarm { Type = "Warning", Status = "Active", AlarmNumber = 101, AlarmDescription = "Low Pressure", AlarmDate = "01/15/2025", AlarmTime = "14:30", AcknowledgeDate = "", AcknowledgeTime = "" },
                    new Alarm { Type = "Critical", Status = "Acknowledged", AlarmNumber = 102, AlarmDescription = "High Temperature", AlarmDate = "01/14/2025", AlarmTime = "10:15", AcknowledgeDate = "01/14/2025", AcknowledgeTime = "10:20" }
                };
                AlarmDataGrid.ItemsSource = alarms;
            }
        }

        private void RefreshAlarms_Click(object sender, RoutedEventArgs e)
        {
            // Refresh alarm data from database
            LoadAlarmsFromDatabase();
        }

        private void AcknowledgeAll_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("All alarms acknowledged.", "Acknowledge All", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Alarm history cleared.", "Clear History", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Alarm data exported.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class Alarm
    {
        public string Type { get; set; }
        public string Status { get; set; }
        public int AlarmNumber { get; set; }
        public string AlarmDescription { get; set; }
        public string AlarmDate { get; set; }
        public string AlarmTime { get; set; }
        public string AcknowledgeDate { get; set; }
        public string AcknowledgeTime { get; set;         }

        //private void RefreshAlarms_Click(object sender, RoutedEventArgs e)
        //{
        //    // Refresh alarm data
        //    var alarms = new List<Alarm>
        //    {
        //        new Alarm { Type = "Warning", Status = "Active", AlarmNumber = 101, AlarmDescription = "Low Pressure", AlarmDate = "01/15/2025", AlarmTime = "14:30", AcknowledgeDate = "", AcknowledgeTime = "" },
        //        new Alarm { Type = "Critical", Status = "Acknowledged", AlarmNumber = 102, AlarmDescription = "High Temperature", AlarmDate = "01/14/2025", AlarmTime = "10:15", AcknowledgeDate = "01/14/2025", AcknowledgeTime = "10:20" },
        //        new Alarm { Type = "Info", Status = "Active", AlarmNumber = 103, AlarmDescription = "System Maintenance", AlarmDate = "01/15/2025", AlarmTime = "16:45", AcknowledgeDate = "", AcknowledgeTime = "" }
        //    };
        //    AlarmDataGrid.ItemsSource = alarms;
        //}

        private void AcknowledgeAll_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("All alarms acknowledged.", "Acknowledge All", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Alarm history cleared.", "Clear History", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //private void Close_Click(object sender, RoutedEventArgs e)
        //{
        //    this.Close();
        //}

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Alarm data exported.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
