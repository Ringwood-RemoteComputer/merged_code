using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Ring.Models;
using Ring.Services;

namespace Ring.ViewModels
{
    /// <summary>
    /// ViewModel for batch steps
    /// </summary>
    public class BatchReportStepViewModel : INotifyPropertyChanged, IDisposable
    {
        #region Private Fields

        private readonly IOperationDescriptionService _operationDescriptionService;
        private readonly ILogger _logger;
        private bool _disposed = false;

        private BatchStepModel _batchStep;
        private string _formattedAmountPreset;
        private string _formattedAmountActual;
        private string _formattedAmountDifference;
        private string _formattedTimePreset;
        private string _formattedTimeActual;
        private string _formattedTimeDifference;
        private string _formattedTemperature;
        private string _formattedPressure;
        private string _formattedStepStartTime;
        private string _formattedStepEndTime;
        private string _amountDifferenceColor;
        private string _timeDifferenceColor;
        private string _stepStatusColor;
        private bool _isAmountVarianceHigh;
        private bool _isTimeVarianceHigh;
        private string _varianceIndicator;

        #endregion

        #region Public Properties

        /// <summary>
        /// Batch step model
        /// </summary>
        public BatchStepModel BatchStep
        {
            get => _batchStep;
            set
            {
                if (_batchStep != value)
                {
                    _batchStep = value;
                    OnPropertyChanged();
                    UpdateFormattedProperties();
                }
            }
        }

        /// <summary>
        /// Step ID
        /// </summary>
        public int StepId => BatchStep?.StepId ?? 0;

        /// <summary>
        /// Step sequence
        /// </summary>
        public int StepSequence => BatchStep?.StepSequence ?? 0;

        /// <summary>
        /// Operation type
        /// </summary>
        public string OperationType => BatchStep?.OperationType ?? string.Empty;

        /// <summary>
        /// Operation description
        /// </summary>
        public string OperationDescription => BatchStep?.OperationDescription ?? string.Empty;

        /// <summary>
        /// Ingredient name
        /// </summary>
        public string IngredientName => BatchStep?.IngredientName ?? string.Empty;

        /// <summary>
        /// Flexible ingredient name
        /// </summary>
        public string FlexibleIngredientName => BatchStep?.FlexibleIngredientName ?? string.Empty;

        /// <summary>
        /// Preset amount
        /// </summary>
        public decimal AmountPreset => BatchStep?.AmountPreset ?? 0;

        /// <summary>
        /// Actual amount
        /// </summary>
        public decimal AmountActual => BatchStep?.AmountActual ?? 0;

        /// <summary>
        /// Preset time in minutes
        /// </summary>
        public int TimePresetMinutes => BatchStep?.TimePresetMinutes ?? 0;

        /// <summary>
        /// Actual time in minutes
        /// </summary>
        public int TimeActualMinutes => BatchStep?.TimeActualMinutes ?? 0;

        /// <summary>
        /// Step start timestamp
        /// </summary>
        public DateTime? StepStartTimestamp => BatchStep?.StepStartTimestamp;

        /// <summary>
        /// Step end timestamp
        /// </summary>
        public DateTime? StepEndTimestamp => BatchStep?.StepEndTimestamp;

        /// <summary>
        /// Temperature
        /// </summary>
        public decimal? Temperature => BatchStep?.Temperature;

        /// <summary>
        /// Pressure
        /// </summary>
        public decimal? Pressure => BatchStep?.Pressure;

        /// <summary>
        /// Step status
        /// </summary>
        public string StepStatus => BatchStep?.StepStatus ?? string.Empty;

        /// <summary>
        /// Step notes
        /// </summary>
        public string StepNotes => BatchStep?.StepNotes ?? string.Empty;

        /// <summary>
        /// Whether step is required
        /// </summary>
        public bool IsRequired => BatchStep?.IsRequired ?? false;

        /// <summary>
        /// Whether step is skipped
        /// </summary>
        public bool IsSkipped => BatchStep?.IsSkipped ?? false;

        /// <summary>
        /// Step priority
        /// </summary>
        public int Priority => BatchStep?.Priority ?? 0;

        /// <summary>
        /// Amount difference
        /// </summary>
        public decimal AmountDifference => BatchStep?.AmountDifference ?? 0;

        /// <summary>
        /// Amount difference percentage
        /// </summary>
        public decimal AmountDifferencePercentage => BatchStep?.AmountDifferencePercentage ?? 0;

        /// <summary>
        /// Time difference in minutes
        /// </summary>
        public int TimeDifferenceMinutes => BatchStep?.TimeDifferenceMinutes ?? 0;

