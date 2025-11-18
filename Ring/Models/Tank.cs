using System;

namespace Ring.Models
{
    /// <summary>
    /// Tank status
    /// </summary>
    public enum TankStatus
    {
        Empty = 0,
        Filling = 1,
        Full = 2,
        Discharging = 3,
        Maintenance = 4,
        Offline = 5
    }

    /// <summary>
    /// Tank entity for storage tank management
    /// </summary>
    public class Tank
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int TankNumber { get; set; }

        public int SystemNumber { get; set; }

        public TankStatus Status { get; set; }

        public decimal Capacity { get; set; }

        public decimal CurrentLevel { get; set; }

        public decimal Temperature { get; set; }

        public decimal Viscosity { get; set; }

        public string? Material { get; set; }

        public string? Units { get; set; }

        public bool IsAgitatorRunning { get; set; } = false;

        public bool IsHeatingEnabled { get; set; } = false;

        public bool IsDischargeValveOpen { get; set; } = false;

        public bool IsFillValveOpen { get; set; } = false;

        public decimal? SetpointTemperature { get; set; }

        public decimal? SetpointLevel { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public string? Notes { get; set; }

        public bool IsOnline => Status != TankStatus.Offline;

        public bool IsEmpty => CurrentLevel <= 0;

        public bool IsFull => CurrentLevel >= Capacity;

        public decimal FillPercentage => Capacity > 0 ? (CurrentLevel / Capacity) * 100 : 0;

        /// <summary>
        /// Update tank level
        /// </summary>
        public void UpdateLevel(decimal newLevel)
        {
            CurrentLevel = Math.Max(0, Math.Min(newLevel, Capacity));
            LastUpdated = DateTime.UtcNow;

            // Update status based on level
            if (CurrentLevel <= 0)
                Status = TankStatus.Empty;
            else if (CurrentLevel >= Capacity)
                Status = TankStatus.Full;
            else if (Status == TankStatus.Empty)
                Status = TankStatus.Filling;
        }

        /// <summary>
        /// Update temperature
        /// </summary>
        public void UpdateTemperature(decimal newTemperature)
        {
            Temperature = newTemperature;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Update viscosity
        /// </summary>
        public void UpdateViscosity(decimal newViscosity)
        {
            Viscosity = newViscosity;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Start agitator
        /// </summary>
        public void StartAgitator()
        {
            IsAgitatorRunning = true;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Stop agitator
        /// </summary>
        public void StopAgitator()
        {
            IsAgitatorRunning = false;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Enable heating
        /// </summary>
        public void EnableHeating()
        {
            IsHeatingEnabled = true;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Disable heating
        /// </summary>
        public void DisableHeating()
        {
            IsHeatingEnabled = false;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Open discharge valve
        /// </summary>
        public void OpenDischargeValve()
        {
            IsDischargeValveOpen = true;
            if (Status == TankStatus.Full)
                Status = TankStatus.Discharging;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Close discharge valve
        /// </summary>
        public void CloseDischargeValve()
        {
            IsDischargeValveOpen = false;
            if (Status == TankStatus.Discharging)
                Status = TankStatus.Full;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Open fill valve
        /// </summary>
        public void OpenFillValve()
        {
            IsFillValveOpen = true;
            if (Status == TankStatus.Empty)
                Status = TankStatus.Filling;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Close fill valve
        /// </summary>
        public void CloseFillValve()
        {
            IsFillValveOpen = false;
            if (Status == TankStatus.Filling)
                Status = TankStatus.Full;
            LastUpdated = DateTime.UtcNow;
        }
    }
}
