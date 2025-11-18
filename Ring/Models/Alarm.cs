
// Models/Alarm.cs
using System;

public class Alarm
{
    public int Id { get; set; }
    public int SystemNumber { get; set; }
    public string SystemType { get; set; } // "Mixer", "Distribution", etc.
    public int AlarmNumber { get; set; }
    public int AbsoluteAlarmNumber { get; set; }
    public AlarmType Type { get; set; }
    public AlarmStatus Status { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime AlarmTime { get; set; }
    public DateTime? AcknowledgeTime { get; set; }
    public string EquipmentName { get; set; }
    public bool IsActive { get; set; }
}

public enum AlarmType
{
    Alarm = 1,
    Warning = 2
}

public enum AlarmStatus
{
    Active = 0,
    AcknowledgedActive = 1,
    AcknowledgedInactive = 2,
    Cleared = 3
}