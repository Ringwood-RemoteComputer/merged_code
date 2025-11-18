using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Ring.Models;
using Ring.Services;

namespace Ring.ViewModels
{
    /// <summary>
    /// ViewModel for individual batch report items
    /// </summary>
    public class BatchReportItemViewModel : INotifyPropertyChanged, IDisposable
    {
        #region Private Fields

        private readonly IOperationDescriptionService _operationDescriptionService;
        private readonly ILogger _logger;
        private bool _disposed = false;

        private BatchReportModel _batchReport;
        private bool _isSelected;
        private string _formattedStartDateTime;
        private string _formattedEndDateTime;
        private string _formattedDuration;
        private string _formattedVolume;
        private string _formattedTemperature;
        private string _formattedWeight;
        private string _statusDisplayText;
        private string _statusColor;

        #endregion

        #region Public Properties

        /// <summary>
        /// Batch report model
        /// </summary>
        public BatchReportModel BatchReport
        {
            get => _batchReport;
            set
            {
                if (_batchReport != value)
                {
                    _batchReport = value;
                    OnPropertyChanged();
                    UpdateFormattedProperties();
                }
            }
        }

        /// <summary>
        /// Whether this item is selected
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Batch ID
        /// </summary>
        public int BatchId => BatchReport?.BatchId ?? 0;

        /// <summary>
        /// Formula number
        /// </summary>
        public int FormulaNumber => BatchReport?.FormulaNumber ?? 0;

        /// <summary>
        /// Formula name
        /// </summary>
        public string FormulaName => BatchReport?.FormulaName ?? string.Empty;

        /// <summary>
        /// Storage tank number
        /// </summary>
        public int StorageTankNumber => BatchReport?.StorageTankNumber ?? 0;

        /// <summary>
        /// Storage tank name
        /// </summary>
        public string StorageTankName => BatchReport?.StorageTankName ?? string.Empty;

        /// <summary>
        /// Start date and time
        /// </summary>
        public DateTime StartDateTime => BatchReport?.StartDateTime ?? DateTime.MinValue;

        /// <summary>
        /// End date and time
        /// </summary>
        public DateTime EndDateTime => BatchReport?.EndDateTime ?? DateTime.MinValue;

        /// <summary>
        /// Volume
        /// </summary>
        public decimal Volume => BatchReport?.Volume ?? 0;

        /// <summary>
        /// Temperature
        /// </summary>
        public decimal Temperature => BatchReport?.Temperature ?? 0;

        /// <summary>
        /// Start weight
        /// </summary>
        public decimal StartWeight => BatchReport?.StartWeight ?? 0;

        /// <summary>
        /// End weight
        /// </summary>
        public decimal EndWeight => BatchReport?.EndWeight ?? 0;

        /// <summary>
        /// Status
        /// </summary>
        public int Status => BatchReport?.Status ?? 0;

        /// <summary>
        /// Status text
        /// </summary>
        public string StatusText => BatchReport?.StatusText ?? string.Empty;

        /// <summary>
        /// Notes
        /// </summary>
        public string Notes => BatchReport?.Notes ?? string.Empty;

        /// <summary>
        /// Operator name
        /// </summary>
        public string OperatorName => BatchReport?.OperatorName ?? string.Empty;

        /// <summary>
        /// Shift
        /// </summary>
        public string Shift => BatchReport?.Shift ?? string.Empty;

