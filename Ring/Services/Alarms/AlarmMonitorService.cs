using System;
using System.Collections.Generic;
using System.Linq;
using libplctag;
using libplctag.DataTypes;
using Ring.Database;
using Ring.Services.PLC;

namespace Ring.Services.Alarms
{
    /// <summary>
    /// Monitors all Alarm_Triggers bits (0-895) and detects state changes.
    /// Reads DINTs efficiently (32 bits per DINT) and tracks previous state.
    /// </summary>
    public class AlarmMonitorService
    {
        private readonly string _plcIp;
        private readonly string _plcPath;
        private readonly AlarmMappingService _alarmMapping;
        
        // Track previous state: Dictionary<bitIndex, previousValue>
        private Dictionary<int, bool> _previousAlarmStates;
        
        // Track which DINT readers we need (896 bits / 32 = 28 DINTs)
        private const int TOTAL_ALARM_BITS = 896;
        private const int BITS_PER_DINT = 32;
        private const int TOTAL_DINTS = (TOTAL_ALARM_BITS + BITS_PER_DINT - 1) / BITS_PER_DINT; // 28 DINTs
        
        // Cache DINT readers for efficiency
        private Dictionary<int, PlcTagReader> _dintReaders;
        
        // Track active alarms: Dictionary<alarmIndex, isActive>
        private Dictionary<int, bool> _activeAlarms;
        
        public event EventHandler<AlarmTriggeredEventArgs> AlarmTriggered;
        public event EventHandler<AlarmClearedEventArgs> AlarmCleared;
        
        public AlarmMonitorService(string plcIp = "192.168.202.10", string plcPath = "1,0")
        {
            _plcIp = plcIp;
            _plcPath = plcPath;
            _alarmMapping = new AlarmMappingService();
            _previousAlarmStates = new Dictionary<int, bool>();
            _activeAlarms = new Dictionary<int, bool>();
            _dintReaders = new Dictionary<int, PlcTagReader>();
            
            // Initialize DINT readers for all required DINTs
            InitializeDintReaders();
        }
        