        /// <summary>
        /// Formatted preset amount
        /// </summary>
        public string FormattedAmountPreset
        {
            get => _formattedAmountPreset;
            private set
            {
                _formattedAmountPreset = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Formatted actual amount
        /// </summary>
        public string FormattedAmountActual
        {
            get => _formattedAmountActual;
            private set
            {
                _formattedAmountActual = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Formatted amount difference
        /// </summary>
        public string FormattedAmountDifference
        {
            get => _formattedAmountDifference;
            private set
            {
                _formattedAmountDifference = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Formatted preset time
        /// </summary>
        public string FormattedTimePreset
        {
            get => _formattedTimePreset;
            private set
            {
                _formattedTimePreset = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Formatted actual time
        /// </summary>
        public string FormattedTimeActual
        {
            get => _formattedTimeActual;
            private set
            {
                _formattedTimeActual = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Formatted time difference
        /// </summary>
        public string FormattedTimeDifference
        {
            get => _formattedTimeDifference;
            private set
            {
                _formattedTimeDifference = value;
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
        /// Formatted pressure
        /// </summary>
        public string FormattedPressure
        {
            get => _formattedPressure;
            private set
            {
                _formattedPressure = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Formatted step start time
        /// </summary>
        public string FormattedStepStartTime
        {
            get => _formattedStepStartTime;
            private set
            {
                _formattedStepStartTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Formatted step end time
        /// </summary>
        public string FormattedStepEndTime
        {
            get => _formattedStepEndTime;
            private set
            {
                _formattedStepEndTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Amount difference color for UI
        /// </summary>
        public string AmountDifferenceColor
        {
            get => _amountDifferenceColor;
            private set
            {
                _amountDifferenceColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Time difference color for UI
        /// </summary>
        public string TimeDifferenceColor
        {
            get => _timeDifferenceColor;
            private set
            {
                _timeDifferenceColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Step status color for UI
        /// </summary>
        public string StepStatusColor
        {
            get => _stepStatusColor;
            private set
            {
                _stepStatusColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether amount variance is high
        /// </summary>
        public bool IsAmountVarianceHigh
        {
            get => _isAmountVarianceHigh;
            private set
            {
                _isAmountVarianceHigh = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether time variance is high
        /// </summary>
        public bool IsTimeVarianceHigh
        {
            get => _isTimeVarianceHigh;
            private set
            {
                _isTimeVarianceHigh = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Variance indicator
        /// </summary>
        public string VarianceIndicator
        {
            get => _varianceIndicator;
            private set
            {
                _varianceIndicator = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether step is completed
        /// </summary>
        public bool IsCompleted => StepStatus?.Equals("Completed", StringComparison.OrdinalIgnoreCase) == true;

        /// <summary>
        /// Whether step is in progress
        /// </summary>
        public bool IsInProgress => StepStatus?.Equals("In Progress", StringComparison.OrdinalIgnoreCase) == true;

        /// <summary>
        /// Whether step failed
        /// </summary>
        public bool IsFailed => StepStatus?.Equals("Failed", StringComparison.OrdinalIgnoreCase) == true;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the BatchReportStepViewModel
        /// </summary>
        /// <param name="batchStep">Batch step model</param>
        /// <param name="operationDescriptionService">Operation description service</param>
        /// <param name="logger">Logger instance</param>
        public BatchReportStepViewModel(BatchStepModel batchStep, 
            IOperationDescriptionService operationDescriptionService = null, 
            ILogger logger = null)
        {
            _operationDescriptionService = operationDescriptionService ?? new OperationDescriptionService();
            _logger = logger ?? new Logger();
            
            BatchStep = batchStep ?? throw new ArgumentNullException(nameof(batchStep));
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
                FormattedAmountPreset = $"{AmountPreset:F2} L";
                FormattedAmountActual = $"{AmountActual:F2} L";
                FormattedAmountDifference = $"{AmountDifference:F2} L ({AmountDifferencePercentage:F1}%)";
                
                FormattedTimePreset = FormatTime(TimePresetMinutes);
                FormattedTimeActual = FormatTime(TimeActualMinutes);
                FormattedTimeDifference = FormatTimeDifference(TimeDifferenceMinutes);
                
                FormattedTemperature = Temperature.HasValue ? $"{Temperature.Value:F1}°C" : "N/A";
                FormattedPressure = Pressure.HasValue ? $"{Pressure.Value:F2} bar" : "N/A";
                
                FormattedStepStartTime = StepStartTimestamp?.ToString("HH:mm:ss") ?? "N/A";
                FormattedStepEndTime = StepEndTimestamp?.ToString("HH:mm:ss") ?? "N/A";
                
                UpdateVarianceIndicators();
                UpdateStatusColors();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating formatted properties: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets a summary of the step
        /// </summary>
        /// <returns>Step summary</returns>
        public string GetSummary()
        {
            try
            {
                return $"Step {StepSequence}: {OperationType} - {FormattedAmountActual} in {FormattedTimeActual}";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting step summary: {ex.Message}", ex);
                return $"Step {StepSequence}: Error getting summary";
            }
        }

        /// <summary>
        /// Gets detailed information about the step
        /// </summary>
        /// <returns>Detailed step information</returns>
        public string GetDetailedInfo()
        {
            try
            {
                return $"Step {StepSequence}: {OperationType}\n" +
                       $"Description: {OperationDescription}\n" +
                       $"Ingredient: {IngredientName}\n" +
                       $"Preset Amount: {FormattedAmountPreset}\n" +
                       $"Actual Amount: {FormattedAmountActual}\n" +
                       $"Difference: {FormattedAmountDifference}\n" +
                       $"Preset Time: {FormattedTimePreset}\n" +
                       $"Actual Time: {FormattedTimeActual}\n" +
                       $"Time Difference: {FormattedTimeDifference}\n" +
                       $"Temperature: {FormattedTemperature}\n" +
                       $"Pressure: {FormattedPressure}\n" +
                       $"Status: {StepStatus}\n" +
                       $"Required: {IsRequired}\n" +
                       $"Skipped: {IsSkipped}\n" +
                       $"Priority: {Priority}";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting detailed step info: {ex.Message}", ex);
                return $"Error getting detailed information: {ex.Message}";
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Formats time in minutes to display string
        /// </summary>
        /// <param name="minutes">Time in minutes</param>
        /// <returns>Formatted time string</returns>
        private string FormatTime(int minutes)
        {
            if (minutes < 60)
            {
                return $"{minutes}m";
            }
            else if (minutes < 1440) // Less than 24 hours
            {
                var hours = minutes / 60;
                var remainingMinutes = minutes % 60;
                return remainingMinutes > 0 ? $"{hours}h {remainingMinutes}m" : $"{hours}h";
            }
            else
            {
                var days = minutes / 1440;
                var remainingHours = (minutes % 1440) / 60;
                return remainingHours > 0 ? $"{days}d {remainingHours}h" : $"{days}d";
            }
        }

        /// <summary>
        /// Formats time difference for display
        /// </summary>
        /// <param name="differenceMinutes">Time difference in minutes</param>
        /// <returns>Formatted time difference string</returns>
        private string FormatTimeDifference(int differenceMinutes)
        {
            var prefix = differenceMinutes >= 0 ? "+" : "";
            return $"{prefix}{FormatTime(Math.Abs(differenceMinutes))}";
        }

        /// <summary>
        /// Updates variance indicators
        /// </summary>
        private void UpdateVarianceIndicators()
        {
            // Amount variance indicators
            IsAmountVarianceHigh = Math.Abs(AmountDifferencePercentage) > 10; // 10% threshold
            
            // Time variance indicators
            var timeVariancePercentage = TimePresetMinutes > 0 ? 
                (TimeDifferenceMinutes / (double)TimePresetMinutes) * 100 : 0;
            IsTimeVarianceHigh = Math.Abs(timeVariancePercentage) > 15; // 15% threshold
            
            // Overall variance indicator
            if (IsAmountVarianceHigh && IsTimeVarianceHigh)
            {
                VarianceIndicator = "⚠️ High Variance";
            }
            else if (IsAmountVarianceHigh || IsTimeVarianceHigh)
            {
                VarianceIndicator = "⚠️ Variance";
            }
            else
            {
                VarianceIndicator = "✅ Normal";
            }
        }

        /// <summary>
        /// Updates status colors
        /// </summary>
        private void UpdateStatusColors()
        {
            // Amount difference color
            if (Math.Abs(AmountDifferencePercentage) <= 5)
            {
                AmountDifferenceColor = "Green";
            }
            else if (Math.Abs(AmountDifferencePercentage) <= 10)
            {
                AmountDifferenceColor = "Orange";
            }
            else
            {
                AmountDifferenceColor = "Red";
            }
            
            // Time difference color
            var timeVariancePercentage = TimePresetMinutes > 0 ? 
                (TimeDifferenceMinutes / (double)TimePresetMinutes) * 100 : 0;
            
            if (Math.Abs(timeVariancePercentage) <= 10)
            {
                TimeDifferenceColor = "Green";
            }
            else if (Math.Abs(timeVariancePercentage) <= 20)
            {
                TimeDifferenceColor = "Orange";
            }
            else
            {
                TimeDifferenceColor = "Red";
            }
            
            // Step status color
            StepStatusColor = StepStatus switch
            {
                "Completed" => "Green",
                "In Progress" => "Blue",
                "Failed" => "Red",
                "Skipped" => "Gray",
                "Pending" => "Orange",
                _ => "Gray"
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