        /// <summary>
        /// Formatted start date and time
        /// </summary>
        public string FormattedStartDateTime
        {
            get => _formattedStartDateTime;
            private set
            {
                _formattedStartDateTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Formatted end date and time
        /// </summary>
        public string FormattedEndDateTime
        {
            get => _formattedEndDateTime;
            private set
            {
                _formattedEndDateTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Formatted duration
        /// </summary>
        public string FormattedDuration
        {
            get => _formattedDuration;
            private set
            {
                _formattedDuration = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Formatted volume
        /// </summary>
        public string FormattedVolume
        {
            get => _formattedVolume;
            private set
            {
                _formattedVolume = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Formatted temperature
        /// </summary>
        public string FormattedTemperature
        {
            get => _formattedTemperature;
            private set
            {
                _formattedTemperature = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Formatted weight
        /// </summary>
        public string FormattedWeight
        {
            get => _formattedWeight;
            private set
            {
                _formattedWeight = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Status display text
        /// </summary>
        public string StatusDisplayText
        {
            get => _statusDisplayText;
            private set
            {
                _statusDisplayText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Status color for UI
        /// </summary>
        public string StatusColor
        {
            get => _statusColor;
            private set
            {
                _statusColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Batch duration
        /// </summary>
        public TimeSpan Duration => EndDateTime - StartDateTime;

        /// <summary>
        /// Weight difference
        /// </summary>
        public decimal WeightDifference => EndWeight - StartWeight;

        /// <summary>
        /// Whether batch is completed
        /// </summary>
        public bool IsCompleted => Status == 1;

        /// <summary>
        /// Whether batch is in progress
        /// </summary>
        public bool IsInProgress => Status == 0;

        /// <summary>
        /// Whether batch failed
        /// </summary>
        public bool IsFailed => Status > 1;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the BatchReportItemViewModel
        /// </summary>
        /// <param name="batchReport">Batch report model</param>
        /// <param name="operationDescriptionService">Operation description service</param>
        /// <param name="logger">Logger instance</param>
        public BatchReportItemViewModel(BatchReportModel batchReport, 
            IOperationDescriptionService operationDescriptionService = null, 
            ILogger logger = null)
        {
            _operationDescriptionService = operationDescriptionService ?? new OperationDescriptionService();
            _logger = logger ?? new Logger();
            
            BatchReport = batchReport ?? throw new ArgumentNullException(nameof(batchReport));
            UpdateFormattedProperties();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the formatted properties
        /// </summary>
        public void UpdateFormattedProperties()
        {
            try
            {
                FormattedStartDateTime = StartDateTime.ToString("MM/dd/yyyy HH:mm:ss");
                FormattedEndDateTime = EndDateTime.ToString("MM/dd/yyyy HH:mm:ss");
                FormattedDuration = FormatDuration(Duration);
                FormattedVolume = $"{Volume:F1} L";
                FormattedTemperature = $"{Temperature:F1}°C";
                FormattedWeight = $"{StartWeight:F1} - {EndWeight:F1} kg";
                StatusDisplayText = GetStatusDisplayText(Status);
                StatusColor = GetStatusColor(Status);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating formatted properties: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets a summary of the batch report
        /// </summary>
        /// <returns>Batch report summary</returns>
        public string GetSummary()
        {
            try
            {
                return $"Batch {BatchId}: {FormulaName} in {StorageTankName} - {StatusDisplayText}";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting batch summary: {ex.Message}", ex);
                return $"Batch {BatchId}: Error getting summary";
            }
        }

        /// <summary>
        /// Gets detailed information about the batch
        /// </summary>
        /// <returns>Detailed batch information</returns>
        public string GetDetailedInfo()
        {
            try
            {
                return $"Batch ID: {BatchId}\n" +
                       $"Formula: {FormulaName} (#{FormulaNumber})\n" +
                       $"Storage Tank: {StorageTankName} (#{StorageTankNumber})\n" +
                       $"Start: {FormattedStartDateTime}\n" +
                       $"End: {FormattedEndDateTime}\n" +
                       $"Duration: {FormattedDuration}\n" +
                       $"Volume: {FormattedVolume}\n" +
                       $"Temperature: {FormattedTemperature}\n" +
                       $"Weight: {FormattedWeight}\n" +
                       $"Status: {StatusDisplayText}\n" +
                       $"Operator: {OperatorName}\n" +
                       $"Shift: {Shift}";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting detailed batch info: {ex.Message}", ex);
                return $"Error getting detailed information: {ex.Message}";
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Formats duration for display
        /// </summary>
        /// <param name="duration">Duration to format</param>
        /// <returns>Formatted duration string</returns>
        private string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalDays >= 1)
            {
                return $"{(int)duration.TotalDays}d {duration.Hours}h {duration.Minutes}m";
            }
            else if (duration.TotalHours >= 1)
            {
                return $"{duration.Hours}h {duration.Minutes}m {duration.Seconds}s";
            }
            else if (duration.TotalMinutes >= 1)
            {
                return $"{duration.Minutes}m {duration.Seconds}s";
            }
            else
            {
                return $"{duration.Seconds}s";
            }
        }

        /// <summary>
        /// Gets status display text
        /// </summary>
        /// <param name="status">Status code</param>
        /// <returns>Status display text</returns>
        private string GetStatusDisplayText(int status)
        {
            return status switch
            {
                0 => "In Progress",
                1 => "Completed",
                2 => "Failed",
                3 => "Cancelled",
                4 => "Paused",
                5 => "Error",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Gets status color for UI
        /// </summary>
        /// <param name="status">Status code</param>
        /// <returns>Status color</returns>
        private string GetStatusColor(int status)
        {
            return status switch
            {
                0 => "Orange",      // In Progress
                1 => "Green",        // Completed
                2 => "Red",          // Failed
                3 => "Gray",         // Cancelled
                4 => "Yellow",       // Paused
                5 => "Red",          // Error
                _ => "Gray"          // Unknown
            };
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _disposed = true;
            }
        }

        #endregion
    }
}
