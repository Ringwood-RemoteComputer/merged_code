//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Threading;
//using libplctag;
//using libplctag.DataTypes;
//using System.ComponentModel.DataAnnotations;

//namespace Ring.Services.PLC
//{
//    public class PlcCommunicationService : IDisposable
//    {
//        private readonly Dictionary<string, PlcTag> _tags = new Dictionary<string, PlcTag>();
//        private readonly Timer _updateTimer;
//        private readonly string _ipAddress;
//        private readonly string _path;
//        private bool _isConnected = false;
//        private bool _simulationMode = false;

//        public event EventHandler<string>? ConnectionStatusChanged;
//        public event EventHandler<PlcTag>? TagValueChanged;

//        public PlcCommunicationService(string ipAddress = "192.168.1.100", string path = "1,0", bool simulationMode = false)
//        {
//            _ipAddress = ipAddress;
//            _path = path;
//            _simulationMode = simulationMode;

//            // Update timer for real-time data
//            _updateTimer = new Timer(UpdateTags, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
//        }

//        public bool IsConnected => _isConnected;

//        public bool SimulationMode => _simulationMode;

//        /// <summary>
//        /// Add a tag to monitor
//        /// </summary>
//        public void AddTag(PlcTag tag)
//        {
//            if (!_tags.ContainsKey(tag.TagName))
//            {
//                _tags[tag.TagName] = tag;
//            }
//        }

//        /// <summary>
//        /// Remove a tag from monitoring
//        /// </summary>
//        public void RemoveTag(string tagName)
//        {
//            if (_tags.ContainsKey(tagName))
//            {
//                _tags.Remove(tagName);
//            }
//        }

//        /// <summary>
//        /// Read a specific tag value
//        /// </summary>
//        public async Task<object?> ReadTagAsync(string tagName)
//        {
//            if (_simulationMode)
//            {
//                return await ReadSimulatedTagAsync(tagName);
//            }

//            if (_tags.TryGetValue(tagName, out var tag))
//            {
//                try
//                {
//                    var value = await tag.ReadValueAsync();
//                    tag.LastValue = value;
//                    tag.LastReadTime = DateTime.UtcNow;
//                    tag.IsConnected = true;
//                    tag.ResetErrors();
//                    return value;
//                }
//                catch (Exception ex)
//                {
//                    tag.UpdateError(ex.Message, true);
//                    tag.IsConnected = false;
//                    throw;
//                }
//            }

//            throw new ArgumentException($"Tag '{tagName}' not found");
//        }

//        /// <summary>
//        /// Write a value to a specific tag
//        /// </summary>
//        public async Task<bool> WriteTagAsync(string tagName, object value)
//        {
//            if (_simulationMode)
//            {
//                return await WriteSimulatedTagAsync(tagName, value);
//            }

//            if (_tags.TryGetValue(tagName, out var tag))
//            {
//                try
//                {
//                    var result = await tag.WriteValueAsync(value);
//                    if (result)
//                    {
//                        tag.LastValue = value;
//                        tag.LastWriteTime = DateTime.UtcNow;
//                        tag.IsConnected = true;
//                        tag.ResetErrors();
//                    }
//                    return result;
//                }
//                catch (Exception ex)
//                {
//                    tag.UpdateError(ex.Message, false);
//                    tag.IsConnected = false;
//                    throw;
//                }
//            }

//            throw new ArgumentException($"Tag '{tagName}' not found");
//        }

//        /// <summary>
//        /// Update all tags (called by timer)
//        /// </summary>
//        private async void UpdateTags(object? state)
//        {
//            if (_simulationMode)
//            {
//                await UpdateSimulatedTagsAsync();
//                return;
//            }

//            foreach (var tag in _tags.Values)
//            {
//                if (tag.IsEnabled)
//                {
//                    try
//                    {
//                        var oldValue = tag.LastValue;
//                        var newValue = await tag.ReadValueAsync();

//                        tag.LastValue = newValue;
//                        tag.LastReadTime = DateTime.UtcNow;
//                        tag.IsConnected = true;
//                        tag.ResetErrors();

//                        // Notify if value changed
//                        if (!Equals(oldValue, newValue))
//                        {
//                            TagValueChanged?.Invoke(this, tag);
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        tag.UpdateError(ex.Message, true);
//                        tag.IsConnected = false;
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Read simulated tag value
//        /// </summary>
//        private async Task<object?> ReadSimulatedTagAsync(string tagName)
//        {
//            await Task.Delay(10); // Simulate network delay

//            return tagName.ToLower() switch
//            {
//                "system_status" => 1,
//                "batching_mode" => 0,
//                "tank1_level" => 75.5f,
//                "tank1_temperature" => 85.2f,
//                "tank1_agitator" => true,
//                "tank1_heating" => false,
//                "tank1_discharge_valve" => false,
//                "tank1_fill_valve" => false,
//                "alarm_active" => false,
//                "alarm_acknowledge_hour" => 0,
//                _ => 0
//            };
//        }

