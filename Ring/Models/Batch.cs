using System;

namespace Ring.Models
{
    /// <summary>
    /// Batch status
    /// </summary>
    public enum BatchStatus
    {
        NotStarted = 0,
        InProgress = 1,
        Completed = 2,
        Aborted = 3,
        Paused = 4
    }

    /// <summary>
    /// Batch entity for tracking production batches
    /// </summary>
    public class Batch
    {
        public int Id { get; set; }

        public string PCID { get; set; } = string.Empty; // Production Control ID

        public string FormulaName { get; set; } = string.Empty;

        public BatchStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime? AbortedAt { get; set; }

        public int? CreatedByUserId { get; set; }

        public string? CreatedByUserName { get; set; }

        public int? SystemNumber { get; set; }

        public int? TankNumber { get; set; }

        public string? Description { get; set; }

        public decimal? TargetQuantity { get; set; }

        public decimal? ActualQuantity { get; set; }

        public string? Units { get; set; }

        public decimal? Temperature { get; set; }

        public decimal? Viscosity { get; set; }

        public string? Notes { get; set; }

        public bool IsActive => Status == BatchStatus.InProgress || Status == BatchStatus.Paused;

        public bool IsCompleted => Status == BatchStatus.Completed;

        public bool IsAborted => Status == BatchStatus.Aborted;

        /// <summary>
        /// Start the batch
        /// </summary>
        public void Start()
        {
            if (Status == BatchStatus.NotStarted)
            {
                Status = BatchStatus.InProgress;
                StartedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Complete the batch
        /// </summary>
        public void Complete()
        {
            if (Status == BatchStatus.InProgress)
            {
                Status = BatchStatus.Completed;
                CompletedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Abort the batch
        /// </summary>
        public void Abort()
        {
            if (Status == BatchStatus.InProgress || Status == BatchStatus.Paused)
            {
                Status = BatchStatus.Aborted;
                AbortedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Pause the batch
        /// </summary>
        public void Pause()
        {
            if (Status == BatchStatus.InProgress)
            {
                Status = BatchStatus.Paused;
            }
        }

        /// <summary>
        /// Resume the batch
        /// </summary>
        public void Resume()
        {
            if (Status == BatchStatus.Paused)
            {
                Status = BatchStatus.InProgress;
            }
        }

        /// <summary>
        /// Get batch duration
        /// </summary>
        public TimeSpan GetDuration()
        {
            var endTime = CompletedAt ?? AbortedAt ?? DateTime.UtcNow;
            var startTime = StartedAt ?? CreatedAt;
            return endTime - startTime;
        }

        /// <summary>
        /// Generate PCID
        /// </summary>
        public static string GeneratePCID()
        {
            return $"PC{DateTime.Now:yyyyMMdd}{DateTime.Now:HHmmss}";
        }
    }
}
