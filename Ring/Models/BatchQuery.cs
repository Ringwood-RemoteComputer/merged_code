using System;
using System.ComponentModel.DataAnnotations;
using Ring.Validation;

namespace Ring.Models
{
    /// <summary>
    /// Represents batch data for querying and reporting purposes
    /// </summary>
    public class BatchQuery
    {
        /// <summary>
        /// Unique identifier for the batch
        /// </summary>
        [Required(ErrorMessage = "Batch ID is required.")]
        [PositiveNumberValidation(ErrorMessage = "Batch ID must be positive.")]
        public int Id { get; set; }

        /// <summary>
        /// Name of the formula used for this batch
        /// </summary>
        [Required(ErrorMessage = "Formula name is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Formula name must be between 1 and 100 characters.")]
        public string FormulaName { get; set; }

        /// <summary>
        /// Number of the formula used for this batch
        /// </summary>
        [Required(ErrorMessage = "Formula number is required.")]
        [FormulaNumberValidation(ErrorMessage = "Formula number must be between 1 and 9999.")]
        public int FormulaNumber { get; set; }

        /// <summary>
        /// Date when the batch started
        /// </summary>
        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Time when the batch started
        /// </summary>
        [Required(ErrorMessage = "Start time is required.")]
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// Date when the batch ended
        /// </summary>
        [Required(ErrorMessage = "End date is required.")]
        [DateRangeValidation("StartDate", ErrorMessage = "End date must be after start date.")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Time when the batch ended
        /// </summary>
        [Required(ErrorMessage = "End time is required.")]
        public TimeSpan EndTime { get; set; }

        /// <summary>
        /// Name of the storage tank used for this batch
        /// </summary>
        [Required(ErrorMessage = "Storage tank name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Storage tank name must be between 1 and 50 characters.")]
        public string StorageTankName { get; set; }

        /// <summary>
        /// Number of the storage tank used for this batch
        /// </summary>
        [Required(ErrorMessage = "Storage tank number is required.")]
        [TankNumberValidation(ErrorMessage = "Storage tank number must be between 1 and 50.")]
        public int StorageTankNumber { get; set; }

        /// <summary>
        /// Volume of the batch
        /// </summary>
        [Required(ErrorMessage = "Volume is required.")]
        [PositiveNumberValidation(ErrorMessage = "Volume must be positive.")]
        [Range(0.01, 10000, ErrorMessage = "Volume must be between 0.01 and 10,000.")]
        public decimal Volume { get; set; }

        /// <summary>
        /// Temperature of the batch
        /// </summary>
        [Required(ErrorMessage = "Temperature is required.")]
        [TemperatureValidation(ErrorMessage = "Temperature must be between -50°C and 200°C.")]
        public decimal Temperature { get; set; }

        /// <summary>
        /// Current status of the batch (numeric code)
        /// </summary>
        [Required(ErrorMessage = "Status is required.")]
        [StatusValidation(ErrorMessage = "Status must be a valid status code (0-5).")]
        public int Status { get; set; }

        /// <summary>
        /// Text description of the batch status
        /// </summary>
        [Required(ErrorMessage = "Status text is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Status text must be between 1 and 50 characters.")]
        public string StatusText { get; set; }

        /// <summary>
        /// Starting weight of the batch
        /// </summary>
        [Required(ErrorMessage = "Start weight is required.")]
        [PositiveNumberValidation(allowZero: true, ErrorMessage = "Start weight must be zero or positive.")]
        [Range(0, 100000, ErrorMessage = "Start weight must be between 0 and 100,000.")]
        public decimal StartWeight { get; set; }

        /// <summary>
        /// Ending weight of the batch
        /// </summary>
        [Required(ErrorMessage = "End weight is required.")]
        [PositiveNumberValidation(allowZero: true, ErrorMessage = "End weight must be zero or positive.")]
        [Range(0, 100000, ErrorMessage = "End weight must be between 0 and 100,000.")]
        public decimal EndWeight { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BatchQuery()
        {
            FormulaName = string.Empty;
            StorageTankName = string.Empty;
            StatusText = string.Empty;
        }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="id">Batch ID</param>
        /// <param name="formulaName">Formula name</param>
        /// <param name="formulaNumber">Formula number</param>
        /// <param name="startDate">Start date</param>
        /// <param name="startTime">Start time</param>
        /// <param name="endDate">End date</param>
        /// <param name="endTime">End time</param>
        /// <param name="storageTankName">Storage tank name</param>
        /// <param name="storageTankNumber">Storage tank number</param>
        /// <param name="volume">Volume</param>
        /// <param name="temperature">Temperature</param>
        /// <param name="status">Status (numeric code)</param>
        /// <param name="statusText">Status text description</param>
        /// <param name="startWeight">Starting weight</param>
        /// <param name="endWeight">Ending weight</param>
        public BatchQuery(int id, string formulaName, int formulaNumber, DateTime startDate, TimeSpan startTime, 
                         DateTime endDate, TimeSpan endTime, string storageTankName, int storageTankNumber, 
                         decimal volume, decimal temperature, int status, string statusText, 
                         decimal startWeight, decimal endWeight)
        {
            Id = id;
            FormulaName = formulaName ?? string.Empty;
            FormulaNumber = formulaNumber;
            StartDate = startDate;
            StartTime = startTime;
            EndDate = endDate;
            EndTime = endTime;
            StorageTankName = storageTankName ?? string.Empty;
            StorageTankNumber = storageTankNumber;
            Volume = volume;
            Temperature = temperature;
            Status = status;
            StatusText = statusText ?? string.Empty;
            StartWeight = startWeight;
            EndWeight = endWeight;
        }

        /// <summary>
        /// Gets the total duration of the batch
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                var startDateTime = StartDate.Add(StartTime);
                var endDateTime = EndDate.Add(EndTime);
                return endDateTime - startDateTime;
            }
        }

        /// <summary>
        /// Gets a formatted string representation of the batch duration
        /// </summary>
        public string FormattedDuration
        {
            get
            {
                var duration = Duration;
                if (duration.TotalDays >= 1)
                    return $"{duration.Days}d {duration.Hours}h {duration.Minutes}m";
                else if (duration.TotalHours >= 1)
                    return $"{duration.Hours}h {duration.Minutes}m";
                else
                    return $"{duration.Minutes}m {duration.Seconds}s";
            }
        }

        /// <summary>
        /// Returns a string representation of the BatchQuery object
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return $"Batch {Id}: {FormulaName} (Formula #{FormulaNumber}) - Tank {StorageTankNumber} - {StatusText} - {FormattedDuration}";
        }
    }
}