//        /// <summary>
//        /// Write simulated tag value
//        /// </summary>
//        private async Task<bool> WriteSimulatedTagAsync(string tagName, object value)
//        {
//            await Task.Delay(10); // Simulate network delay
//            return true; // Always successful in simulation
//        }

//        /// <summary>
//        /// Update all simulated tags
//        /// </summary>
//        private async Task UpdateSimulatedTagsAsync()
//        {
//            await Task.Delay(10); // Simulate network delay

//            foreach (var tag in _tags.Values)
//            {
//                if (tag.IsEnabled)
//                {
//                    var oldValue = tag.LastValue;
//                    var newValue = await ReadSimulatedTagAsync(tag.TagName);

//                    tag.LastValue = newValue;
//                    tag.LastReadTime = DateTime.UtcNow;
//                    tag.IsConnected = true;

//                    // Notify if value changed
//                    if (!Equals(oldValue, newValue))
//                    {
//                        TagValueChanged?.Invoke(this, tag);
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Connect to PLC
//        /// </summary>
//        public async Task<bool> ConnectAsync()
//        {
//            if (_simulationMode)
//            {
//                _isConnected = true;
//                ConnectionStatusChanged?.Invoke(this, "Connected (Simulation Mode)");
//                return true;
//            }

//            try
//            {
//                // Test connection with a simple tag read
//                var testTag = new TagBool()
//                {
//                    Name = "TestConnection",
//                    Gateway = _ipAddress,
//                    Path = _path,
//                    PlcType = PlcType.ControlLogix,
//                    Protocol = Protocol.ab_eip
//                };

//                await Task.Run(() => testTag.Read());
//                _isConnected = true;
//                ConnectionStatusChanged?.Invoke(this, "Connected");
//                return true;
//            }
//            catch (Exception ex)
//            {
//                _isConnected = false;
//                ConnectionStatusChanged?.Invoke(this, $"Connection Failed: {ex.Message}");
//                return false;
//            }
//        }

//        /// <summary>
//        /// Disconnect from PLC
//        /// </summary>
//        public void Disconnect()
//        {
//            _isConnected = false;
//            ConnectionStatusChanged?.Invoke(this, "Disconnected");
//        }

//        /// <summary>
//        /// Get all tags
//        /// </summary>
//        public IEnumerable<PlcTag> GetAllTags()
//        {
//            return _tags.Values;
//        }

//        /// <summary>
//        /// Get tag by name
//        /// </summary>
//        public PlcTag? GetTag(string tagName)
//        {
//            return _tags.TryGetValue(tagName, out var tag) ? tag : null;
//        }

//        public void Dispose()
//        {
//            _updateTimer?.Dispose();
//        }
//    }
//}
// Services/PlcAlarmService.cs
//using System.Collections.Generic;
//using libplctag;

//public class PlcAlarmService
//{
//    private readonly Dictionary<string, Tag> _alarmTags;

//    public PlcAlarmService()
//    {
//        _alarmTags = new Dictionary<string, Tag>
//        {
//            // Mixer Alarms (Template 580-590)
//            { "MixerAlarmArray", new Tag("protocol=ab-eip&gateway=192.168.1.10&path=1,0&cpu=lgx&name=AlarmMixerArray&elem_count=15") },
            
//            // Distribution Alarms (Template 980-990)  
//            { "DistributionAlarmArray", new Tag("protocol=ab-eip&gateway=192.168.1.10&path=1,0&cpu=lgx&name=AlarmDistributionArray&elem_count=15") },
            
//            // Alarm Timestamps
//            { "AlarmYear", new Tag("protocol=ab-eip&gateway=192.168.1.10&path=1,0&cpu=lgx&name=AlarmAlarmYear&elem_count=20") },
//            { "AlarmMonth", new Tag("protocol=ab-eip&gateway=192.168.1.10&path=1,0&cpu=lgx&name=AlarmAlarmMonth&elem_count=20") },
//            { "AlarmDay", new Tag("protocol=ab-eip&gateway=192.168.1.10&path=1,0&cpu=lgx&name=AlarmAlarmDay&elem_count=20") },
//            { "AlarmHour", new Tag("protocol=ab-eip&gateway=192.168.1.10&path=1,0&cpu=lgx&name=AlarmAlarmHour&elem_count=20") },
//            { "AlarmMinute", new Tag("protocol=ab-eip&gateway=192.168.1.10&path=1,0&cpu=lgx&name=AlarmAlarmMinute&elem_count=20") },
//            { "AlarmSecond", new Tag("protocol=ab-eip&gateway=192.168.1.10&path=1,0&cpu=lgx&name=AlarmAlarmSecond&elem_count=20") }
//        };
//    }
//}