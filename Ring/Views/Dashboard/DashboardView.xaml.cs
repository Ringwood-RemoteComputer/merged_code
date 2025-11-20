using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Ring.Services;
using Ring.ViewModels;
using Ring.Views.UserControls;

namespace Ring.Views.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        private DashboardService _dashboardService;
        private DashboardGraphViewModel _graphViewModel;
        private List<DashboardAlarm> _recentAlarms;
        private DispatcherTimer _dateTimeTimer;

        public DashboardView()
        {
            InitializeComponent();
            InitializeDashboard();
            
            // Set alarm data source and initialize toggle state after XAML is loaded
            Loaded += (s, e) =>
            {
                SetAlarmDataSource();
                InitializeToggleState();
                InitializeGraphToggleState();
                InitializeDateTimeTimer();
                UpdateSimulationButtonState();
            };
        }

        private void InitializeDashboard()
        {
            // Initialize the dashboard service
            _dashboardService = new DashboardService();
            
            // Initialize the graph view model
            _graphViewModel = new DashboardGraphViewModel();
            
            // Load placeholder alarm data
            LoadRecentAlarms();
            
            // Set the data context
            DataContext = _dashboardService;
            
            // Initialize graph controls
            InitializeGraphControls();
            
            // Subscribe to navigation events
            _dashboardService.NavigationRequested += OnNavigationRequested;
        }

        private void InitializeGraphControls()
        {
            // Create and configure the Batches Per Shift chart control
            var batchesChart = new BatchesPerShiftChart();
            batchesChart.DataContext = _graphViewModel;
            
            // Create and configure the Starch Usage chart control
            var starchChart = new StarchUsageChart();
            starchChart.DataContext = _graphViewModel;
            
            // Set the controls as the content for the panels
            if (BatchesGraphPanel != null)
            {
                BatchesGraphPanel.Child = batchesChart;
            }
            
            if (StarchGraphPanel != null)
            {
                StarchGraphPanel.Child = starchChart;
            }
        }

        private void LoadRecentAlarms()
        {
            // Placeholder alarm data for dashboard (top 5 most recent)
            _recentAlarms = new List<DashboardAlarm>
            {
                new DashboardAlarm 
                { 
                    Severity = "Warning", 
                    Source = "81", 
                    Description = "Bulk Caustic Tank Low Level", 
                    TimeAgo = "2m ago",
                    SeverityColor = "#FFC107"
                },
                new DashboardAlarm 
                { 
                    Severity = "Warning", 
                    Source = "21", 
                    Description = "Domestic Starch Low Level", 
                    TimeAgo = "8m ago",
                    SeverityColor = "#FFC107"
                },
                new DashboardAlarm 
                { 
                    Severity = "Alarm", 
                    Source = "2", 
                    Description = "On Hold for Batch Inspection", 
                    TimeAgo = "15m ago",
                    SeverityColor = "#DC3545"
                }
            };
        }

        private void SetAlarmDataSource()
        {
            // This method is called after XAML initialization
            if (AlarmListBox != null)
            {
                AlarmListBox.ItemsSource = _recentAlarms;
            }
        }

        private void InitializeToggleState()
        {
            // Set initial state: Storage Tanks selected (black text), Use Tanks unselected (gray text)
            UpdateToggleButtonStates(isStorageSelected: true);
        }

        private void InitializeGraphToggleState()
        {
            // Set initial state: Batches graph selected
            UpdateGraphToggleButtonStates(isBatchesSelected: true);
        }

        private void UpdateToggleButtonStates(bool isStorageSelected)
        {
            if (isStorageSelected)
            {
                // Storage Tanks is selected
                StorageTanksText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                StorageTanksText.FontWeight = FontWeights.Bold;
                StorageTanksPanel.Visibility = Visibility.Visible;

                // Use Tanks is unselected
                UseTanksText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#999999"));
                UseTanksText.FontWeight = FontWeights.SemiBold;
                UseTanksPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Use Tanks is selected
                UseTanksText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                UseTanksText.FontWeight = FontWeights.Bold;
                UseTanksPanel.Visibility = Visibility.Visible;

                // Storage Tanks is unselected
                StorageTanksText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#999999"));
                StorageTanksText.FontWeight = FontWeights.SemiBold;
                StorageTanksPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void OnNavigationRequested(object sender, string viewName)
        {
            // Handle navigation to specific views using the existing NavBar pattern
            try
            {
                var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
                if (mainWindow != null)
                {
                    var mainContentArea = mainWindow.FindName("MainContentArea") as ContentControl;
                    if (mainContentArea != null)
                    {
                        // Navigate to the requested view
                        switch (viewName)
                        {
                            case "MakeReadyTank":
                                var makeReadyTankControl = new Ring.Views.MakeReadyTank();
                                mainContentArea.Content = makeReadyTankControl;
                                break;
                            case "StorageTankGroup":
                                mainContentArea.Content = new Ring.Views.MainScreen.StorageTankGroup();
                                break;
                            case "TvcControl":
                                // For now, show a message since we need to determine which TVC view to show
                                MessageBox.Show("TVC Control navigation - will be implemented", "Navigation", MessageBoxButton.OK, MessageBoxImage.Information);
                                break;
                            case "UseTankGroup":
                                mainContentArea.Content = new Ring.Views.MainScreen.UseTankGroup();
                                break;
                            default:
                                MessageBox.Show($"Navigation to {viewName} not implemented yet", "Navigation", MessageBoxButton.OK, MessageBoxImage.Information);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to {viewName}: {ex.Message}", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Card Click Handlers
        private void MakeReadyTankCard_Click(object sender, MouseButtonEventArgs e)
        {
            _dashboardService.NavigateToMakeReadyTankCommand.Execute(null);
        }

        #endregion

        #region Toggle Button Handlers
        private void StorageTanksButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateToggleButtonStates(isStorageSelected: true);
        }

        private void UseTanksButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateToggleButtonStates(isStorageSelected: false);
        }

        private void BatchesGraphButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateGraphToggleButtonStates(isBatchesSelected: true);
        }

        private void StarchGraphButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateGraphToggleButtonStates(isBatchesSelected: false);
        }

        private void UpdateGraphToggleButtonStates(bool isBatchesSelected)
        {
            if (isBatchesSelected)
            {
                // Batches graph is selected
                BatchesGraphText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                BatchesGraphText.FontWeight = FontWeights.Bold;
                BatchesGraphPanel.Visibility = Visibility.Visible;

                // Starch graph is unselected
                StarchGraphText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#999999"));
                StarchGraphText.FontWeight = FontWeights.SemiBold;
                StarchGraphPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Starch graph is selected
                StarchGraphText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                StarchGraphText.FontWeight = FontWeights.Bold;
                StarchGraphPanel.Visibility = Visibility.Visible;

                // Batches graph is unselected
                BatchesGraphText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#999999"));
                BatchesGraphText.FontWeight = FontWeights.SemiBold;
                BatchesGraphPanel.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Alarm Handlers
        private void ViewAllAlarms_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
                if (mainWindow != null)
                {
                    var alarmWindow = new Ring.AlarmWindow();
                    alarmWindow.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to alarms: {ex.Message}", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Date/Time Timer
        private void InitializeDateTimeTimer()
        {
            _dateTimeTimer = new DispatcherTimer();
            _dateTimeTimer.Interval = TimeSpan.FromSeconds(1); // Update every second
            _dateTimeTimer.Tick += DateTimeTimer_Tick;
            _dateTimeTimer.Start();
            
            // Update immediately
            UpdateDateTime();
        }

        private void DateTimeTimer_Tick(object sender, EventArgs e)
        {
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            if (SystemTimeText != null)
            {
                SystemTimeText.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        #endregion

        #region Simulation Mode Toggle
        private void ToggleSimulationMode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
                if (mainWindow != null)
                {
                    // Call MainWindow's toggle method
                    mainWindow.ToggleSimulationMode_Click(sender, e);
                    
                    // Update button state after toggle
                    UpdateSimulationButtonState();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error toggling simulation mode: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSimulationButtonState()
        {
            try
            {
                var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
                if (mainWindow != null && SimulationToggleButton != null)
                {
                    bool isSimulationMode = mainWindow.IsSimulationMode;
                    
                    if (isSimulationMode)
                    {
                        SimulationToggleButton.Content = "Simulation Mode";
                        SimulationToggleButton.Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)); // Primary blue #2196F3
                        SimulationToggleButton.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        SimulationToggleButton.Content = "Live PLC Mode";
                        SimulationToggleButton.Background = new SolidColorBrush(Color.FromRgb(220, 53, 69)); // Red #DC3545
                        SimulationToggleButton.Foreground = new SolidColorBrush(Colors.White);
                    }
                }
            }
            catch (Exception ex)
            {
                // Silently fail - button state will update on next check
                Console.WriteLine($"Error updating simulation button state: {ex.Message}");
            }
        }
        #endregion

        #region Cleanup
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // Clean up resources
            if (_dateTimeTimer != null)
            {
                _dateTimeTimer.Stop();
                _dateTimeTimer = null;
            }
            
            if (_dashboardService != null)
            {
                _dashboardService.NavigationRequested -= OnNavigationRequested;
                _dashboardService.Dispose();
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents an alarm for dashboard display
    /// </summary>
    public class DashboardAlarm
    {
        public string Severity { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public string TimeAgo { get; set; }
        public string SeverityColor { get; set; }
    }
}

