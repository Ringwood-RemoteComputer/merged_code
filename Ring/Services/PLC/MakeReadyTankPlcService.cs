using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Ring.Services.PLC;

namespace Ring.Services.PLC
{
    /// <summary>
    /// PLC service for Make Ready Tank operations
    /// Connects to actual PLC tags for real-time monitoring and control
    /// </summary>
    public class MakeReadyTankPlcService : INotifyPropertyChanged, IDisposable
    {
        private readonly string _plcIpAddress;
        private readonly string _plcPath;
        private readonly DispatcherTimer _updateTimer;
        private bool _isConnected = false;
        private bool _simulationMode = false;

        // PLC Tag Readers for monitoring
        private readonly PlcTagReader _weightReader;
        private readonly PlcTagReader _temperatureReader;
        private readonly PlcTagReader _boraxCausticWeightReader;
        private readonly PlcTagReader _phLevelReader;
        private readonly PlcTagReader _tankLevelReader;
        private readonly PlcTagReader _agitatorStatusReader;
        private readonly PlcTagReader _heatingStatusReader;
        private readonly PlcTagReader _fillValveStatusReader;
        private readonly PlcTagReader _emergencyStopStatusReader;
        private readonly PlcTagReader _processStatusReader;
        private readonly PlcTagReader _formulaProgressReader;
        private readonly PlcTagReader _alarmStatusReader;

        // PLC Tag Writers for control
        private readonly PlcTagWriter _agitatorControlWriter;
        private readonly PlcTagWriter _heatingControlWriter;
        private readonly PlcTagWriter _fillValveControlWriter;
        private readonly PlcTagWriter _emergencyStopWriter;
        private readonly PlcTagWriter _processControlWriter;
        private readonly PlcTagWriter _formulaSelectionWriter;

        // Real-time data properties
        private double _weight = 0.0;
        private double _temperature = 0.0;
        private double _boraxCausticWeight = 0.0;
        private double _phLevel = 0.0;
        private double _tankLevel = 0.0;
        private bool _agitatorStatus = false;
        private bool _heatingStatus = false;
        private bool _fillValveStatus = false;
        private bool _emergencyStopStatus = false;
        private int _processStatus = 0;
        private double _formulaProgress = 0.0;
        private int _alarmStatus = 0;
        private DateTime _lastUpdateTime = DateTime.Now;
        private string _connectionStatus = "Disconnected";

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<string> ConnectionStatusChanged;
        public event EventHandler<string> PlcErrorOccurred;

        public MakeReadyTankPlcService(string plcIpAddress = "192.168.202.10", string plcPath = "1,0", bool simulationMode = false)
        {
            _plcIpAddress = plcIpAddress;
            _plcPath = plcPath;
            _simulationMode = simulationMode;

            // Initialize PLC Tag Readers for monitoring
            _weightReader = new PlcTagReader("MR_Tank_Weight", _plcIpAddress, PlcDataType.REAL, _plcPath);
            _temperatureReader = new PlcTagReader("MR_Tank_Temperature", _plcIpAddress, PlcDataType.REAL, _plcPath);
            _boraxCausticWeightReader = new PlcTagReader("MR_Borax_Caustic_Weight", _plcIpAddress, PlcDataType.REAL, _plcPath);
            _phLevelReader = new PlcTagReader("MR_Tank_PH", _plcIpAddress, PlcDataType.REAL, _plcPath);
            _tankLevelReader = new PlcTagReader("MR_Tank_Level", _plcIpAddress, PlcDataType.REAL, _plcPath);
            _agitatorStatusReader = new PlcTagReader("MR_Agitator_Status", _plcIpAddress, PlcDataType.BOOL, _plcPath);
            _heatingStatusReader = new PlcTagReader("MR_Heating_Status", _plcIpAddress, PlcDataType.BOOL, _plcPath);
            _fillValveStatusReader = new PlcTagReader("MR_Fill_Valve_Status", _plcIpAddress, PlcDataType.BOOL, _plcPath);
            _emergencyStopStatusReader = new PlcTagReader("MR_Emergency_Stop_Status", _plcIpAddress, PlcDataType.BOOL, _plcPath);
            _processStatusReader = new PlcTagReader("MR_Process_Status", _plcIpAddress, PlcDataType.DINT, _plcPath);
            _formulaProgressReader = new PlcTagReader("MR_Formula_Progress", _plcIpAddress, PlcDataType.REAL, _plcPath);
            _alarmStatusReader = new PlcTagReader("MR_Alarm_Status", _plcIpAddress, PlcDataType.DINT, _plcPath);

            // Initialize PLC Tag Writers for control
            _agitatorControlWriter = new PlcTagWriter("MR_Agitator_Control", _plcIpAddress, PlcDataType.BOOL, _plcPath);
            _heatingControlWriter = new PlcTagWriter("MR_Heating_Control", _plcIpAddress, PlcDataType.BOOL, _plcPath);
            _fillValveControlWriter = new PlcTagWriter("MR_Fill_Valve_Control", _plcIpAddress, PlcDataType.BOOL, _plcPath);
            _emergencyStopWriter = new PlcTagWriter("MR_Emergency_Stop_Control", _plcIpAddress, PlcDataType.BOOL, _plcPath);
            _processControlWriter = new PlcTagWriter("MR_Process_Control", _plcIpAddress, PlcDataType.DINT, _plcPath);
            _formulaSelectionWriter = new PlcTagWriter("MR_Formula_Selection", _plcIpAddress, PlcDataType.DINT, _plcPath);

            // Initialize update timer
            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromMilliseconds(500); // Update every 500ms
            _updateTimer.Tick += UpdateTimer_Tick;

            // Start connection
            Task.Run(async () => await ConnectAsync());
        }