        private void InitializeDintReaders()
        {
            try
            {
                // Create readers for DINT[0] through DINT[27] (28 DINTs total)
                for (int dintIndex = 0; dintIndex < TOTAL_DINTS; dintIndex++)
                {
                    string dintTagName = $"Alarm_Triggers[{dintIndex}]";
                    _dintReaders[dintIndex] = new PlcTagReader(dintTagName, _plcIp, PlcDataType.DINT);
                    Console.WriteLine($"[AlarmMonitorService] Initialized reader for {dintTagName}");
                }
                Console.WriteLine($"[AlarmMonitorService] ✓ Initialized {TOTAL_DINTS} DINT readers for {TOTAL_ALARM_BITS} alarm bits");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AlarmMonitorService] ✗ Error initializing DINT readers: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Reads all alarm bits and detects state changes.
        /// Returns list of newly triggered alarms (changed from 0 to 1).
        /// </summary>
        public List<AlarmInfo> CheckAllAlarms()
        {
            List<AlarmInfo> triggeredAlarms = new List<AlarmInfo>();
            
            try
            {
                // Read all DINTs
                for (int dintIndex = 0; dintIndex < TOTAL_DINTS; dintIndex++)
                {
                    try
                    {
                        if (!_dintReaders.ContainsKey(dintIndex))
                        {
                            Console.WriteLine($"[AlarmMonitorService] Warning: DINT reader [{dintIndex}] not found, skipping");
                            continue;
                        }
                        
                        // Read the DINT value
                        string dintValueStr = _dintReaders[dintIndex].Read();
                        
                        if (dintValueStr == "Error" || string.IsNullOrEmpty(dintValueStr))
                        {
                            Console.WriteLine($"[AlarmMonitorService] Error reading Alarm_Triggers[{dintIndex}]: {dintValueStr}");
                            continue;
                        }
                        
                        int dintValue = int.Parse(dintValueStr);
                        
                        // Log DINT[0] reads for debugging Alarm_Triggers[1]
                        if (dintIndex == 0)
                        {
                            Console.WriteLine($"[AlarmMonitorService] DINT[0] = {dintValue} (binary: {Convert.ToString(dintValue, 2).PadLeft(32, '0')})");
                        }
                        
                        // Check each bit in this DINT (bits 0-31)
                        for (int bitPosition = 0; bitPosition < BITS_PER_DINT; bitPosition++)
                        {
                            int alarmIndex = (dintIndex * BITS_PER_DINT) + bitPosition;
                            
                            // Stop if we've exceeded total alarm bits
                            if (alarmIndex >= TOTAL_ALARM_BITS)
                                break;
                            
                            // Extract bit value
                            bool currentValue = (dintValue & (1 << bitPosition)) != 0;
                            
                            // Extra logging for Alarm_Triggers[1] (bit 1 of DINT[0])
                            if (alarmIndex == 1)
                            {
                                Console.WriteLine($"[AlarmMonitorService] Alarm_Triggers[1] check: DINT[0]={dintValue}, bit 1={currentValue}, previous={(_previousAlarmStates.ContainsKey(1) ? _previousAlarmStates[1].ToString() : "null")}");
                            }
                            
                            // Check if state changed
                            if (_previousAlarmStates.ContainsKey(alarmIndex))
                            {
                                bool previousValue = _previousAlarmStates[alarmIndex];
                                
                                // Alarm triggered: changed from 0 to 1
                                if (!previousValue && currentValue)
                                {
                                    Console.WriteLine($"[AlarmMonitorService] ✓✓✓ ALARM TRIGGERED: Alarm_Triggers[{alarmIndex}] changed from 0 to 1 ✓✓✓");
                                    
                                    // Get alarm name from mapping service
                                    string alarmName = _alarmMapping.GetAlarmDescription(alarmIndex);
                                    
                                    // Create alarm info
                                    var alarmInfo = new AlarmInfo
                                    {
                                        AlarmIndex = alarmIndex,
                                        AlarmName = alarmName,
                                        AlarmType = _alarmMapping.GetAlarmType(alarmIndex),
                                        TriggeredAt = DateTime.Now
                                    };
                                    
                                    triggeredAlarms.Add(alarmInfo);
                                    _activeAlarms[alarmIndex] = true;
                                    
                                    // Raise event
                                    AlarmTriggered?.Invoke(this, new AlarmTriggeredEventArgs(alarmInfo));
                                }
                                // Alarm cleared: changed from 1 to 0
                                else if (previousValue && !currentValue)
                                {
                                    Console.WriteLine($"[AlarmMonitorService] ✓ Alarm cleared: Alarm_Triggers[{alarmIndex}] changed from 1 to 0");
                                    _activeAlarms[alarmIndex] = false;
                                    
                                    // Raise event
                                    AlarmCleared?.Invoke(this, new AlarmClearedEventArgs(alarmIndex));
                                }
                            }
                            else
                            {
                                // First read - store initial state
                                _previousAlarmStates[alarmIndex] = currentValue;
                                _activeAlarms[alarmIndex] = currentValue;
                                
                                if (currentValue)
                                {
                                    Console.WriteLine($"[AlarmMonitorService] Initial state: Alarm_Triggers[{alarmIndex}] = 1 (already active)");
                                    Console.WriteLine($"[AlarmMonitorService] NOTE: Alarm is already active. To trigger, set to 0 then back to 1 in PLC.");
                                }
                            }
                            
                            // Update previous state
                            _previousAlarmStates[alarmIndex] = currentValue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[AlarmMonitorService] ✗ Error reading DINT[{dintIndex}]: {ex.Message}");
                        // Continue with next DINT
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AlarmMonitorService] ✗ Error in CheckAllAlarms: {ex.Message}");
            }
            
            return triggeredAlarms;
        }
        
        /// <summary>
        /// Gets count of currently active alarms
        /// </summary>
        public int GetActiveAlarmCount()
        {
            return _activeAlarms.Values.Count(v => v == true);
        }
        
        /// <summary>
        /// Gets list of all currently active alarm indices
        /// </summary>
        public List<int> GetActiveAlarmIndices()
        {
            return _activeAlarms.Where(kvp => kvp.Value == true).Select(kvp => kvp.Key).ToList();
        }
        
        /// <summary>
        /// Cleans up resources
        /// </summary>
        public void Dispose()
        {
            // Clear all readers - PlcTagReader doesn't implement IDisposable
            _dintReaders.Clear();
        }
    }
    
    /// <summary>
    /// Information about a triggered alarm
    /// </summary>
    public class AlarmInfo
    {
        public int AlarmIndex { get; set; }
        public string AlarmName { get; set; }
        public AlarmType AlarmType { get; set; }
        public DateTime TriggeredAt { get; set; }
    }
    
    /// <summary>
    /// Event args for alarm triggered event
    /// </summary>
    public class AlarmTriggeredEventArgs : EventArgs
    {
        public AlarmInfo AlarmInfo { get; }
        
        public AlarmTriggeredEventArgs(AlarmInfo alarmInfo)
        {
            AlarmInfo = alarmInfo;
        }
    }
    
    /// <summary>
    /// Event args for alarm cleared event
    /// </summary>
    public class AlarmClearedEventArgs : EventArgs
    {
        public int AlarmIndex { get; }
        
        public AlarmClearedEventArgs(int alarmIndex)
        {
            AlarmIndex = alarmIndex;
        }
    }
}

