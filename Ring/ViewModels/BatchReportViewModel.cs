using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Ring.Models;
using Ring.Services;

namespace Ring.ViewModels
{
    /// <summary>
    /// Main ViewModel for batch reports that replaces legacy ReportBatchPreviousForm functionality
    /// </summary>
    public class BatchReportViewModel : INotifyPropertyChanged, IDisposable
    {
        #region Private Fields

        private readonly IBatchReportService _batchReportService;
        private readonly IOperationDescriptionService _operationDescriptionService;
        private readonly ILogger _logger;
        private bool _disposed = false;

        private BatchReportModel _selectedBatchReport;
        private ObservableCollection<BatchReportItemViewModel> _batchReports;
        private ObservableCollection<BatchReportStepViewModel> _batchSteps;
        private ObservableCollection<BatchIngredientModel> _batchIngredients;
        private bool _isLoading;
        private string _loadingMessage;
        private string _errorMessage;
        private string _statusMessage;
        private bool _useBasicNames;
        private int _selectedPCID;
        private DateTime _startDate;
        private DateTime _endDate;
        private int _selectedFormulaNumber;
        private int _selectedStorageTankNumber;
        private int _selectedStatus;
        private string _searchText;
        private bool _isReportGenerating;
        private double _reportGenerationProgress;
        private string _reportGenerationStatus;

        #endregion

        #region Public Properties

