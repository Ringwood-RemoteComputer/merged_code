using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ring.Services.Alarms
{
    // Services/AlarmMappingService.cs
    public class AlarmMappingService
    {
        private readonly Dictionary<int, string> _alarmDescriptions;

        public AlarmMappingService()
        {
            _alarmDescriptions = new Dictionary<int, string>
        {
            { 0, "No Alarm" },
            { 1, "E-STOP has Been Pressed" },
            { 2, "Mix System on hold for batch inspection" },
            { 3, "Mix System on hold for too long" },
            { 6, "Processor was corrupted, transfer from EEPROM has occurred" },
            { 7, "P.L.C (Programmable Logic Controller) battery low" },
            { 8, "Starch system low air pressure" },
            { 9, "{EquipmentName} Make Ready Tank scale failure" },
            { 10, "Borax/Caustic scale failure" },
            // ... continue with all alarm mappings
        };
        }

        public string GetAlarmDescription(int alarmNumber, string equipmentName = null)
        {
            if (_alarmDescriptions.TryGetValue(alarmNumber, out string description))
            {
                return description.Replace("{EquipmentName}", equipmentName ?? "");
            }
            // Default format for unmapped alarms
            return $"Alarm {alarmNumber} has been triggered";
        }

        public AlarmType GetAlarmType(int alarmNumber)
        {
            var warningAlarms = new[] { 7, 12, 21, 30, 35, 61, 62, 81, 82, 85, 86, 95, 96, 97, 98, 99, 113, 114, 120, 138, 139, 140, 141, 142, 143 };
            return warningAlarms.Contains(alarmNumber) ? AlarmType.Warning : AlarmType.Alarm;
        }

        public AlarmStatus GetAlarmStatus(int alarmNumber)
        {
            if (alarmNumber >= 3000 && alarmNumber < 4000) return AlarmStatus.Cleared;
            if (alarmNumber >= 2000 && alarmNumber < 3000) return AlarmStatus.AcknowledgedInactive;
            if (alarmNumber >= 1000 && alarmNumber < 2000) return AlarmStatus.AcknowledgedActive;
            return AlarmStatus.Active;
        }

        public int GetAbsoluteAlarmNumber(int alarmNumber)
        {
            if (alarmNumber >= 3000 && alarmNumber < 4000) return alarmNumber - 3000;
            if (alarmNumber >= 2000 && alarmNumber < 3000) return alarmNumber - 2000;
            if (alarmNumber >= 1000 && alarmNumber < 2000) return alarmNumber - 1000;
            return alarmNumber;
        }
    }
}