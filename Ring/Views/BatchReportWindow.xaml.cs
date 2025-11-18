using System;
using System.Windows;
using Ring.ViewModels;

namespace Ring.Views
{
    /// <summary>
    /// Interaction logic for BatchReportWindow.xaml
    /// </summary>
    public partial class BatchReportWindow : Window
    {
        private readonly BatchReportViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the BatchReportWindow
        /// </summary>
        /// <param name="viewModel">Batch report view model</param>
        public BatchReportWindow(BatchReportViewModel viewModel = null)
        {
            InitializeComponent();
            
            _viewModel = viewModel ?? new BatchReportViewModel();
            DataContext = _viewModel;
            
            // Subscribe to events
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            
            // Initialize window
            InitializeWindow();
        }

        /// <summary>
        /// Initializes the window
        /// </summary>
        private void InitializeWindow()
        {
            try
            {
                // Set window properties
                Title = "Batch Report Manager - RS3000";
                WindowState = WindowState.Normal;
                
                // Set initial focus
                PCIDTextBox.Focus();
                
                // Initialize combo boxes
                InitializeComboBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing window: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Initializes combo boxes with data
        /// </summary>
        private void InitializeComboBoxes()
        {
            try
            {
                // Initialize formula combo box
                FormulaComboBox.SelectedIndex = 0;
                
                // Initialize tank combo box
                TankComboBox.SelectedIndex = 0;
                
                // Initialize status combo box
                StatusComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing combo boxes: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles property changed events from the view model
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                switch (e.PropertyName)
                {
                    case nameof(_viewModel.IsLoading):
                        UpdateLoadingState();
                        break;
                    case nameof(_viewModel.IsReportGenerating):
                        UpdateReportGenerationState();
                        break;
                    case nameof(_viewModel.ErrorMessage):
                        UpdateErrorState();
                        break;
                    case nameof(_viewModel.StatusMessage):
                        UpdateStatusMessage();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error handling property change: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Updates the loading state
        /// </summary>
        private void UpdateLoadingState()
        {
            try
            {
                // Update UI elements based on loading state
                if (_viewModel.IsLoading)
                {
                    // Disable controls during loading
                    StartDatePicker.IsEnabled = false;
                    EndDatePicker.IsEnabled = false;
                    FormulaComboBox.IsEnabled = false;
                    TankComboBox.IsEnabled = false;
                    StatusComboBox.IsEnabled = false;
                    SearchTextBox.IsEnabled = false;
                    PCIDTextBox.IsEnabled = false;
                    BasicNamesCheckBox.IsEnabled = false;
                }
                else
                {
                    // Re-enable controls after loading
                    StartDatePicker.IsEnabled = true;
                    EndDatePicker.IsEnabled = true;
                    FormulaComboBox.IsEnabled = true;
                    TankComboBox.IsEnabled = true;
                    StatusComboBox.IsEnabled = true;
                    SearchTextBox.IsEnabled = true;
                    PCIDTextBox.IsEnabled = true;
                    BasicNamesCheckBox.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating loading state: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Updates the report generation state
        /// </summary>
        private void UpdateReportGenerationState()
        {
            try
            {
                // Update UI elements based on report generation state
                if (_viewModel.IsReportGenerating)
                {
                    // Disable controls during report generation
                    StartDatePicker.IsEnabled = false;
                    EndDatePicker.IsEnabled = false;
                    FormulaComboBox.IsEnabled = false;
                    TankComboBox.IsEnabled = false;
                    StatusComboBox.IsEnabled = false;
                    SearchTextBox.IsEnabled = false;
                    PCIDTextBox.IsEnabled = false;
                    BasicNamesCheckBox.IsEnabled = false;
                }
                else
                {
                    // Re-enable controls after report generation
                    StartDatePicker.IsEnabled = true;
                    EndDatePicker.IsEnabled = true;
                    FormulaComboBox.IsEnabled = true;
                    TankComboBox.IsEnabled = true;
                    StatusComboBox.IsEnabled = true;
                    SearchTextBox.IsEnabled = true;
                    PCIDTextBox.IsEnabled = true;
                    BasicNamesCheckBox.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating report generation state: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Updates the error state
        /// </summary>
        private void UpdateErrorState()
        {
            try
            {
                // Error state is handled by the XAML binding
                // This method can be used for additional error handling if needed
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating error state: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Updates the status message
        /// </summary>
        private void UpdateStatusMessage()
        {
            try
            {
                // Status message is handled by the XAML binding
                // This method can be used for additional status handling if needed
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating status message: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles window closing event
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                // Unsubscribe from events
                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                }

                // Dispose view model
                _viewModel?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error closing window: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles window loaded event
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Set initial focus
                PCIDTextBox.Focus();
                
                // Load initial data if not already loaded
                if (_viewModel.BatchReports.Count == 0)
                {
                    _ = _viewModel.RefreshAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading window: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles key down events
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.F5:
                        // Refresh data
                        _ = _viewModel.RefreshAsync();
                        e.Handled = true;
                        break;
                    case System.Windows.Input.Key.Escape:
                        // Clear error message
                        _viewModel.ClearError();
                        e.Handled = true;
                        break;
                    case System.Windows.Input.Key.Enter:
                        // Generate report if PCID is entered
                        if (PCIDTextBox.IsFocused && !string.IsNullOrEmpty(PCIDTextBox.Text))
                        {
                            _ = _viewModel.GenerateReportAsync();
                            e.Handled = true;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error handling key press: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles PCID text box text changed event
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void PCIDTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                // Validate PCID input
                if (int.TryParse(PCIDTextBox.Text, out int pcid))
                {
                    _viewModel.SelectedPCID = pcid;
                }
                else if (string.IsNullOrEmpty(PCIDTextBox.Text))
                {
                    _viewModel.SelectedPCID = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error handling PCID text change: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles search text box text changed event
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                // Update search text in view model
                _viewModel.SearchText = SearchTextBox.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error handling search text change: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles basic names check box checked event
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void BasicNamesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Update basic names setting in view model
                _viewModel.UseBasicNames = BasicNamesCheckBox.IsChecked == true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error handling basic names check: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles basic names check box unchecked event
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void BasicNamesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Update basic names setting in view model
                _viewModel.UseBasicNames = BasicNamesCheckBox.IsChecked == true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error handling basic names uncheck: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