        /// <summary>
        /// Selected batch report
        /// </summary>
        public BatchReportModel SelectedBatchReport
        {
            get => _selectedBatchReport;
            set
            {
                if (_selectedBatchReport != value)
                {
                    _selectedBatchReport = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsBatchSelected));
                    OnPropertyChanged(nameof(CanGenerateReport));
                    OnPropertyChanged(nameof(CanExportReport));
                    OnPropertyChanged(nameof(CanPrintReport));
                }
            }
        }

        /// <summary>
        /// Collection of batch reports
        /// </summary>
        public ObservableCollection<BatchReportItemViewModel> BatchReports
        {
            get => _batchReports;
            set
            {
                _batchReports = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Collection of batch steps
        /// </summary>
        public ObservableCollection<BatchReportStepViewModel> BatchSteps
        {
            get => _batchSteps;
            set
            {
                _batchSteps = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Collection of batch ingredients
        /// </summary>
        public ObservableCollection<BatchIngredientModel> BatchIngredients
        {
            get => _batchIngredients;
            set
            {
                _batchIngredients = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether data is loading
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotLoading));
            }
        }

        /// <summary>
        /// Whether data is not loading
        /// </summary>
        public bool IsNotLoading => !_isLoading;

        /// <summary>
        /// Loading message
        /// </summary>
        public string LoadingMessage
        {
            get => _loadingMessage;
            set
            {
                _loadingMessage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasError));
            }
        }

        /// <summary>
        /// Status message
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether to use basic names
        /// </summary>
        public bool UseBasicNames
        {
            get => _useBasicNames;
            set
            {
                _useBasicNames = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UseFlexibleNames));
            }
        }

        /// <summary>
        /// Whether to use flexible names
        /// </summary>
        public bool UseFlexibleNames => !_useBasicNames;

        /// <summary>
        /// Selected PCID
        /// </summary>
        public int SelectedPCID
        {
            get => _selectedPCID;
            set
            {
                _selectedPCID = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanGenerateReport));
            }
        }

        /// <summary>
        /// Start date for filtering
        /// </summary>
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSearch));
            }
        }

        /// <summary>
        /// End date for filtering
        /// </summary>
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSearch));
            }
        }

        /// <summary>
        /// Selected formula number
        /// </summary>
        public int SelectedFormulaNumber
        {
            get => _selectedFormulaNumber;
            set
            {
                _selectedFormulaNumber = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSearch));
            }
        }

        /// <summary>
        /// Selected storage tank number
        /// </summary>
        public int SelectedStorageTankNumber
        {
            get => _selectedStorageTankNumber;
            set
            {
                _selectedStorageTankNumber = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSearch));
            }
        }

        /// <summary>
        /// Selected status
        /// </summary>
        public int SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSearch));
            }
        }

        /// <summary>
        /// Search text
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSearch));
            }
        }

        /// <summary>
        /// Whether report is being generated
        /// </summary>
        public bool IsReportGenerating
        {
            get => _isReportGenerating;
            set
            {
                _isReportGenerating = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanGenerateReport));
                OnPropertyChanged(nameof(CanExportReport));
                OnPropertyChanged(nameof(CanPrintReport));
            }
        }

        /// <summary>
        /// Report generation progress (0-100)
        /// </summary>
        public double ReportGenerationProgress
        {
            get => _reportGenerationProgress;
            set
            {
                _reportGenerationProgress = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Report generation status
        /// </summary>
        public string ReportGenerationStatus
        {
            get => _reportGenerationStatus;
            set
            {
                _reportGenerationStatus = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether a batch is selected
        /// </summary>
        public bool IsBatchSelected => SelectedBatchReport != null;

        /// <summary>
        /// Whether there is an error
        /// </summary>
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        /// <summary>
        /// Whether search can be performed
        /// </summary>
        public bool CanSearch => !IsLoading && !IsReportGenerating;

        /// <summary>
        /// Whether report can be generated
        /// </summary>
        public bool CanGenerateReport => !IsLoading && !IsReportGenerating && (SelectedPCID > 0 || IsBatchSelected);

        /// <summary>
        /// Whether report can be exported
        /// </summary>
        public bool CanExportReport => !IsLoading && !IsReportGenerating && IsBatchSelected;

        /// <summary>
        /// Whether report can be printed
        /// </summary>
        public bool CanPrintReport => !IsLoading && !IsReportGenerating && IsBatchSelected;

        #endregion

        #region Commands

        /// <summary>
        /// Command to generate batch report
        /// </summary>
        public ICommand GenerateReportCommand { get; }

        /// <summary>
        /// Command to search batch reports
        /// </summary>
        public ICommand SearchReportsCommand { get; }

        /// <summary>
        /// Command to clear search
        /// </summary>
        public ICommand ClearSearchCommand { get; }

        /// <summary>
        /// Command to export report
        /// </summary>
        public ICommand ExportReportCommand { get; }

        /// <summary>
        /// Command to print report
        /// </summary>
        public ICommand PrintReportCommand { get; }

        /// <summary>
        /// Command to refresh data
        /// </summary>
        public ICommand RefreshCommand { get; }

        /// <summary>
        /// Command to select batch
        /// </summary>
        public ICommand SelectBatchCommand { get; }

        /// <summary>
        /// Command to clear error
        /// </summary>
        public ICommand ClearErrorCommand { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the BatchReportViewModel
        /// </summary>
        /// <param name="batchReportService">Batch report service</param>
        /// <param name="operationDescriptionService">Operation description service</param>
        /// <param name="logger">Logger instance</param>
        public BatchReportViewModel(IBatchReportService batchReportService = null, 
            IOperationDescriptionService operationDescriptionService = null, 
            ILogger logger = null)
        {
            _batchReportService = batchReportService ?? new BatchReportService();
            _operationDescriptionService = operationDescriptionService ?? new OperationDescriptionService();
            _logger = logger ?? new Logger();

            // Initialize collections
            _batchReports = new ObservableCollection<BatchReportItemViewModel>();
            _batchSteps = new ObservableCollection<BatchReportStepViewModel>();
            _batchIngredients = new ObservableCollection<BatchIngredientModel>();

            // Initialize properties
            _startDate = DateTime.Now.AddDays(-30);
            _endDate = DateTime.Now;
            _selectedFormulaNumber = 0;
            _selectedStorageTankNumber = 0;
            _selectedStatus = -1;
            _searchText = string.Empty;
            _useBasicNames = false;

            // Initialize commands
            GenerateReportCommand = new RelayCommand(async () => await GenerateReportAsync(), () => CanGenerateReport);
            SearchReportsCommand = new RelayCommand(async () => await SearchReportsAsync(), () => CanSearch);
            ClearSearchCommand = new RelayCommand(ClearSearch, () => CanSearch);
            ExportReportCommand = new RelayCommand(async () => await ExportReportAsync(), () => CanExportReport);
            PrintReportCommand = new RelayCommand(async () => await PrintReportAsync(), () => CanPrintReport);
            RefreshCommand = new RelayCommand(async () => await RefreshAsync(), () => CanSearch);
            SelectBatchCommand = new RelayCommand<BatchReportItemViewModel>(SelectBatch);
            ClearErrorCommand = new RelayCommand(ClearError);

            // Load initial data
            _ = Task.Run(async () => await LoadInitialDataAsync());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generates batch report for selected PCID
        /// </summary>
        public async Task GenerateReportAsync()
        {
            try
            {
                IsReportGenerating = true;
                ReportGenerationProgress = 0;
                ReportGenerationStatus = "Initializing report generation...";

                var pcid = SelectedPCID > 0 ? SelectedPCID : SelectedBatchReport?.BatchId ?? 0;
                if (pcid <= 0)
                {
                    ErrorMessage = "Please select a valid batch PCID.";
                    return;
                }

                ReportGenerationProgress = 20;
                ReportGenerationStatus = "Loading batch information...";

                // Generate batch report
                var response = await _batchReportService.GenerateBatchReportAsync(pcid, UseBasicNames);
                
                if (response?.BatchReports?.Any() == true)
                {
                    ReportGenerationProgress = 60;
                    ReportGenerationStatus = "Processing batch steps...";

                    var batchReport = response.BatchReports.First();
                    SelectedBatchReport = batchReport;

                    // Load batch steps
                    var steps = await _batchReportService.GetBatchStepsAsync(pcid, UseBasicNames);
                    await LoadBatchStepsAsync(steps);

                    ReportGenerationProgress = 80;
                    ReportGenerationStatus = "Loading batch ingredients...";

                    // Load batch ingredients
                    var ingredients = await _batchReportService.GetBatchIngredientsAsync(pcid, UseBasicNames);
                    await LoadBatchIngredientsAsync(ingredients);

                    ReportGenerationProgress = 100;
                    ReportGenerationStatus = "Report generation completed successfully.";

                    StatusMessage = $"Batch report generated successfully for PCID: {pcid}";
                    ErrorMessage = string.Empty;
                }
                else
                {
                    ErrorMessage = "Failed to generate batch report. No data returned.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating batch report: {ex.Message}", ex);
                ErrorMessage = $"Error generating batch report: {ex.Message}";
            }
            finally
            {
                IsReportGenerating = false;
                ReportGenerationProgress = 0;
                ReportGenerationStatus = string.Empty;
            }
        }

        /// <summary>
        /// Searches for batch reports
        /// </summary>
        public async Task SearchReportsAsync()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Searching batch reports...";
                ErrorMessage = string.Empty;

                var request = new BatchReportRequest
                {
                    StartDate = StartDate,
                    EndDate = EndDate,
                    FormulaNumber = SelectedFormulaNumber > 0 ? SelectedFormulaNumber : null,
                    StorageTankNumber = SelectedStorageTankNumber > 0 ? SelectedStorageTankNumber : null,
                    Status = SelectedStatus >= 0 ? SelectedStatus : null,
                    SearchText = !string.IsNullOrEmpty(SearchText) ? SearchText : null,
                    MaxBatches = 100
                };

                var response = await _batchReportService.GenerateBatchReportAsync(request);
                
                if (response?.BatchReports?.Any() == true)
                {
                    await LoadBatchReportsAsync(response.BatchReports);
                    StatusMessage = $"Found {response.BatchReports.Count} batch reports.";
                }
                else
                {
                    BatchReports.Clear();
                    StatusMessage = "No batch reports found matching the search criteria.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching batch reports: {ex.Message}", ex);
                ErrorMessage = $"Error searching batch reports: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                LoadingMessage = string.Empty;
            }
        }

        /// <summary>
        /// Clears search criteria
        /// </summary>
        public void ClearSearch()
        {
            StartDate = DateTime.Now.AddDays(-30);
            EndDate = DateTime.Now;
            SelectedFormulaNumber = 0;
            SelectedStorageTankNumber = 0;
            SelectedStatus = -1;
            SearchText = string.Empty;
            BatchReports.Clear();
            StatusMessage = "Search criteria cleared.";
        }

        /// <summary>
        /// Exports the current report
        /// </summary>
        public async Task ExportReportAsync()
        {
            try
            {
                if (SelectedBatchReport == null)
                {
                    ErrorMessage = "No batch report selected for export.";
                    return;
                }

                IsLoading = true;
                LoadingMessage = "Exporting report...";

                // Create export dialog or use default format
                var format = "PDF"; // Default format
                var response = await _batchReportService.GenerateBatchReportAsync(SelectedBatchReport.BatchId, UseBasicNames);
                
                if (response?.BatchReports?.Any() == true)
                {
                    var exportResult = await _batchReportService.ExportBatchReportAsync(response, format);
                    
                    if (exportResult.IsSuccess)
                    {
                        StatusMessage = $"Report exported successfully to: {exportResult.FilePath}";
                    }
                    else
                    {
                        ErrorMessage = $"Export failed: {string.Join(", ", exportResult.Errors)}";
                    }
                }
                else
                {
                    ErrorMessage = "Failed to generate report for export.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting report: {ex.Message}", ex);
                ErrorMessage = $"Error exporting report: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                LoadingMessage = string.Empty;
            }
        }

        /// <summary>
        /// Prints the current report
        /// </summary>
        public async Task PrintReportAsync()
        {
            try
            {
                if (SelectedBatchReport == null)
                {
                    ErrorMessage = "No batch report selected for printing.";
                    return;
                }

                IsLoading = true;
                LoadingMessage = "Preparing report for printing...";

                // Generate report for printing
                var response = await _batchReportService.GenerateBatchReportAsync(SelectedBatchReport.BatchId, UseBasicNames);
                
                if (response?.BatchReports?.Any() == true)
                {
                    // Export to PDF for printing
                    var exportResult = await _batchReportService.ExportBatchReportAsync(response, "PDF");
                    
                    if (exportResult.IsSuccess)
                    {
                        StatusMessage = "Report prepared for printing successfully.";
                        // Here you would typically open the PDF in the default printer dialog
                    }
                    else
                    {
                        ErrorMessage = $"Print preparation failed: {string.Join(", ", exportResult.Errors)}";
                    }
                }
                else
                {
                    ErrorMessage = "Failed to generate report for printing.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error printing report: {ex.Message}", ex);
                ErrorMessage = $"Error printing report: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                LoadingMessage = string.Empty;
            }
        }

        /// <summary>
        /// Refreshes the current data
        /// </summary>
        public async Task RefreshAsync()
        {
            await SearchReportsAsync();
        }

        /// <summary>
        /// Selects a batch report
        /// </summary>
        /// <param name="batchItem">Batch report item to select</param>
        public void SelectBatch(BatchReportItemViewModel batchItem)
        {
            if (batchItem?.BatchReport != null)
            {
                SelectedBatchReport = batchItem.BatchReport;
                SelectedPCID = batchItem.BatchReport.BatchId;
                StatusMessage = $"Selected batch report: {batchItem.BatchReport.BatchId}";
            }
        }

        /// <summary>
        /// Clears error message
        /// </summary>
        public void ClearError()
        {
            ErrorMessage = string.Empty;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads initial data
        /// </summary>
        private async Task LoadInitialDataAsync()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Loading initial data...";

                // Load recent batch reports
                await SearchReportsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading initial data: {ex.Message}", ex);
                ErrorMessage = $"Error loading initial data: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                LoadingMessage = string.Empty;
            }
        }

        /// <summary>
        /// Loads batch reports into the collection
        /// </summary>
        /// <param name="batchReports">Batch reports to load</param>
        private async Task LoadBatchReportsAsync(List<BatchReportModel> batchReports)
        {
            try
            {
                BatchReports.Clear();

                foreach (var batchReport in batchReports)
                {
                    var batchItem = new BatchReportItemViewModel(batchReport, _operationDescriptionService, _logger);
                    BatchReports.Add(batchItem);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading batch reports: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Loads batch steps into the collection
        /// </summary>
        /// <param name="batchSteps">Batch steps to load</param>
        private async Task LoadBatchStepsAsync(List<BatchStepModel> batchSteps)
        {
            try
            {
                BatchSteps.Clear();

                foreach (var batchStep in batchSteps)
                {
                    var stepItem = new BatchReportStepViewModel(batchStep, _operationDescriptionService, _logger);
                    BatchSteps.Add(stepItem);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading batch steps: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Loads batch ingredients into the collection
        /// </summary>
        /// <param name="batchIngredients">Batch ingredients to load</param>
        private async Task LoadBatchIngredientsAsync(List<BatchIngredientModel> batchIngredients)
        {
            try
            {
                BatchIngredients.Clear();

                foreach (var ingredient in batchIngredients)
                {
                    BatchIngredients.Add(ingredient);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading batch ingredients: {ex.Message}", ex);
                throw;
            }
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
                _batchReportService?.Dispose();
                _disposed = true;
            }
        }

        #endregion
    }

    /// <summary>
    /// Relay command implementation for MVVM
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }

    /// <summary>
    /// Relay command with parameter implementation for MVVM
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke((T)parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
}
