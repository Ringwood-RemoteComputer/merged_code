using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Ring.Database;
using Ring.Services.PLC;
using Ring.ViewModels.MainScreen;
using Ring.Views.Dashboard;

namespace Ring.Views.MainScreen
{
    public partial class MakeReadyTank : UserControl
    {
        private List<AlarmData> _alarms;
        private MakeReadyTankViewModel _viewModel;
        private DispatcherTimer _updateTimer;
        private DispatcherTimer _alarmRefreshTimer;
        private int _alarmRefreshCounter = 0;

        public MakeReadyTank()
        {
            InitializeComponent();
            _viewModel = new MakeReadyTankViewModel();
            DataContext = _viewModel;
            
            // Load alarms from database
            LoadAlarmData();
            
            // Start timer to update ViewModel from PLC data
            StartPlcDataUpdateTimer();
            
            // Start timer to refresh alarms periodically (every 5 seconds)
            StartAlarmRefreshTimer();
        }

        private void StartPlcDataUpdateTimer()
        {
            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromMilliseconds(500); // Update every 500ms
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        private void StartAlarmRefreshTimer()
        {
            _alarmRefreshTimer = new DispatcherTimer();
            _alarmRefreshTimer.Interval = TimeSpan.FromSeconds(5); // Refresh alarms every 5 seconds
            _alarmRefreshTimer.Tick += AlarmRefreshTimer_Tick;
            _alarmRefreshTimer.Start();
        }
        
        private void AlarmRefreshTimer_Tick(object sender, EventArgs e)
        {
            _alarmRefreshCounter++;
            // Refresh alarms every 5 seconds (counter increments every 5 seconds, so refresh every tick)
            LoadAlarmData();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Update ViewModel from MakeReadyTankPlcData
                if (MakeReadyTankPlcData.IsDataFresh)
                {
                    _viewModel.Weight = MakeReadyTankPlcData.Weight.ToString("F0");
                    _viewModel.Temperature = MakeReadyTankPlcData.Temperature.ToString("F0");
                    _viewModel.BoraxCausticWeight = MakeReadyTankPlcData.BoraxCausticWeight.ToString("F0");
                    
                    // Update PLC status
                    _viewModel.PlcStatusText = "PLC Connected";
                    _viewModel.PlcStatusBrush = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(40, 167, 69)); // Green
                }
                else
                {
                    // Data is stale
                    _viewModel.PlcStatusText = "PLC Disconnected";
                    _viewModel.PlcStatusBrush = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(220, 53, 69)); // Red
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating MakeReadyTank ViewModel: {ex.Message}");
            }
        }

        private void LoadAlarmData()
        {
            try
            {
                // Load alarms from database
                var alarmRecords = AlarmDatabaseHelper.GetAlarms();
                _alarms = new List<AlarmData>();
                
                foreach (var record in alarmRecords)
                {
                    _alarms.Add(new AlarmData
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
                
                // If no alarms in database, show empty list
                if (_alarms.Count == 0)
                {
                    _alarms = new List<AlarmData>();
                }
                
                // Sort by most recent first
                _alarms = _alarms.OrderByDescending(a => a.AlarmDate + " " + a.AlarmTime).ToList();
                
                // Update the DataGrid on the UI thread
                Dispatcher.Invoke(() =>
                {
                    var alarmGrid = this.FindName("AlarmDataGrid") as DataGrid;
                    if (alarmGrid != null)
                    {
                        alarmGrid.ItemsSource = null; // Clear first
                        alarmGrid.ItemsSource = _alarms; // Then set new data
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading alarms: {ex.Message}");
                // Fallback to empty list on error
                _alarms = new List<AlarmData>();
                Dispatcher.Invoke(() =>
                {
                    var alarmGrid = this.FindName("AlarmDataGrid") as DataGrid;
                    if (alarmGrid != null)
                    {
                        alarmGrid.ItemsSource = _alarms;
                    }
                });
            }
        }


        private void ProcessToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                if (button.Content.ToString() == "PAUSE PROCESS")
                {
                    button.Content = "RESUME PROCESS";
                    // Add logic here to pause the process
                }
                    else
                {
                    button.Content = "PAUSE PROCESS";
                    // Add logic here to resume the process
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            // Stop update timers
            if (_updateTimer != null)
            {
                _updateTimer.Stop();
                _updateTimer = null;
            }
            
            if (_alarmRefreshTimer != null)
            {
                _alarmRefreshTimer.Stop();
                _alarmRefreshTimer = null;
            }
            
            var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
            if (mainWindow != null)
            {
                var mainContentArea = mainWindow.FindName("MainContentArea") as ContentControl;
                if (mainContentArea != null)
                {
                    mainContentArea.Content = new DashboardView();
                }
            }
        }
        
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // Clean up timers when control is unloaded
            if (_updateTimer != null)
            {
                _updateTimer.Stop();
                _updateTimer = null;
            }
            
            if (_alarmRefreshTimer != null)
            {
                _alarmRefreshTimer.Stop();
                _alarmRefreshTimer = null;
            }
        }
        
        // Method to refresh alarms (can be called from a button if needed)
        public void RefreshAlarms()
        {
            LoadAlarmData();
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

