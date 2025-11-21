using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
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
            
            // Populate component grid (will be updated with PLC data in Step 9)
            PopulateMakeReadyComponentGrid();
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

        /// <summary>
        /// Shows the component details popup for Make Ready Tank
        /// </summary>
        private void ShowComponentsButtonMakeReady_Click(object sender, RoutedEventArgs e)
        {
            ComponentPopupMakeReady.IsOpen = true;
        }

        /// <summary>
        /// Populates the component grid for Make Ready Tank
        /// This will be updated in Step 9 to use PLC data from MakeReadyTankPlcData
        /// </summary>
        private void PopulateMakeReadyComponentGrid()
        {
            // Component data: Name, Status (true = On/Open/High, false = Off/Closed/Low)
            // For now using static data - will be updated in Step 9 to use PLC data
            var components = new[]
            {
                ("Agitator", true),
                ("Secondary agitator", false),
                ("Temperature probe", true),
                ("Transfer valve", false),
                ("Transfer pump", false),
                ("Domestic starch auger", true),
                ("Domestic starch valve", true),
                ("Modified starch valve", false),
                ("Modified starch auger", false),
                ("Liquid caustic valve", true),
                ("Borax valve", false),
                ("Borax feeder motor", true),
                ("Liquid borax pump", false),
                ("Liquid borax valve", false),
                ("Steam valve", true),
                ("Fast water valve", true),
                ("Slow water valve", false),
                ("Additive #1 valve", false),
                ("Additive #1 pump", false),
                ("Additive #2 valve", true),
                ("Additive #2 pump", true),
                ("Additive #3 valve", false),
                ("Additive #3 pump", false)
            };

            for (int i = 0; i < components.Length; i++)
            {
                var (componentName, isOn) = components[i];
                var row = i / 2;  // 2 columns per row
                var col = i % 2;

                // Create component container
                var border = new Border
                {
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(4),
                    Margin = new Thickness(4),
                    Padding = new Thickness(10, 8, 10, 8),
                    ToolTip = $"{componentName} - {(isOn ? "On" : "Off")}"
                };

                Grid.SetRow(border, row);
                Grid.SetColumn(border, col);

                // Create grid for label and status light alignment
                var innerGrid = new Grid();
                innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Component name (left-aligned)
                var textBlock = new TextBlock
                {
                    Text = componentName,
                    FontSize = 13,
                    FontWeight = FontWeights.Normal,
                    Foreground = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                    Margin = new Thickness(0, 0, 10, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    TextWrapping = TextWrapping.NoWrap
                };
                System.Windows.Controls.Grid.SetColumn(textBlock, 0);

                // Status indicator (right-aligned)
                var statusLight = new Ellipse
                {
                    Width = 14,
                    Height = 14,
                    Fill = isOn 
                        ? new SolidColorBrush(Color.FromRgb(76, 175, 80))  // Green
                        : new SolidColorBrush(Color.FromRgb(244, 67, 54)), // Red
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                System.Windows.Controls.Grid.SetColumn(statusLight, 1);

                innerGrid.Children.Add(textBlock);
                innerGrid.Children.Add(statusLight);
                border.Child = innerGrid;

                ComponentGridMakeReady.Children.Add(border);
            }
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

