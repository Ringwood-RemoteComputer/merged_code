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
        
        // Public property to access simulation mode state
        public bool IsSimulationMode
        {
            get => _simulationMode;
            private set => _simulationMode = value;
        }
        
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


        // Navigation is now handled by NavBar UserControl

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

        // Navigation is now handled by NavBar UserControl

        // Navigation is now handled by NavBar UserControl

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

        // Make this method public so Dashboard can call it
        public void ToggleSimulationMode_Click(object sender, RoutedEventArgs e)
        {
            IsSimulationMode = !IsSimulationMode;
            
            // Reinitialize PLC readers when switching to live mode
            if (!IsSimulationMode)
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
            
            // Button state is now managed by Dashboard, so we don't update it here
            if (IsSimulationMode)
            {
                Console.WriteLine("Switched to SIMULATION MODE - Generating realistic test data");
            }
            else
            {
                Console.WriteLine("Switched to LIVE PLC MODE - Reading from actual PLC at 192.168.202.10");
            }
            
            MessageBox.Show($"Switched to {(IsSimulationMode ? "Simulation" : "Live PLC")} mode.\n\n" +
                          (IsSimulationMode ? 
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
            // Close all NavBar popups when clicking on main content
            if (NavBar != null)
            {
                NavBar.CloseAllPopupsPublic();
            }
        }

        // Navigation is now handled by NavBar UserControl
    }
}