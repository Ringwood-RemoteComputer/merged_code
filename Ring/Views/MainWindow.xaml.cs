using System;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Ring.Database;
using Ring.Views;
using Ring.Views.BatchStart;
using Ring.Views.Reports;
using Ring.Views.Shifts;
using Ring.Views.TVCcontrol;
using Ring.Services.PLC;
using Ring.Services.Alarms;
using System.Collections.Generic;

namespace Ring
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _plcUpdateTimer;
        private bool _isPlcMonitoringActive = false;
        
        // Simulation mode
        private bool _simulationMode = true; // Default to simulation mode
        
        // Setup menu authentication
        private bool _isSupervisorAuthenticated = false;
        private bool _isAdminAuthenticated = false;
        private Random _random = new Random();
        private double _simulatedWeight = 250.0; // Start with safe weight
        private double _simulatedTemperature = 45.0;
        private double _simulatedBorax = 15.0;
        
        // Alarm components
        private AlarmPopup _alarmPopup;
        private bool _alarmActive = false;
        private const double CRITICAL_WEIGHT_LIMIT = 500.0;
        
        // Alarm monitoring service (handles all Alarm_Triggers[0-895])
        private AlarmMonitorService _alarmMonitorService = null;
        private Dictionary<int, AlarmPopup> _activeAlarmPopups = new Dictionary<int, AlarmPopup>(); // Track popups by alarm index
        
        // Reusable PLC tag readers (to avoid creating new instances every second)
        private PlcTagReader _weightReader = null;
        private PlcTagReader _temperatureReader = null;
        private PlcTagReader _boraxCausticReader = null;

        public MainWindow()
        {
            InitializeComponent();
            //PlcDatabaseHelper.Initialize();
            
            // Initialize alarm database
            try
            {
                AlarmDatabaseHelper.CreateTable();
                Console.WriteLine("Alarm database initialized successfully.");
                
                // Test database connection on startup
                Console.WriteLine("Testing database connection...");
                AlarmDatabaseHelper.TestDatabaseConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not initialize alarm database: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            InitializePlcMonitoring();
            StartPlcMonitoring(); // Start PLC monitoring automatically
            
            // Initialize PLC tag readers (reusable instances)
            if (!_simulationMode)
            {
                try
                {
                    Console.WriteLine("Initializing PLC tag readers...");
                    _weightReader = new PlcTagReader("PC_Read_Float[30]", "192.168.202.10", PlcDataType.REAL);
                    _temperatureReader = new PlcTagReader("PC_Read_Float[31]", "192.168.202.10", PlcDataType.REAL);
                    _boraxCausticReader = new PlcTagReader("PC_Read_Float[32]", "192.168.202.10", PlcDataType.REAL);
                    Console.WriteLine("✓ PLC tag readers initialized successfully");
                    
                    // Initialize alarm monitoring service (monitors all Alarm_Triggers[0-895])
                    Console.WriteLine("Initializing Alarm Monitor Service...");
                    _alarmMonitorService = new AlarmMonitorService("192.168.202.10", "1,0");
                    _alarmMonitorService.AlarmTriggered += AlarmMonitorService_AlarmTriggered;
                    _alarmMonitorService.AlarmCleared += AlarmMonitorService_AlarmCleared;
                    Console.WriteLine("✓ Alarm Monitor Service initialized successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Warning: Could not initialize PLC tag readers: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }
                }
            }
            
            // Set initial button state for simulation mode
            var simulationButton = this.FindName("SimulationToggleButton") as Button;
            if (simulationButton != null)
            {
                simulationButton.Content = "Simulation Mode";
                simulationButton.Foreground = new SolidColorBrush(Color.FromRgb(26, 58, 138)); // Blue color
            }
            
            // PLC monitoring will read values (simulation or live PLC)
            Console.WriteLine($"MainWindow - PLC monitoring started in {(_simulationMode ? "SIMULATION" : "LIVE PLC")} mode");
        }

        private void InitializePlcMonitoring()
        {
            _plcUpdateTimer = new DispatcherTimer();
            _plcUpdateTimer.Interval = TimeSpan.FromMilliseconds(1000); // Update every 1 second
            _plcUpdateTimer.Tick += PlcUpdateTimer_Tick;
        }

        private void StartPlcMonitoring()
        {
            try
            {
                _plcUpdateTimer.Start();
                _isPlcMonitoringActive = true;
                
                // Perform initial read
                ReadPlcTags();
                
                // Log automatic start (no user message needed)
                Console.WriteLine("PLC monitoring started automatically - monitoring Make Ready Tank values");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start PLC monitoring: {ex.Message}", "PLC Monitoring Error", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PlcUpdateTimer_Tick(object sender, EventArgs e)
        {
            ReadPlcTags();
            CheckCriticalWeightAlarm(); // Check for alarm conditions after reading PLC data
        }

        private void ReadPlcTags()
        {
            try
            {
                string weightValue, temperatureValue, boraxCausticValue;
                
                if (_simulationMode)
                {
                    // Generate simulation data
                    GenerateSimulationData();
                    weightValue = _simulatedWeight.ToString("F1");
                    temperatureValue = _simulatedTemperature.ToString("F1");
                    boraxCausticValue = _simulatedBorax.ToString("F1");
                    
                    Console.WriteLine($"SIMULATION MODE - Weight: {weightValue} lbs, Temperature: {temperatureValue}°C, Borax: {boraxCausticValue} lbs");
                    
                    // Note: Alarm monitoring is disabled in simulation mode
                    // In live mode, AlarmMonitorService handles all alarms
                }
                else
                {
                    // Read from live PLC using reusable readers
                    if (_weightReader == null || _temperatureReader == null || _boraxCausticReader == null)
                    {
                        // Initialize readers if not already done
                        _weightReader = new PlcTagReader("PC_Read_Float[30]", "192.168.202.10", PlcDataType.REAL);
                        _temperatureReader = new PlcTagReader("PC_Read_Float[31]", "192.168.202.10", PlcDataType.REAL);
                        _boraxCausticReader = new PlcTagReader("PC_Read_Float[32]", "192.168.202.10", PlcDataType.REAL);
                    }
                    
                    weightValue = _weightReader.Read();
                    temperatureValue = _temperatureReader.Read();
                    boraxCausticValue = _boraxCausticReader.Read();
                    
                    // Check if reads were successful (not "Error")
                    if (weightValue == "Error" || temperatureValue == "Error" || boraxCausticValue == "Error")
                    {
                        Console.WriteLine($"PLC Read Error - Weight: {weightValue}, Temperature: {temperatureValue}, Borax: {boraxCausticValue}");
                        UpdateCommunicationStatus(false, DateTime.Now);
                        UpdateMakeReadyTankStatus("Error", "Error", "Error");
                        return; // Exit early on error
                    }
                    
                    Console.WriteLine($"LIVE PLC MODE - Weight: {weightValue} lbs, Temperature: {temperatureValue}°C, Borax: {boraxCausticValue} lbs");
                    
                    // Check all alarms using AlarmMonitorService
                    if (_alarmMonitorService != null)
                    {
                        try
                        {
                            var triggeredAlarms = _alarmMonitorService.CheckAllAlarms();
                            if (triggeredAlarms.Count > 0)
                            {
                                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {triggeredAlarms.Count} alarm(s) triggered");
                            }
                        }
                        catch (Exception alarmEx)
                        {
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ✗ Error checking alarms: {alarmEx.Message}");
                        }
                    }
                }
                
                
                // Store values for Make Ready Tank (only if we have valid values)
                if (!_simulationMode)
                {
                    if (weightValue != "Error" && temperatureValue != "Error" && boraxCausticValue != "Error")
                    {
                        StorePlcValuesForMakeReadyTank(weightValue, temperatureValue, boraxCausticValue);
                    }
                }
                else
                {
                    StorePlcValuesForMakeReadyTank(weightValue, temperatureValue, boraxCausticValue);
                }
                
                // Update UI elements
                if (!_simulationMode)
                {
                    if (weightValue != "Error" && temperatureValue != "Error" && boraxCausticValue != "Error")
                    {
                        UpdateCommunicationStatus(true, DateTime.Now);
                        UpdateMakeReadyTankStatus(weightValue, temperatureValue, boraxCausticValue);
                    }
                    else
                    {
                        UpdateCommunicationStatus(false, DateTime.Now);
                    }
                }
                else
                {
                    UpdateCommunicationStatus(true, DateTime.Now);
                    UpdateMakeReadyTankStatus(weightValue, temperatureValue, boraxCausticValue);
                }
                
                UpdateAlarmStatus();
                
                // Check for critical weight alarm in main window
                CheckCriticalWeightAlarm();
            }
            catch (Exception ex)
            {
                // Don't show error message for every failed read during monitoring
                // Just log it or update UI to show connection issue
                Console.WriteLine($"PLC Read Error: {ex.Message}");
                
                // Update UI to show error state
                UpdateCommunicationStatus(false, DateTime.Now);
                UpdateMakeReadyTankStatus("Error", "Error", "Error");
                UpdateAlarmStatus();
            }
        }
        
        /// <summary>
        /// Event handler for when an alarm is triggered by AlarmMonitorService
        /// </summary>
        private void AlarmMonitorService_AlarmTriggered(object sender, AlarmTriggeredEventArgs e)
        {
            try
            {
                var alarmInfo = e.AlarmInfo;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] *** ALARM TRIGGERED: Alarm_Triggers[{alarmInfo.AlarmIndex}] - {alarmInfo.AlarmName} ***");
                
                // Save alarm to database
                var alarm = new AlarmRecord
                {
                    ALMNUMBER = alarmInfo.AlarmIndex,
                    ALMTIME = DateTime.Now.ToString("HH:mm:ss"),
                    ALMDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                    ACKTIME = null,
                    ACKDATE = null,
                    ALMTYPENUMBER = alarmInfo.AlarmType == AlarmType.Alarm ? 1 : 2, // 1 = Alarm, 2 = Warning
                    ALMSTATUSNUMBER = 1, // 1 = Unacknowledged
                    ALMIDTYPE = 1,
                    ALMNAME = alarmInfo.AlarmName
                };
                
                try
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Attempting to save alarm to database...");
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Alarm details: ALMNUMBER={alarm.ALMNUMBER}, ALMNAME='{alarm.ALMNAME}', ALMDATE={alarm.ALMDATE}, ALMTIME={alarm.ALMTIME}");
                    
                    AlarmDatabaseHelper.InsertAlarm(alarm);
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ✓✓✓ Alarm saved to database successfully: {alarmInfo.AlarmName} ✓✓✓");
                }
                catch (Exception dbEx)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ✗✗✗ ERROR saving alarm to database: {dbEx.Message} ✗✗✗");
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Stack trace: {dbEx.StackTrace}");
                    if (dbEx.InnerException != null)
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Inner exception: {dbEx.InnerException.Message}");
                    }
                }
                
                // Show alarm popup
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        // Close existing popup for this alarm if open
                        if (_activeAlarmPopups.ContainsKey(alarmInfo.AlarmIndex))
                        {
                            _activeAlarmPopups[alarmInfo.AlarmIndex].Close();
                            _activeAlarmPopups.Remove(alarmInfo.AlarmIndex);
                        }
                        
                        // Create and show alarm popup
                        string popupMessage = alarmInfo.AlarmIndex == 1 
                            ? "Emergency Stop has been activated. All operations have been halted."
                            : $"Alarm {alarmInfo.AlarmIndex} has been triggered.";
                        var popup = new AlarmPopup(alarmInfo.AlarmName, popupMessage);
                        popup.AlarmClosed += (s, args) =>
                        {
                            _activeAlarmPopups.Remove(alarmInfo.AlarmIndex);
                            UpdateAlarmStatus();
                        };
                        
                        _activeAlarmPopups[alarmInfo.AlarmIndex] = popup;
                        popup.Show();
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Alarm popup shown: {alarmInfo.AlarmName}");
                    }
                    catch (Exception popupEx)
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Error showing popup: {popupEx.Message}");
                    }
                });
                
                // Update alarm status display
                UpdateAlarmStatus();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ✗ ERROR in AlarmMonitorService_AlarmTriggered: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Event handler for when an alarm is cleared by AlarmMonitorService
        /// </summary>
        private void AlarmMonitorService_AlarmCleared(object sender, AlarmClearedEventArgs e)
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Alarm cleared: Alarm_Triggers[{e.AlarmIndex}]");
                
                // Close popup if open
                Dispatcher.Invoke(() =>
                {
                    if (_activeAlarmPopups.ContainsKey(e.AlarmIndex))
                    {
                        _activeAlarmPopups[e.AlarmIndex].Close();
                        _activeAlarmPopups.Remove(e.AlarmIndex);
                    }
                });
                
                // Update alarm status display
                UpdateAlarmStatus();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ✗ ERROR in AlarmMonitorService_AlarmCleared: {ex.Message}");
            }
        }
        
        // Old ShowEstopAlarmPopup method removed - now handled by AlarmMonitorService_AlarmTriggered
        
        private void GenerateSimulationData()
        {
            // Simulate realistic tank behavior
            // Weight: Gradually increase with some variation, occasionally spike above 500 for alarm testing
            double weightChange = (_random.NextDouble() - 0.5) * 10; // -5 to +5 lbs variation
            _simulatedWeight += weightChange;
            
            // Keep weight within realistic bounds (100-600 lbs)
            if (_simulatedWeight < 100) _simulatedWeight = 100;
            if (_simulatedWeight > 600) _simulatedWeight = 600;
            
            // Occasionally create alarm condition for testing (10% chance)
            if (_random.NextDouble() < 0.1)
            {
                _simulatedWeight = 500 + _random.NextDouble() * 100; // 500-600 lbs for alarm
                Console.WriteLine("SIMULATION: Creating alarm condition for testing");
            }
            
            // Temperature: Gradual changes with small variations
            double tempChange = (_random.NextDouble() - 0.5) * 2; // -1 to +1°C variation
            _simulatedTemperature += tempChange;
            
            // Keep temperature within realistic bounds (30-60°C)
            if (_simulatedTemperature < 30) _simulatedTemperature = 30;
            if (_simulatedTemperature > 60) _simulatedTemperature = 60;
            
            // Borax: Gradual changes with small variations
            double boraxChange = (_random.NextDouble() - 0.5) * 1; // -0.5 to +0.5 lbs variation
            _simulatedBorax += boraxChange;
            
            // Keep borax within realistic bounds (5-25 lbs)
            if (_simulatedBorax < 5) _simulatedBorax = 5;
            if (_simulatedBorax > 25) _simulatedBorax = 25;
        }
        
        // Store PLC values for Make Ready Tank to access
        private void StorePlcValuesForMakeReadyTank(string weightValue, string temperatureValue, string boraxCausticValue)
        {
            // Store values in a way that Make Ready Tank can access them
            try
            {
                Console.WriteLine($"MainWindow - Storing PLC values: Weight='{weightValue}', Temp='{temperatureValue}', Borax='{boraxCausticValue}'");
                
                if (double.TryParse(weightValue, out double weight))
                {
                    MakeReadyTankPlcData.Weight = weight;
                    Console.WriteLine($"MainWindow - Stored Weight: {weight}");
                }
                else
                {
                    Console.WriteLine($"MainWindow - Failed to parse weight: '{weightValue}'");
                }
                
                if (double.TryParse(temperatureValue, out double temperature))
                {
                    MakeReadyTankPlcData.Temperature = temperature;
                    Console.WriteLine($"MainWindow - Stored Temperature: {temperature}");
                }
                else
                {
                    Console.WriteLine($"MainWindow - Failed to parse temperature: '{temperatureValue}'");
                }
                
                if (double.TryParse(boraxCausticValue, out double boraxCaustic))
                {
                    MakeReadyTankPlcData.BoraxCausticWeight = boraxCaustic;
                    Console.WriteLine($"MainWindow - Stored BoraxCaustic: {boraxCaustic}");
                }
                else
                {
                    Console.WriteLine($"MainWindow - Failed to parse borax caustic: '{boraxCausticValue}'");
                }
                
                Console.WriteLine($"MainWindow - Data freshness: {MakeReadyTankPlcData.IsDataFresh}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error storing PLC values: {ex.Message}");
            }
        }
        
        // Check for critical weight alarm in main window
        private void CheckCriticalWeightAlarm()
        {
            double currentWeight = MakeReadyTankPlcData.Weight;
            Console.WriteLine($"MainWindow - Checking alarm: Weight={currentWeight:F1}, Limit={CRITICAL_WEIGHT_LIMIT}, AlarmActive={_alarmActive}");
            
            // Check if weight exceeds critical limit
            if (currentWeight > CRITICAL_WEIGHT_LIMIT)
            {
                if (!_alarmActive)
                {
                    // Trigger critical alarm
                    _alarmActive = true;
                    ShowCriticalAlarm();
                    Console.WriteLine($"MainWindow - Triggering new alarm for weight {currentWeight:F1}");
                }
                else if (_alarmPopup != null)
                {
                    // Update existing alarm with current weight
                    _alarmPopup.UpdateWeight(currentWeight);
                    Console.WriteLine($"MainWindow - Updating existing alarm with weight {currentWeight:F1}");
                }
            }
            else if (currentWeight <= CRITICAL_WEIGHT_LIMIT && _alarmActive)
            {
                // Weight is back to safe levels - update alarm popup
                Console.WriteLine($"MainWindow - Weight {currentWeight:F1} is now safe, updating alarm popup");
                if (_alarmPopup != null)
                {
                    _alarmPopup.UpdateWeight(currentWeight);
                    Console.WriteLine($"MainWindow - Called UpdateWeight with {currentWeight:F1} - popup should auto-close in 2 seconds");
                }
                _alarmActive = false;
            }
        }
        
        // Show critical alarm popup from main window
        private void ShowCriticalAlarm()
        {
            try
            {
                _alarmPopup = new AlarmPopup(MakeReadyTankPlcData.Weight);
                _alarmPopup.AlarmClosed += (sender, e) => {
                    _alarmPopup = null; // Clear reference when alarm closes
                    Console.WriteLine("Alarm popup closed");
                };
                _alarmPopup.Show();
                Console.WriteLine($"CRITICAL ALARM: Weight {MakeReadyTankPlcData.Weight:F1} lbs exceeds limit of {CRITICAL_WEIGHT_LIMIT} lbs");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error showing critical alarm: {ex.Message}");
            }
        }
        
        // Update Communication Status UI
        private void UpdateCommunicationStatus(bool isConnected, DateTime lastUpdate)
        {
            // Update LastUpdateText if it exists
            var lastUpdateText = this.FindName("LastUpdateText") as TextBlock;
            if (lastUpdateText != null)
            {
                lastUpdateText.Text = $"Last update: {lastUpdate:HH:mm:ss}";
            }
            
            // Update PLC status indicator
            var plcStatusIndicator = this.FindName("PlcStatusIndicator") as System.Windows.Shapes.Ellipse;
            var plcStatusText = this.FindName("PlcStatusText") as TextBlock;
            
            if (plcStatusIndicator != null && plcStatusText != null)
            {
                if (isConnected)
                {
                    plcStatusIndicator.Fill = new SolidColorBrush(Color.FromRgb(16, 185, 129)); // Green #10B981
                    plcStatusText.Text = "PLC Connected";
                    plcStatusText.Foreground = new SolidColorBrush(Color.FromRgb(16, 185, 129));
                }
                else
                {
                    plcStatusIndicator.Fill = new SolidColorBrush(Color.FromRgb(239, 68, 68)); // Red #EF4444
                    plcStatusText.Text = "PLC Disconnected";
                    plcStatusText.Foreground = new SolidColorBrush(Color.FromRgb(239, 68, 68));
                }
            }
            
            // Log to console for debugging
            Console.WriteLine($"PLC Status: {(isConnected ? "Connected" : "Disconnected")} - {lastUpdate:HH:mm:ss}");
        }
        
        // Update Make Ready Tank Status UI
        private void UpdateMakeReadyTankStatus(string weight, string temperature, string borax)
        {
            // Update Weight
            var weightText = this.FindName("TankWeightText") as TextBlock;
            if (weightText != null)
            {
                weightText.Text = $"Weight: {weight} lbs";
            }
            
            // Update Temperature
            var temperatureText = this.FindName("TankTemperatureText") as TextBlock;
            if (temperatureText != null)
            {
                temperatureText.Text = $"Temperature: {temperature} °C";
            }
            
            // Update Borax/Caustic
            var boraxText = this.FindName("TankBoraxText") as TextBlock;
            if (boraxText != null)
            {
                boraxText.Text = $"Borax/Caustic: {borax} lbs";
            }
            
            // Log to console for debugging
            Console.WriteLine($"Tank Status - Weight: {weight} lbs, Temperature: {temperature}°C, Borax: {borax} lbs");
        }
        
        // Update Alarm Status UI
        private void UpdateAlarmStatus()
        {
            var alarmStatusText = this.FindName("AlarmStatusText") as TextBlock;
            var alarmStatusBorder = this.FindName("AlarmStatusBorder") as Border;
            var alarmCountText = this.FindName("AlarmCountText") as TextBlock;
            var alarmLastText = this.FindName("AlarmLastText") as TextBlock;
            
            if (alarmStatusText != null && alarmStatusBorder != null && alarmCountText != null)
            {
                // Get active alarm count from AlarmMonitorService (if available) or use legacy flags
                int activeAlarmCount = 0;
                bool hasActiveAlarm = false;
                
                if (_alarmMonitorService != null && !_simulationMode)
                {
                    activeAlarmCount = _alarmMonitorService.GetActiveAlarmCount();
                    hasActiveAlarm = activeAlarmCount > 0;
                }
                else
                {
                    // Fallback to legacy alarm flags (for weight alarm and simulation mode)
                    hasActiveAlarm = _alarmActive;
                    activeAlarmCount = _alarmActive ? 1 : 0;
                }
                
                // Also include weight alarm if active
                if (_alarmActive)
                {
                    hasActiveAlarm = true;
                    activeAlarmCount++;
                }
                
                if (hasActiveAlarm)
                {
                    // Active alarm state - red
                    alarmStatusText.Text = "Active Alarm";
                    alarmStatusBorder.Background = new SolidColorBrush(Color.FromRgb(239, 68, 68)); // Red #EF4444
                    alarmCountText.Text = $"Active: {activeAlarmCount}";
                    
                    if (alarmLastText != null)
                    {
                        // Get the most recent active alarm name
                        string lastAlarmName = "Unknown";
                        if (_alarmMonitorService != null && !_simulationMode)
                        {
                            var activeIndices = _alarmMonitorService.GetActiveAlarmIndices();
                            if (activeIndices.Count > 0)
                            {
                                var alarmMapping = new AlarmMappingService();
                                lastAlarmName = alarmMapping.GetAlarmDescription(activeIndices[0]);
                            }
                        }
                        
                        if (_alarmActive)
                        {
                            alarmLastText.Text = $"Last: Weight > {CRITICAL_WEIGHT_LIMIT} lbs";
                        }
                        else if (lastAlarmName != "Unknown")
                        {
                            alarmLastText.Text = $"Last: {lastAlarmName}";
                        }
                        else
                        {
                            alarmLastText.Text = "Last: Active Alarm";
                        }
                        alarmLastText.Foreground = new SolidColorBrush(Color.FromRgb(239, 68, 68));
                    }
                }
                else
                {
                    // No active alarms - green
                    alarmStatusText.Text = "No Active Alarms";
                    alarmStatusBorder.Background = new SolidColorBrush(Color.FromRgb(16, 185, 129)); // Green #10B981
                    alarmCountText.Text = "Active: 0";
                    
                    if (alarmLastText != null)
                    {
                        alarmLastText.Text = "Last: None";
                        alarmLastText.Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)); // Gray #666
                    }
                }
            }
            
            // Log to console for debugging
            int totalActive = (_alarmMonitorService != null && !_simulationMode) ? _alarmMonitorService.GetActiveAlarmCount() : 0;
            if (_alarmActive) totalActive++;
            Console.WriteLine($"Alarm Status: {(totalActive > 0 ? $"Active ({totalActive})" : "No Active Alarms")}");
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Event handlers for Menu Items

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Report clicked.");
        }

        private void GenerateBatchReport_Click(object sender, RoutedEventArgs e)
        {
            var batchReportWindow = new BatchReportWindow();
            var window = new Window
            {
                Title = "Batch Report Generator",
                Content = batchReportWindow,
                Width = 1000,
                Height = 700,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.CanResize
            };
            window.ShowDialog();
        }

        private void TVCControl_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TVC Control clicked.");
        }

        private void Hold_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hold clicked.");
        }

        private void BatchStart_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Batch Start clicked.");
        }

        private void Shifts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Add prominent clicked state
                var shiftsButton = this.FindName("ShiftsButton") as Button;
                if (shiftsButton != null)
                {
                    // Highlight the button with a bright background
                    shiftsButton.Background = new SolidColorBrush(Color.FromRgb(0, 150, 255)); // Bright blue
                    shiftsButton.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255)); // White border
                    shiftsButton.BorderThickness = new Thickness(2);
                    
                    // Reset other menu buttons to normal state
                    ResetAllMenuButtons();
                    
                    // Keep this button highlighted
                    shiftsButton.Background = new SolidColorBrush(Color.FromRgb(0, 150, 255));
                    shiftsButton.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    shiftsButton.BorderThickness = new Thickness(2);
                }
                
                var shiftsWindow = new ShiftControlWindow();
                shiftsWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Shifts: {ex.Message}", "Error", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetAllMenuButtons()
        {
            // Reset all menu buttons to normal state
            var menuButtons = new[] { "DashboardButton", "ProcessButton", "MainScreenButton", "ReportsButton", 
                                    "BatchStartButton", "TVCControlButton", "UseTanksButton", "ShiftsButton", 
                                    "LanguageButton", "SetupButton", "WindowButton", "HelpButton" };
            
            foreach (var buttonName in menuButtons)
            {
                var button = this.FindName(buttonName) as Button;
                if (button != null)
                {
                    button.Background = new SolidColorBrush(Colors.Transparent);
                    button.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    button.BorderThickness = new Thickness(0);
                }
            }
        }

        private void Setup_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Setup clicked.");
        }

        private void Window_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Window clicked.");
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Help clicked.");
        }

        private void Alarms_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var alarmWindow = new AlarmWindow();
                alarmWindow.Show(); // Opens the AlarmWindow (non-modal so it can refresh)
                Console.WriteLine("[MainWindow] AlarmWindow opened");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainWindow] Error opening AlarmWindow: {ex.Message}");
                MessageBox.Show($"Error opening Alarms window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MainScreen_Click(object sender, RoutedEventArgs e)
        {
            var makeReadyTankControl = new Ring.Views.MakeReadyTank();
            var mainContentArea = FindName("MainContentArea") as ContentControl;
            if (mainContentArea != null)
            {
                mainContentArea.Content = makeReadyTankControl;
            }
        }

        private void BatchReport_Click(object sender, RoutedEventArgs e)
        {
            var batchReportWindow = new BatchReportWindow();
            batchReportWindow.ShowDialog(); // Opens the Batch Report window as a modal dialog
        }

        private void UsageReport_Click(object sender, RoutedEventArgs e)
        {
            var usageReportWindow = new UsageReportWindow();
            usageReportWindow.ShowDialog(); // Opens the Usage Report window as a modal dialog
        }

        private void AlarmHistory_Click(object sender, RoutedEventArgs e)
        {
            var alarmHistoryWindow = new AlarmHistory();
            alarmHistoryWindow.ShowDialog(); // Opens the Usage Report window as a modal dialog
        }

        private void BatchHistoryReport_Click(object sender, RoutedEventArgs e)
        {
            var batchHistoryReport = new BatchHistoryReport();
            batchHistoryReport.ShowDialog(); // Opens the window as a modal dialog
        }

        private void BatchHistoryUsageReport_Click(object sender, RoutedEventArgs e)
        {
            var batchHistoryUsageReport = new BatchHistoryUsageReport();
            batchHistoryUsageReport.ShowDialog(); // Opens the window as a modal dialog
        }

        private void InventoryReport_Click(object sender, RoutedEventArgs e)
        {
            var inventoryReport = new InventoryReport();
            inventoryReport.ShowDialog(); // Opens the window as a modal dialog
        }

        private void FormulaReport_Click(object sender, RoutedEventArgs e)
        {
            var formulaReport = new FormulaReport();
            formulaReport.ShowDialog(); // Opens the window as a modal dialog
        }

        private void ShiftConsumption_Click(object sender, RoutedEventArgs e)
        {
            var shiftConsumption = new ShiftConsumption();
            shiftConsumption.ShowDialog(); // Opens the window as a modal dialog
        }

        private void BatchQuery_Click(object sender, RoutedEventArgs e)
        {
            var batchQueryWindow = new BatchQueryView();
            batchQueryWindow.ShowDialog(); // Opens the Batch Query Manager window as a modal dialog
        }

        private void DatabaseTest_Click(object sender, RoutedEventArgs e)
        {
            // Temporarily disabled - DatabaseTestWindow has XAML compilation issues
            MessageBox.Show("Database Test window is temporarily disabled due to XAML compilation issues.", 
                          "Feature Disabled", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Information);
        }

        private void ReadTags_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var boolReader = new PlcTagReader("PC_Display_Outputs[0]", "192.168.202.10", PlcDataType.BOOL);
                var boolValue = boolReader.Read();
                Console.WriteLine($"BOOL Tag: {boolValue}");
                //PlcDatabaseHelper.InsertTagValue("Alarm_ons1", boolValue);

                var dintReader = new PlcTagReader("PC_Read_Integer[71].1", "192.168.202.10", PlcDataType.DINT);
                var dintValue = dintReader.Read();
                Console.WriteLine($"DINT Tag: {dintValue}");
                //PlcDatabaseHelper.InsertTagValue("Alarm_block", dintValue);

                var realReader = new PlcTagReader("PC_Read_Float[32]", "192.168.202.10", PlcDataType.REAL);
                var realValue = realReader.Read();
                Console.WriteLine($"REAL Tag: {realValue}");
                //PlcDatabaseHelper.InsertTagValue("BC_Scale", realValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to read from PLC: {ex.Message}", "PLC Read Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WriteToPlc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // --- EXAMPLE A: write a single BOOL bit (e.g., a DINT bit alias like PC_Write_Integer[0].0) ---
                // If you're targeting a single bit of a DINT, use PlcDataType.BOOL and the .0 suffix.
                //var bitWriter = new PlcTagWriter("PC_Write_Integer[0].0", "192.168.202.10", PlcDataType.BOOL);
                //bitWriter.Write("true");  // or "1"

                //// --- EXAMPLE B: write an entire DINT element ---
                //// If you want to write the whole word, drop the .0 and use DINT.
                //var dintWriter = new PlcTagWriter("PC_Write_Integer[0]", "192.168.202.10", PlcDataType.DINT);
                //dintWriter.Write("42");

                // --- EXAMPLE C: write a REAL (float) ---
                var realWriter = new PlcTagWriter("PC_Read_Float[32]", "192.168.202.10", PlcDataType.REAL);
                realWriter.Write("78.65");

                // Optionally update UI or status message here
            }
            catch (Exception ex)
            {
                MessageBox.Show($"PLC write failed: {ex.Message}", "Write Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StorageGroup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // --- EXAMPLE A: write a single BOOL bit (e.g., a DINT bit alias like PC_Write_Integer[0].0) ---
                // If you're targeting a single bit of a DINT, use PlcDataType.BOOL and the .0 suffix.
                //var bitWriter = new PlcTagWriter("PC_Write_Integer[0].0", "192.168.202.10", PlcDataType.BOOL);
                //bitWriter.Write("true");  // or "1"

                //// --- EXAMPLE B: write an entire DINT element ---
                //// If you want to write the whole word, drop the .0 and use DINT.
                //var dintWriter = new PlcTagWriter("PC_Write_Integer[0]", "192.168.202.10", PlcDataType.DINT);
                //dintWriter.Write("42");

                // --- EXAMPLE C: write a REAL (float) ---
                var realWriter = new PlcTagWriter("PC_Read_Float[32]", "192.168.202.10", PlcDataType.REAL);
                realWriter.Write("78.65");

                // Optionally update UI or status message here
            }
            catch (Exception ex)
            {
                MessageBox.Show($"PLC write failed: {ex.Message}", "Write Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Additional click handlers for the new menu items
        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Dashboard view
            var mainContentArea = this.FindName("MainContentArea") as ContentControl;
            if (mainContentArea != null)
            {
                mainContentArea.Content = new Ring.Views.Dashboard.DashboardView();
            }
        }

        private void Process_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Process clicked.");
        }

        private void UseTanks_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Use Tank Group view
            var mainContentArea = this.FindName("MainContentArea") as ContentControl;
            if (mainContentArea != null)
            {
                mainContentArea.Content = new Ring.Views.MainScreen.UseTankGroup();
            }
        }

        private void Language_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Language clicked.");
        }

        private void TankMonitoring_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var makeReadyTankControl = new Ring.Views.MakeReadyTank();
                var mainContentArea = FindName("MainContentArea") as ContentControl;
                if (mainContentArea != null)
                {
                    mainContentArea.Content = makeReadyTankControl;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Make Ready Tank: {ex.Message}", "Error", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Password_Click(object sender, RoutedEventArgs e)
        {
            var passwordDialog = new Ring.Views.PasswordDialog
            {
                Owner = this
            };

            bool? result = passwordDialog.ShowDialog();
            
            if (result == true && passwordDialog.IsPasswordCorrect)
            {
                if (passwordDialog.PasswordType == "Supervisor")
                {
                    _isSupervisorAuthenticated = true;
                    _isAdminAuthenticated = false; // Reset admin if supervisor is entered
                    ShowSetupMenuItems();
                    MessageBox.Show("Supervisor password accepted! Setup menu items are now available.", 
                                  "Password Verified", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Information);
                }
                else if (passwordDialog.PasswordType == "Admin")
                {
                    _isAdminAuthenticated = true;
                    _isSupervisorAuthenticated = true; // Admin also has supervisor access
                    ShowSetupMenuItems();
                    MessageBox.Show("Admin password accepted! All setup menu items are now available.", 
                                  "Password Verified", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Information);
                }
            }
        }

        private void SystemStatus_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("System Status:\n\n" +
                          "• PLC Communication: Active\n" +
                          "• Make Ready Tank: Monitoring\n" +
                          "• Alarms: None Active\n" +
                          "• System Health: Good", 
                          "System Status", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Information);
        }

        private void StartPlcMonitoring_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isPlcMonitoringActive)
                {
                    _plcUpdateTimer.Stop();
                    _isPlcMonitoringActive = false;
                    MessageBox.Show("PLC monitoring stopped.", "PLC Monitoring", 
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    StartPlcMonitoring();
                    MessageBox.Show("PLC monitoring started successfully!", "PLC Monitoring", 
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to control PLC monitoring: {ex.Message}\n\nNote: Make sure PLC is connected and accessible at 192.168.202.10", 
                               "PLC Connection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ToggleSimulationMode_Click(object sender, RoutedEventArgs e)
        {
            _simulationMode = !_simulationMode;
            
            // Reinitialize PLC readers when switching to live mode
            if (!_simulationMode)
            {
                try
                {
                    _weightReader = new PlcTagReader("PC_Read_Float[30]", "192.168.202.10", PlcDataType.REAL);
                    _temperatureReader = new PlcTagReader("PC_Read_Float[31]", "192.168.202.10", PlcDataType.REAL);
                    _boraxCausticReader = new PlcTagReader("PC_Read_Float[32]", "192.168.202.10", PlcDataType.REAL);
                    Console.WriteLine("PLC tag readers reinitialized for Live PLC mode.");
                    
                    // Initialize alarm monitoring service
                    if (_alarmMonitorService == null)
                    {
                        Console.WriteLine("Initializing Alarm Monitor Service...");
                        _alarmMonitorService = new AlarmMonitorService("192.168.202.10", "1,0");
                        _alarmMonitorService.AlarmTriggered += AlarmMonitorService_AlarmTriggered;
                        _alarmMonitorService.AlarmCleared += AlarmMonitorService_AlarmCleared;
                        Console.WriteLine("✓ Alarm Monitor Service initialized");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing PLC readers: {ex.Message}");
                }
            }
            else
            {
                // Clear readers when switching to simulation mode
                _weightReader = null;
                _temperatureReader = null;
                _boraxCausticReader = null;
                
                // Dispose alarm monitor service
                if (_alarmMonitorService != null)
                {
                    _alarmMonitorService.AlarmTriggered -= AlarmMonitorService_AlarmTriggered;
                    _alarmMonitorService.AlarmCleared -= AlarmMonitorService_AlarmCleared;
                    _alarmMonitorService.Dispose();
                    _alarmMonitorService = null;
                    Console.WriteLine("Alarm Monitor Service disposed (simulation mode)");
                }
            }
            
            var button = sender as Button;
            if (button != null)
            {
                if (_simulationMode)
                {
                    button.Content = "Simulation Mode";
                    button.Foreground = new SolidColorBrush(Color.FromRgb(26, 58, 138)); // Blue color
                    Console.WriteLine("Switched to SIMULATION MODE - Generating realistic test data");
                }
                else
                {
                    button.Content = "Live PLC Mode";
                    button.Foreground = new SolidColorBrush(Color.FromRgb(220, 38, 38)); // Red color
                    Console.WriteLine("Switched to LIVE PLC MODE - Reading from actual PLC at 192.168.202.10");
                }
            }
            
            MessageBox.Show($"Switched to {(_simulationMode ? "Simulation" : "Live PLC")} mode.\n\n" +
                          (_simulationMode ? 
                           "• Generating realistic test data\n• Will occasionally trigger alarms for testing\n• No PLC connection required" :
                           "• Reading from live PLC at 192.168.202.10\n• Requires PLC connection\n• Real production data"), 
                          "Mode Changed", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowSetupMenuItems()
        {
            // Supervisor-level items (shown when supervisor password is entered)
            var supervisorButtons = new[]
            {
                "MakeReadyTankInventoryEditButton",
                "MakeReadyTankFormulaEditButton",
                "MakeReadyTankFormulaExchangeButton",
                "MakeReadyTankNamesEditButton",
                "StorageGroupSystemNamesEditButton",
                "ShiftNamesAndSpansButton",
                "EditPasswordsButton",
                "CommunicationDiskLoggingButton",
                "UpdateProcessorTimeDateButton"
            };

            // Admin-level items (shown only when admin password is entered)
            var adminButtons = new[]
            {
                "MixersButton",
                "DistributionButton",
                "ViscometerButton",
                "GroupsButton",
                "TanksButton",
                "DataEntryButton",
                "AlarmsButton",
                "InventoryButton",
                "MixerBatchFormulaDataButton",
                "DistributionBatchDataButton",
                "ShiftsAdminButton",
                "PasswordSubmenuButton",
                "CommunicationButton",
                "ImportExportSetupDatabaseButton"
            };

            // Show supervisor items if supervisor is authenticated
            if (_isSupervisorAuthenticated)
            {
                foreach (var buttonName in supervisorButtons)
                {
                    var button = this.FindName(buttonName) as Button;
                    if (button != null)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                }
            }

            // Show admin items if admin is authenticated
            if (_isAdminAuthenticated)
            {
                // Admin gets all supervisor items plus admin items
                foreach (var buttonName in supervisorButtons)
                {
                    var button = this.FindName(buttonName) as Button;
                    if (button != null)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                }

                foreach (var buttonName in adminButtons)
                {
                    var button = this.FindName(buttonName) as Button;
                    if (button != null)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void HideSetupMenuItems()
        {
            // Hide all protected setup menu items (supervisor + admin)
            var allButtons = new[]
            {
                "MakeReadyTankInventoryEditButton",
                "MakeReadyTankFormulaEditButton",
                "MakeReadyTankFormulaExchangeButton",
                "MakeReadyTankNamesEditButton",
                "StorageGroupSystemNamesEditButton",
                "ShiftNamesAndSpansButton",
                "EditPasswordsButton",
                "CommunicationDiskLoggingButton",
                "UpdateProcessorTimeDateButton",
                "MixersButton",
                "DistributionButton",
                "ViscometerButton",
                "GroupsButton",
                "TanksButton",
                "DataEntryButton",
                "AlarmsButton",
                "InventoryButton",
                "MixerBatchFormulaDataButton",
                "DistributionBatchDataButton",
                "ShiftsAdminButton",
                "PasswordSubmenuButton",
                "CommunicationButton",
                "ImportExportSetupDatabaseButton"
            };

            foreach (var buttonName in allButtons)
            {
                var button = this.FindName(buttonName) as Button;
                if (button != null)
                {
                    button.Visibility = Visibility.Collapsed;
                }
            }

            // Also hide all submenus
            var submenuNames = new[]
            {
                "ShiftNamesAndSpansSubmenu",
                "CommunicationDiskLoggingSubmenu",
                "MixersSubmenu",
                "DistributionSubmenu",
                "ViscometerSubmenu",
                "GroupsSubmenu",
                "TanksSubmenu",
                "DataEntrySubmenu",
                "AlarmsAdminSubmenu",
                "InventorySubmenu",
                "MixerBatchFormulaDataSubmenu",
                "DistributionBatchDataSubmenu",
                "ShiftsAdminSubmenu",
                "PasswordSubmenu",
                "CommunicationSubmenu",
                "ImportExportSetupDatabaseSubmenu"
            };

            foreach (var submenuName in submenuNames)
            {
                var submenu = this.FindName(submenuName) as Border;
                if (submenu != null) submenu.Visibility = Visibility.Collapsed;
            }
        }

        private void MainContent_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Close all submenus when clicking on main content
            HideAllSubmenus();
        }

        // Setup menu item click handlers
        private void MakeReadyTankInventoryEdit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Make Ready Tank Inventory Edit - Feature to be implemented", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MakeReadyTankFormulaEdit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Make Ready Tank Formula Edit - Feature to be implemented", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MakeReadyTankFormulaExchange_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Make Ready Tank Formula Exchange - Feature to be implemented", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MakeReadyTankNamesEdit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Make Ready Tank Names Edit - Feature to be implemented", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StorageGroupSystemNamesEdit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Storage Group System Names Edit - Feature to be implemented", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShiftNamesAndSpans_Click(object sender, RoutedEventArgs e)
        {
            // Submenu is handled by hover events
        }

        private void ShiftNamesAndSpans_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubmenu("ShiftNamesAndSpansSubmenu");
        }

        private void ShiftNamesAndSpans_MouseLeave(object sender, MouseEventArgs e)
        {
            // Submenu will stay open until user clicks elsewhere
        }

        private void EditPasswords_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Edit Passwords - Feature to be implemented", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CommunicationDiskLogging_Click(object sender, RoutedEventArgs e)
        {
            // Submenu is handled by hover events
        }

        private void CommunicationDiskLogging_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubmenu("CommunicationDiskLoggingSubmenu");
        }

        private void CommunicationDiskLogging_MouseLeave(object sender, MouseEventArgs e)
        {
            // Submenu will stay open until user clicks elsewhere
        }

        private void UpdateProcessorTimeDate_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Update Processor Time/Date - Feature to be implemented", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Admin menu item click handlers
        private void Mixers_Click(object sender, RoutedEventArgs e) { }
        private void Mixers_MouseEnter(object sender, MouseEventArgs e) { ShowSubmenu("MixersSubmenu"); }
        private void Mixers_MouseLeave(object sender, MouseEventArgs e) { }

        private void Distribution_Click(object sender, RoutedEventArgs e) { }
        private void Distribution_MouseEnter(object sender, MouseEventArgs e) { ShowSubmenu("DistributionSubmenu"); }
        private void Distribution_MouseLeave(object sender, MouseEventArgs e) { }

        private void Viscometer_Click(object sender, RoutedEventArgs e) { }
        private void Viscometer_MouseEnter(object sender, MouseEventArgs e) { ShowSubmenu("ViscometerSubmenu"); }
        private void Viscometer_MouseLeave(object sender, MouseEventArgs e) { }

        private void Groups_Click(object sender, RoutedEventArgs e) { }
        private void Groups_MouseEnter(object sender, MouseEventArgs e) { ShowSubmenu("GroupsSubmenu"); }
        private void Groups_MouseLeave(object sender, MouseEventArgs e) { }

        private void Tanks_Click(object sender, RoutedEventArgs e) { }
        private void Tanks_MouseEnter(object sender, MouseEventArgs e) { ShowSubmenu("TanksSubmenu"); }
        private void Tanks_MouseLeave(object sender, MouseEventArgs e) { }

        private void DataEntry_Click(object sender, RoutedEventArgs e) { }
        private void DataEntry_MouseEnter(object sender, MouseEventArgs e) { ShowSubmenu("DataEntrySubmenu"); }
        private void DataEntry_MouseLeave(object sender, MouseEventArgs e) { }

        private void AlarmsAdmin_Click(object sender, RoutedEventArgs e) { }
        private void AlarmsAdmin_MouseEnter(object sender, MouseEventArgs e) { ShowSubmenu("AlarmsAdminSubmenu"); }
        private void AlarmsAdmin_MouseLeave(object sender, MouseEventArgs e) { }

        private void Inventory_Click(object sender, RoutedEventArgs e) { }
        private void Inventory_MouseEnter(object sender, MouseEventArgs e) { ShowSubmenu("InventorySubmenu"); }
        private void Inventory_MouseLeave(object sender, MouseEventArgs e) { }

        private void MixerBatchFormulaData_Click(object sender, RoutedEventArgs e) { }
        private void MixerBatchFormulaData_MouseEnter(object sender, MouseEventArgs e) { ShowSubmenu("MixerBatchFormulaDataSubmenu"); }
        private void MixerBatchFormulaData_MouseLeave(object sender, MouseEventArgs e) { }

        private void DistributionBatchData_Click(object sender, RoutedEventArgs e) { }
        private void DistributionBatchData_MouseEnter(object sender, MouseEventArgs e) { ShowSubmenu("DistributionBatchDataSubmenu"); }
        private void DistributionBatchData_MouseLeave(object sender, MouseEventArgs e) { }

        private void ShiftsAdmin_Click(object sender, RoutedEventArgs e) { }
        private void ShiftsAdmin_MouseEnter(object sender, MouseEventArgs e) { ShowSubmenu("ShiftsAdminSubmenu"); }
        private void ShiftsAdmin_MouseLeave(object sender, MouseEventArgs e) { }

        private void PasswordSubmenu_Click(object sender, RoutedEventArgs e) { }
        private void PasswordSubmenu_MouseEnter(object sender, MouseEventArgs e) { ShowSubmenu("PasswordSubmenu"); }
        private void PasswordSubmenu_MouseLeave(object sender, MouseEventArgs e) { }

        private void Communication_Click(object sender, RoutedEventArgs e) { }
        private void Communication_MouseEnter(object sender, MouseEventArgs e) { ShowSubmenu("CommunicationSubmenu"); }
        private void Communication_MouseLeave(object sender, MouseEventArgs e) { }

        private void ImportExportSetupDatabase_Click(object sender, RoutedEventArgs e) { }
        private void ImportExportSetupDatabase_MouseEnter(object sender, MouseEventArgs e) { ShowSubmenu("ImportExportSetupDatabaseSubmenu"); }
        private void ImportExportSetupDatabase_MouseLeave(object sender, MouseEventArgs e) { }

        // Submenu management
        private string _currentSubmenu = null;

        // Submenu hover event handlers
        private void Process_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubmenu("ProcessSubmenu");
        }

        private void Process_MouseLeave(object sender, MouseEventArgs e)
        {
            // Submenu will stay open until user clicks elsewhere
        }

        private void MainScreen_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubmenu("MainScreenSubmenu");
        }

        private void MainScreen_MouseLeave(object sender, MouseEventArgs e)
        {
            // Submenu will stay open until user clicks elsewhere
        }

        private void Reports_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubmenu("ReportsSubmenu");
        }

        private void Reports_MouseLeave(object sender, MouseEventArgs e)
        {
            // Submenu will stay open until user clicks elsewhere
        }

        private void BatchStart_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubmenu("BatchStartSubmenu");
        }

        private void BatchStart_MouseLeave(object sender, MouseEventArgs e)
        {
            // Submenu will stay open until user clicks elsewhere
        }

        private void TVCControl_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubmenu("TVCControlSubmenu");
        }

        private void TVCControl_MouseLeave(object sender, MouseEventArgs e)
        {
            // Submenu will stay open until user clicks elsewhere
        }

        private void Shifts_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubmenu("ShiftsSubmenu");
        }

        private void Shifts_MouseLeave(object sender, MouseEventArgs e)
        {
            // Submenu will stay open until user clicks elsewhere
        }

        private void Setup_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubmenu("SetupSubmenu");
        }

        private void Setup_MouseLeave(object sender, MouseEventArgs e)
        {
            // Submenu will stay open until user clicks elsewhere
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubmenu("WindowSubmenu");
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            // Submenu will stay open until user clicks elsewhere
        }

        private void Help_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubmenu("HelpSubmenu");
        }

        private void Help_MouseLeave(object sender, MouseEventArgs e)
        {
            // Submenu will stay open until user clicks elsewhere
        }

        private void ShowSubmenu(string submenuName)
        {
            // Hide any currently open submenu
            if (_currentSubmenu != null && _currentSubmenu != submenuName)
            {
                HideSubmenu(_currentSubmenu);
            }

            _currentSubmenu = submenuName;
            var submenu = this.FindName(submenuName) as Border;
            if (submenu != null)
            {
                submenu.Visibility = Visibility.Visible;
                submenu.Opacity = 0;
                submenu.RenderTransform = new TranslateTransform(-10, 0);
                
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150));
                var slideIn = new DoubleAnimation(-10, 0, TimeSpan.FromMilliseconds(150));
                
                submenu.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                submenu.RenderTransform.BeginAnimation(TranslateTransform.XProperty, slideIn);
            }
        }

        private void HideSubmenu(string submenuName)
        {
            var submenu = this.FindName(submenuName) as Border;
            if (submenu != null)
            {
                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150));
                var slideOut = new DoubleAnimation(0, -10, TimeSpan.FromMilliseconds(150));
                
                fadeOut.Completed += (s, e) => submenu.Visibility = Visibility.Collapsed;
                
                submenu.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                submenu.RenderTransform.BeginAnimation(TranslateTransform.XProperty, slideOut);
            }
        }

        private void HideAllSubmenus()
        {
            if (_currentSubmenu != null)
            {
                HideSubmenu(_currentSubmenu);
                _currentSubmenu = null;
            }
        }

        // Additional click handlers for submenu items
        private void StorageTank1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Storage Tank 1 clicked.");
        }

        private void StorageTank2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Storage Tank 2 clicked.");
        }

        private void LowMidtank_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("LOW/MID D/B TANK clicked.");
        }

        private void StorageTank4_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Storage Tank 4 clicked.");
        }

        private void TVCStorageTank1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TVC Storage Tank 1 clicked.");
        }

        private void TVCStorageTank2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TVC Storage Tank 2 clicked.");
        }

        private void TVClowmidtank_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TVC LOW/MID D/B TANK clicked.");
        }

        private void TVCStorageTank4_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TVC Storage Tank 4 clicked.");
        }

        private void ShiftControl_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Shift Control clicked.");
        }

        private void Shift1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Shift 1 clicked.");
        }

        private void Shift2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Shift 2 clicked.");
        }

        private void Shift3_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Shift 3 clicked.");
        }
    }
}