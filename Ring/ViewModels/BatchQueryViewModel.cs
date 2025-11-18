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
    /// ViewModel for batch query operations with comprehensive filtering and export capabilities
    /// </summary>
    public class BatchQueryViewModel : INotifyPropertyChanged
    {
        private readonly BatchQueryService _batchQueryService;
        private readonly ExportService _exportService;
        private readonly ILogger _logger;
        private readonly ValidationService _validationService;
        private ObservableCollection<BatchQuery> _batches;
        private BatchQueryFilter _currentFilter;
        private BatchQuery _selectedBatch;
        private bool _isLoading;
        private string _statusMessage;
        private DateTime _startDate;
        private DateTime _endDate;
        private List<int> _availableFormulaNumbers;
        private List<int> _availableStorageTankNumbers;
        private DateTime _reportDateTime;
        private string _pageInfo;

        /// <summary>
        /// Initializes a new instance of the BatchQueryViewModel
        /// </summary>
        public BatchQueryViewModel()
        {
            _logger = new Logger();
            _validationService = new ValidationService(_logger);
            _batchQueryService = new BatchQueryService(_logger, _validationService);
            _exportService = new ExportService();
            _batches = new ObservableCollection<BatchQuery>();
            _currentFilter = new BatchQueryFilter();
            _startDate = DateTime.Today.AddDays(-30); // Default to last 30 days
            _endDate = DateTime.Today;
            _availableFormulaNumbers = new List<int>();
            _availableStorageTankNumbers = new List<int>();
            _statusMessage = "Ready";
            _reportDateTime = DateTime.Now;
            _pageInfo = "1 of 1";

            // Initialize commands
            SearchCommand = new RelayCommand(async () => await SearchBatchesAsync(), () => !IsLoading);
            ClearFilterCommand = new RelayCommand(ClearFilters, () => !IsLoading);
            ExportToExcelCommand = new RelayCommand(ExportToExcel, () => !IsLoading && Batches.Any());
            ExportToPdfCommand = new RelayCommand(ExportToPdf, () => !IsLoading && Batches.Any());
            PrintCommand = new RelayCommand(PrintBatches, () => !IsLoading && Batches.Any());
            RefreshCommand = new RelayCommand(async () => await RefreshDataAsync(), () => !IsLoading);

            // Load initial data
            _ = Task.Run(async () => await LoadInitialDataAsync());
        }

        #region Properties

        /// <summary>
        /// Collection of batch queries
        /// </summary>
        public ObservableCollection<BatchQuery> Batches
        {
            get => _batches;
            set
            {
                _batches = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasBatches));
            }
        }

        /// <summary>
        /// Current filter criteria
        /// </summary>
        public BatchQueryFilter CurrentFilter
        {
            get => _currentFilter;
            set
            {
                _currentFilter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Currently selected batch
        /// </summary>
        public BatchQuery SelectedBatch
        {
            get => _selectedBatch;
            set
            {
                _selectedBatch = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedBatch));
            }
        }

        /// <summary>
        /// Indicates if data is currently loading
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotLoading));
                UpdateCommandStates();
            }
        }

        /// <summary>
        /// Current status message
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
        /// Start date for date range filtering
        /// </summary>
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
                CurrentFilter.StartDate = value;
            }
        }

        /// <summary>
        /// End date for date range filtering
        /// </summary>
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
                CurrentFilter.EndDate = value;
            }
        }

        /// <summary>
        /// Available formula numbers for filtering
        /// </summary>
        public List<int> AvailableFormulaNumbers
        {
            get => _availableFormulaNumbers;
            set
            {
                _availableFormulaNumbers = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Available storage tank numbers for filtering
        /// </summary>
        public List<int> AvailableStorageTankNumbers
        {
            get => _availableStorageTankNumbers;
            set
            {
                _availableStorageTankNumbers = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicates if there are any batches loaded
        /// </summary>
        public bool HasBatches => Batches?.Any() == true;

        /// <summary>
        /// Indicates if a batch is currently selected
        /// </summary>
        public bool HasSelectedBatch => SelectedBatch != null;

        /// <summary>
        /// Indicates if data is not currently loading
        /// </summary>
        public bool IsNotLoading => !IsLoading;

        /// <summary>
        /// Current report date and time for display
        /// </summary>
        public DateTime ReportDateTime
        {
            get => _reportDateTime;
            set
            {
                _reportDateTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Page information for display
        /// </summary>
        public string PageInfo
        {
            get => _pageInfo;
            set
            {
                _pageInfo = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command to search for batches based on current filter
        /// </summary>
        public ICommand SearchCommand { get; }

        /// <summary>
        /// Command to clear all filter criteria
        /// </summary>
        public ICommand ClearFilterCommand { get; }

        /// <summary>
        /// Command to export batches to Excel
        /// </summary>
        public ICommand ExportToExcelCommand { get; }

        /// <summary>
        /// Command to export batches to PDF
        /// </summary>
        public ICommand ExportToPdfCommand { get; }

        /// <summary>
        /// Command to print batches
        /// </summary>
        public ICommand PrintCommand { get; }

        /// <summary>
        /// Command to refresh all data
        /// </summary>
        public ICommand RefreshCommand { get; }

        #endregion

        #region Command Implementations

        /// <summary>
        /// Searches for batches based on current filter criteria
        /// </summary>
        private async Task SearchBatchesAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Searching batches...";

                var batches = await _batchQueryService.GetBatchesAsync(CurrentFilter);
                
                Batches.Clear();
                foreach (var batch in batches)
                {
                    Batches.Add(batch);
                }

                StatusMessage = $"Found {batches.Count} batches";
                ReportDateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error searching batches: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error in SearchBatchesAsync: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Clears all filter criteria and refreshes data
        /// </summary>
        private void ClearFilters()
        {
            try
            {
                CurrentFilter.ClearFilters();
                StartDate = DateTime.Today.AddDays(-30);
                EndDate = DateTime.Today;
                StatusMessage = "Filters cleared";
                
                // Trigger search with cleared filters
                _ = Task.Run(async () => await SearchBatchesAsync());
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error clearing filters: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error in ClearFilters: {ex}");
            }
        }

        /// <summary>
        /// Exports current batches to Excel format
        /// </summary>
        private async void ExportToExcel()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Exporting to Excel...";

                // Show save file dialog
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*",
                    DefaultExt = "xlsx",
                    FileName = $"BatchReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    await _exportService.ExportToExcelAsync(Batches.ToList(), saveFileDialog.FileName);
                    StatusMessage = $"Successfully exported {Batches.Count} batches to Excel";
                }
                else
                {
                    StatusMessage = "Excel export cancelled";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error exporting to Excel: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error in ExportToExcel: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Exports current batches to PDF format
        /// </summary>
        private async void ExportToPdf()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Exporting to PDF...";

                // Show save file dialog
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*",
                    DefaultExt = "pdf",
                    FileName = $"BatchReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    await _exportService.ExportToPdfAsync(Batches.ToList(), saveFileDialog.FileName);
                    StatusMessage = $"Successfully exported {Batches.Count} batches to PDF";
                }
                else
                {
                    StatusMessage = "PDF export cancelled";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error exporting to PDF: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error in ExportToPdf: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Prints current batches
        /// </summary>
        private async void PrintBatches()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Preparing print job...";

                await _exportService.PrintReportAsync(Batches.ToList());
                StatusMessage = $"Printed {Batches.Count} batches";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error printing batches: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error in PrintBatches: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Refreshes all data including available formula and tank numbers
        /// </summary>
        private async Task RefreshDataAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Refreshing data...";

                // Load available formula numbers
                var formulaNumbers = await _batchQueryService.GetAvailableFormulaNumbersAsync();
                AvailableFormulaNumbers = formulaNumbers;

                // Load available storage tank numbers
                var tankNumbers = await _batchQueryService.GetAvailableStorageTankNumbersAsync();
                AvailableStorageTankNumbers = tankNumbers;

                // Refresh batches
                await SearchBatchesAsync();

                StatusMessage = "Data refreshed successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error refreshing data: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error in RefreshDataAsync: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads initial data when the ViewModel is created
        /// </summary>
        private async Task LoadInitialDataAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading initial data...";

                // Load available formula numbers
                var formulaNumbers = await _batchQueryService.GetAvailableFormulaNumbersAsync();
                AvailableFormulaNumbers = formulaNumbers;

                // Load available storage tank numbers
                var tankNumbers = await _batchQueryService.GetAvailableStorageTankNumbersAsync();
                AvailableStorageTankNumbers = tankNumbers;

                // Load initial batches with default filter
                await SearchBatchesAsync();

                StatusMessage = "Initial data loaded";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading initial data: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error in LoadInitialDataAsync: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Updates the enabled state of all commands
        /// </summary>
        private void UpdateCommandStates()
        {
            if (SearchCommand is RelayCommand searchCmd)
                searchCmd.RaiseCanExecuteChanged();
            if (ClearFilterCommand is RelayCommand clearCmd)
                clearCmd.RaiseCanExecuteChanged();
            if (ExportToExcelCommand is RelayCommand excelCmd)
                excelCmd.RaiseCanExecuteChanged();
            if (ExportToPdfCommand is RelayCommand pdfCmd)
                pdfCmd.RaiseCanExecuteChanged();
            if (PrintCommand is RelayCommand printCmd)
                printCmd.RaiseCanExecuteChanged();
            if (RefreshCommand is RelayCommand refreshCmd)
                refreshCmd.RaiseCanExecuteChanged();
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    /// Simple RelayCommand implementation for MVVM pattern
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

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