        /// <summary>
        /// Connect to PLC and start monitoring
        /// </summary>
        public Task<bool> ConnectAsync()
        {
            try
            {
                if (_simulationMode)
                {
                    _isConnected = true;
                    _connectionStatus = "Connected (Simulation Mode)";
                    ConnectionStatusChanged?.Invoke(this, _connectionStatus);
                    StartMonitoring();
                    return Task.FromResult(true);
                }

                // Test connection by reading a simple tag
                var testValue = _weightReader.Read();
                if (testValue != "Error")
                {
                    _isConnected = true;
                    _connectionStatus = "Connected";
                    ConnectionStatusChanged?.Invoke(this, _connectionStatus);
                    StartMonitoring();
                    return Task.FromResult(true);
                }
                else
                {
                    _isConnected = false;
                    _connectionStatus = "Connection Failed";
                    ConnectionStatusChanged?.Invoke(this, _connectionStatus);
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                _isConnected = false;
                _connectionStatus = $"Connection Error: {ex.Message}";
                ConnectionStatusChanged?.Invoke(this, _connectionStatus);
                PlcErrorOccurred?.Invoke(this, ex.Message);
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Start monitoring PLC tags
        /// </summary>
        public void StartMonitoring()
        {
            if (_isConnected)
            {
                _updateTimer.Start();
            }
        }

        /// <summary>
        /// Stop monitoring PLC tags
        /// </summary>
        public void StopMonitoring()
        {
            _updateTimer.Stop();
        }

        /// <summary>
        /// Update timer event - reads all PLC tags
        /// </summary>
        private async void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (!_isConnected) return;

            try
            {
                await Task.Run(() => UpdateAllTags());
                LastUpdateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                PlcErrorOccurred?.Invoke(this, ex.Message);
            }
        }

        /// <summary>
        /// Update all PLC tags
        /// </summary>
        private void UpdateAllTags()
        {
            try
            {
                // Read all monitoring tags
                var weightStr = _weightReader.Read();
                var temperatureStr = _temperatureReader.Read();
                var boraxCausticWeightStr = _boraxCausticWeightReader.Read();
                var phLevelStr = _phLevelReader.Read();
                var tankLevelStr = _tankLevelReader.Read();
                var agitatorStatusStr = _agitatorStatusReader.Read();
                var heatingStatusStr = _heatingStatusReader.Read();
                var fillValveStatusStr = _fillValveStatusReader.Read();
                var emergencyStopStatusStr = _emergencyStopStatusReader.Read();
                var processStatusStr = _processStatusReader.Read();
                var formulaProgressStr = _formulaProgressReader.Read();
                var alarmStatusStr = _alarmStatusReader.Read();

                // Update properties with PLC data
                if (double.TryParse(weightStr, out double weight)) Weight = weight;
                if (double.TryParse(temperatureStr, out double temperature)) Temperature = temperature;
                if (double.TryParse(boraxCausticWeightStr, out double boraxCausticWeight)) BoraxCausticWeight = boraxCausticWeight;
                if (double.TryParse(phLevelStr, out double phLevel)) PhLevel = phLevel;
                if (double.TryParse(tankLevelStr, out double tankLevel)) TankLevel = tankLevel;
                if (bool.TryParse(agitatorStatusStr, out bool agitatorStatus)) AgitatorStatus = agitatorStatus;
                if (bool.TryParse(heatingStatusStr, out bool heatingStatus)) HeatingStatus = heatingStatus;
                if (bool.TryParse(fillValveStatusStr, out bool fillValveStatus)) FillValveStatus = fillValveStatus;
                if (bool.TryParse(emergencyStopStatusStr, out bool emergencyStopStatus)) EmergencyStopStatus = emergencyStopStatus;
                if (int.TryParse(processStatusStr, out int processStatus)) ProcessStatus = processStatus;
                if (double.TryParse(formulaProgressStr, out double formulaProgress)) FormulaProgress = formulaProgress;
                if (int.TryParse(alarmStatusStr, out int alarmStatus)) AlarmStatus = alarmStatus;
            }
            catch (Exception ex)
            {
                PlcErrorOccurred?.Invoke(this, $"Error updating tags: {ex.Message}");
            }
        }

        #region Control Methods

        /// <summary>
        /// Control agitator
        /// </summary>
        public bool SetAgitator(bool status)
        {
            try
            {
                if (_simulationMode)
                {
                    AgitatorStatus = status;
                    return true;
                }

                var result = _agitatorControlWriter.Write(status.ToString());
                if (result)
                {
                    AgitatorStatus = status;
                }
                return result;
            }
            catch (Exception ex)
            {
                PlcErrorOccurred?.Invoke(this, $"Error setting agitator: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Control heating system
        /// </summary>
        public bool SetHeating(bool status)
        {
            try
            {
                if (_simulationMode)
                {
                    HeatingStatus = status;
                    return true;
                }

                var result = _heatingControlWriter.Write(status.ToString());
                if (result)
                {
                    HeatingStatus = status;
                }
                return result;
            }
            catch (Exception ex)
            {
                PlcErrorOccurred?.Invoke(this, $"Error setting heating: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Control fill valve
        /// </summary>
        public bool SetFillValve(bool status)
        {
            try
            {
                if (_simulationMode)
                {
                    FillValveStatus = status;
                    return true;
                }

                var result = _fillValveControlWriter.Write(status.ToString());
                if (result)
                {
                    FillValveStatus = status;
                }
                return result;
            }
            catch (Exception ex)
            {
                PlcErrorOccurred?.Invoke(this, $"Error setting fill valve: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Activate emergency stop
        /// </summary>
        public bool ActivateEmergencyStop()
        {
            try
            {
                if (_simulationMode)
                {
                    EmergencyStopStatus = true;
                    return true;
                }

                var result = _emergencyStopWriter.Write("true");
                if (result)
                {
                    EmergencyStopStatus = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                PlcErrorOccurred?.Invoke(this, $"Error activating emergency stop: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Control process
        /// </summary>
        public bool SetProcess(int processCommand)
        {
            try
            {
                if (_simulationMode)
                {
                    ProcessStatus = processCommand;
                    return true;
                }

                var result = _processControlWriter.Write(processCommand.ToString());
                if (result)
                {
                    ProcessStatus = processCommand;
                }
                return result;
            }
            catch (Exception ex)
            {
                PlcErrorOccurred?.Invoke(this, $"Error setting process: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Select formula
        /// </summary>
        public bool SelectFormula(int formulaId)
        {
            try
            {
                if (_simulationMode)
                {
                    return true;
                }

                return _formulaSelectionWriter.Write(formulaId.ToString());
            }
            catch (Exception ex)
            {
                PlcErrorOccurred?.Invoke(this, $"Error selecting formula: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Properties

        public bool IsConnected => _isConnected;
        public bool SimulationMode => _simulationMode;
        public string ConnectionStatus => _connectionStatus;

        public double Weight
        {
            get => _weight;
            set
            {
                _weight = value;
                OnPropertyChanged();
            }
        }

        public double Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                OnPropertyChanged();
            }
        }

        public double BoraxCausticWeight
        {
            get => _boraxCausticWeight;
            set
            {
                _boraxCausticWeight = value;
                OnPropertyChanged();
            }
        }

        public double PhLevel
        {
            get => _phLevel;
            set
            {
                _phLevel = value;
                OnPropertyChanged();
            }
        }

        public double TankLevel
        {
            get => _tankLevel;
            set
            {
                _tankLevel = value;
                OnPropertyChanged();
            }
        }

        public bool AgitatorStatus
        {
            get => _agitatorStatus;
            set
            {
                _agitatorStatus = value;
                OnPropertyChanged();
            }
        }

        public bool HeatingStatus
        {
            get => _heatingStatus;
            set
            {
                _heatingStatus = value;
                OnPropertyChanged();
            }
        }

        public bool FillValveStatus
        {
            get => _fillValveStatus;
            set
            {
                _fillValveStatus = value;
                OnPropertyChanged();
            }
        }

        public bool EmergencyStopStatus
        {
            get => _emergencyStopStatus;
            set
            {
                _emergencyStopStatus = value;
                OnPropertyChanged();
            }
        }

        public int ProcessStatus
        {
            get => _processStatus;
            set
            {
                _processStatus = value;
                OnPropertyChanged();
            }
        }

        public double FormulaProgress
        {
            get => _formulaProgress;
            set
            {
                _formulaProgress = value;
                OnPropertyChanged();
            }
        }

        public int AlarmStatus
        {
            get => _alarmStatus;
            set
            {
                _alarmStatus = value;
                OnPropertyChanged();
            }
        }

        public DateTime LastUpdateTime
        {
            get => _lastUpdateTime;
            set
            {
                _lastUpdateTime = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            _updateTimer?.Stop();
        }

        #endregion
    }
}
