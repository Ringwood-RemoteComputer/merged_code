using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Ring.Services.PLC;
using Ring;

namespace Ring.Views.BatchStart
{
    /// <summary>
    /// Interaction logic for Lowmidtank.xaml
    /// </summary>
    public partial class Lowmidtank : Window
    {
        private const int MIN_BATCH_VOLUME = 900;
        private const int MAX_BATCH_VOLUME = 1350;
        private const int MIN_BATCH_START_LEVEL = 0;
        private const int MAX_BATCH_START_LEVEL = 2500;

        // PLC Configuration
        private const string PLC_IP = "192.168.202.10";
        private const string PLC_PATH = "1,0";
        private const int TANK_INDEX = 2; // Tanks[2] for Storage Tank 3
        
        private bool _simulationMode => MainWindow.IsSimulationMode;

        // PLC Tag Readers
        private PlcTagReader _formulaNumberReader;
        private PlcTagReader _requestLevelReader;
        private PlcTagReader _startTypeReader;
        private PlcTagReader _splittingAReader;
        private PlcTagReader _splittingBReader;

        // PLC Tag Writers
        private PlcTagWriter _formulaNumberWriter;
        private PlcTagWriter _requestLevelWriter;
        private PlcTagWriter _startTypeWriter;
        private PlcTagWriter _splittingAWriter;
        private PlcTagWriter _splittingBWriter;

        // Update timer
        private DispatcherTimer _plcUpdateTimer;
        private bool _isUpdatingFromPlc = false; // Prevent circular updates

        public Lowmidtank()
        {
            InitializeComponent();
            
            // Wait for window to load before initializing
            this.Loaded += Lowmidtank_Loaded;
            this.Closing += Lowmidtank_Closing;
        }

        private void Lowmidtank_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize PLC readers/writers
            InitializePlcTags();
            
            // Subscribe to text changes for summary updates
            if (BatchVolumeTextBox != null)
            {
                BatchVolumeTextBox.TextChanged += BatchVolumeTextBox_TextChanged;
            }
            if (BatchStartLevelTextBox != null)
            {
                BatchStartLevelTextBox.TextChanged += BatchStartLevelTextBox_TextChanged;
            }
            if (FormulaListBox != null)
            {
                FormulaListBox.SelectionChanged += FormulaListBox_SelectionChanged;
            }
            
            // Read initial values from PLC
            ReadFromPlc();
            
            // Start periodic PLC read timer (every 2 seconds)
            StartPlcUpdateTimer();
            
            // Initial summary update after everything is loaded
            UpdateSummary();
        }

        private void Lowmidtank_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Stop and dispose timer
            if (_plcUpdateTimer != null)
            {
                _plcUpdateTimer.Stop();
                _plcUpdateTimer = null;
            }
        }

        private void InitializePlcTags()
        {
            try
            {
                if (!_simulationMode)
                {
                    // Initialize readers
                    _formulaNumberReader = new PlcTagReader($"Tanks[{TANK_INDEX}].Formula_number", PLC_IP, PlcDataType.DINT, PLC_PATH);
                    _requestLevelReader = new PlcTagReader($"Tanks[{TANK_INDEX}].Request_level", PLC_IP, PlcDataType.DINT, PLC_PATH);
                    _startTypeReader = new PlcTagReader($"Tanks[{TANK_INDEX}].Start_Type", PLC_IP, PlcDataType.DINT, PLC_PATH);
                    _splittingAReader = new PlcTagReader($"Tanks[{TANK_INDEX}].Splitting_A", PLC_IP, PlcDataType.DINT, PLC_PATH);
                    _splittingBReader = new PlcTagReader($"Tanks[{TANK_INDEX}].Splitting_B", PLC_IP, PlcDataType.DINT, PLC_PATH);

                    // Initialize writers
                    _formulaNumberWriter = new PlcTagWriter($"Tanks[{TANK_INDEX}].Formula_number", PLC_IP, PlcDataType.DINT, PLC_PATH);
                    _requestLevelWriter = new PlcTagWriter($"Tanks[{TANK_INDEX}].Request_level", PLC_IP, PlcDataType.DINT, PLC_PATH);
                    _startTypeWriter = new PlcTagWriter($"Tanks[{TANK_INDEX}].Start_Type", PLC_IP, PlcDataType.DINT, PLC_PATH);
                    _splittingAWriter = new PlcTagWriter($"Tanks[{TANK_INDEX}].Splitting_A", PLC_IP, PlcDataType.DINT, PLC_PATH);
                    _splittingBWriter = new PlcTagWriter($"Tanks[{TANK_INDEX}].Splitting_B", PLC_IP, PlcDataType.DINT, PLC_PATH);

                    Console.WriteLine($"[Lowmidtank] PLC tags initialized for Tanks[{TANK_INDEX}]");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lowmidtank] Error initializing PLC tags: {ex.Message}");
            }
        }

        private void StartPlcUpdateTimer()
        {
            if (_plcUpdateTimer == null)
            {
                _plcUpdateTimer = new DispatcherTimer();
                _plcUpdateTimer.Interval = TimeSpan.FromSeconds(2); // Update every 2 seconds
                _plcUpdateTimer.Tick += (s, e) => ReadFromPlc();
            }
            
            if (!_simulationMode)
            {
                _plcUpdateTimer.Start();
            }
        }

        private void ReadFromPlc()
        {
            if (_simulationMode || _isUpdatingFromPlc)
                return;

            try
            {
                _isUpdatingFromPlc = true;

                // Read Formula Number (1-6)
                string formulaValue = _formulaNumberReader?.Read();
                if (formulaValue != null && formulaValue != "Error" && int.TryParse(formulaValue, out int formulaNum) && formulaNum >= 1 && formulaNum <= 6)
                {
                    UpdateFormulaSelection(formulaNum);
                }

                // Read Request Level (0-2500)
                string levelValue = _requestLevelReader?.Read();
                if (levelValue != null && levelValue != "Error" && int.TryParse(levelValue, out int level) && level >= 0 && level <= 2500)
                {
                    if (BatchStartLevelTextBox != null && BatchStartLevelTextBox.Text != level.ToString())
                    {
                        BatchStartLevelTextBox.Text = level.ToString();
                    }
                }

                // Read Start Type (0=No Batching, 1=Single Batch, 2=Continuous Batching)
                string startTypeValue = _startTypeReader?.Read();
                if (startTypeValue != null && startTypeValue != "Error" && int.TryParse(startTypeValue, out int startType) && startType >= 0 && startType <= 2)
                {
                    UpdateBatchStartType(startType);
                }

                // Read Splitting A (0=No Splitting, 1=Tank2, 2=Tank3, 3=Tank4)
                string splittingAValue = _splittingAReader?.Read();
                if (splittingAValue != null && splittingAValue != "Error" && int.TryParse(splittingAValue, out int splittingA) && splittingA >= 0 && splittingA <= 3)
                {
                    UpdateSplittingTypeA(splittingA);
                }

                // Read Splitting B (0=No Splitting, 1=Tank2, 2=Tank3, 3=Tank4)
                string splittingBValue = _splittingBReader?.Read();
                if (splittingBValue != null && splittingBValue != "Error" && int.TryParse(splittingBValue, out int splittingB) && splittingB >= 0 && splittingB <= 3)
                {
                    UpdateSplittingTypeB(splittingB);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lowmidtank] Error reading from PLC: {ex.Message}");
            }
            finally
            {
                _isUpdatingFromPlc = false;
            }
        }

        private void UpdateFormulaSelection(int formulaNumber)
        {
            if (FormulaListBox == null) return;

            // Formula numbers are 1-6, ListBox items are 0-5 indexed
            int index = formulaNumber - 1;
            if (index >= 0 && index < FormulaListBox.Items.Count)
            {
                FormulaListBox.SelectedIndex = index;
            }
        }

        private void UpdateBatchStartType(int startType)
        {
            // 0=No Batching, 1=Single Batch, 2=Continuous Batching
            var batchTypeButtons = FindVisualChildren<RadioButton>(this)
                .Where(rb => rb.GroupName == "BatchType")
                .ToList();

            if (batchTypeButtons.Count >= 3)
            {
                // Order: No Batching (0), Single Batch (1), Continuous Batching (2)
                if (startType >= 0 && startType < batchTypeButtons.Count)
                {
                    batchTypeButtons[startType].IsChecked = true;
                }
            }
        }

        private void UpdateSplittingTypeA(int splittingType)
        {
            // 0=No Splitting, 1=Tank2, 2=Tank3, 3=Tank4
            var splitAButtons = FindVisualChildren<RadioButton>(this)
                .Where(rb => rb.GroupName == "SplitA")
                .ToList();

            if (splitAButtons.Count >= 4 && splittingType >= 0 && splittingType < splitAButtons.Count)
            {
                splitAButtons[splittingType].IsChecked = true;
            }
        }

        private void UpdateSplittingTypeB(int splittingType)
        {
            // 0=No Splitting, 1=Tank2, 2=Tank3, 3=Tank4
            var splitBButtons = FindVisualChildren<RadioButton>(this)
                .Where(rb => rb.GroupName == "SplitB")
                .ToList();

            if (splitBButtons.Count >= 4 && splittingType >= 0 && splittingType < splitBButtons.Count)
            {
                splitBButtons[splittingType].IsChecked = true;
            }
        }

        private void WriteToPlc(string tagName, int value, PlcTagWriter writer)
        {
            if (_simulationMode || writer == null)
                return;

            try
            {
                bool success = writer.Write(value.ToString());
                if (success)
                {
                    Console.WriteLine($"[Lowmidtank] ? Wrote {tagName} = {value}");
                }
                else
                {
                    Console.WriteLine($"[Lowmidtank] ? Failed to write {tagName} = {value}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lowmidtank] Error writing {tagName}: {ex.Message}");
            }
        }

        private void UpdateSummary()
        {
            try
            {
                // Check if controls are initialized
                if (SummaryTextBlock == null || BatchVolumeTextBox == null || BatchStartLevelTextBox == null)
                {
                    return;
                }

                string formula = "Formula 1";
                if (FormulaListBox != null && FormulaListBox.SelectedItem is ListBoxItem selectedItem)
                {
                    formula = selectedItem.Content.ToString();
                }

                string batchType = "No Batching";
                // Find checked radio button in BatchType group
                var batchTypeRb = FindVisualChildren<RadioButton>(this)
                    .FirstOrDefault(rb => rb.GroupName == "BatchType" && rb.IsChecked == true);
                if (batchTypeRb != null)
                {
                    batchType = batchTypeRb.Content.ToString();
                }

                string splitting = "No splitting";
                // Find checked radio button in SplitA group
                var splitRb = FindVisualChildren<RadioButton>(this)
                    .FirstOrDefault(rb => rb.GroupName == "SplitA" && rb.IsChecked == true);
                if (splitRb != null)
                {
                    string splitText = splitRb.Content.ToString();
                    splitting = splitText == "No Splitting" ? "No splitting" : splitText.Replace("Split with ", "");
                }

                string volume = BatchVolumeTextBox?.Text ?? "900";
                string level = BatchStartLevelTextBox?.Text ?? "500";

                SummaryTextBlock.Text = $"{formula} � {batchType} � {splitting} � {volume} gal from level {level}";
            }
            catch (Exception ex)
            {
                // Log error but don't crash
                Console.WriteLine($"Error updating summary: {ex.Message}");
            }
        }

        private System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FormulaListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingFromPlc || FormulaListBox == null)
                return;

            if (FormulaListBox.SelectedItem is ListBoxItem selectedItem)
            {
                // Formula numbers are 1-6, ListBox items are 0-5 indexed
                int formulaNumber = FormulaListBox.SelectedIndex + 1;
                Console.WriteLine($"[Lowmidtank] Formula selected: {selectedItem.Content} (value: {formulaNumber})");
                
                // Write to PLC
                WriteToPlc("Formula_number", formulaNumber, _formulaNumberWriter);
                
                UpdateSummary();
            }
        }

        private void BatchVolumeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingFromPlc)
                return;

            UpdateSummary();
            // Note: Batch Volume is not mapped to PLC in the requirements
        }

        private void BatchStartLevelTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingFromPlc || BatchStartLevelTextBox == null)
                return;

            if (int.TryParse(BatchStartLevelTextBox.Text, out int level) && level >= 0 && level <= 2500)
            {
                // Write to PLC
                WriteToPlc("Request_level", level, _requestLevelWriter);
            }

            UpdateSummary();
        }

        private void AllTanksNoBatching_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("All tanks set to No Batching mode.", "Batch Configuration", 
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BatchVolumeUp_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(BatchVolumeTextBox.Text, out int value))
            {
                value = Math.Min(value + 1, MAX_BATCH_VOLUME);
                BatchVolumeTextBox.Text = value.ToString();
                UpdateSummary();
            }
        }

        private void BatchVolumeDown_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(BatchVolumeTextBox.Text, out int value))
            {
                value = Math.Max(value - 1, MIN_BATCH_VOLUME);
                BatchVolumeTextBox.Text = value.ToString();
                UpdateSummary();
            }
        }

        private void BatchStartLevelUp_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(BatchStartLevelTextBox.Text, out int value))
            {
                value = Math.Min(value + 1, MAX_BATCH_START_LEVEL);
                BatchStartLevelTextBox.Text = value.ToString();
                UpdateSummary();
            }
        }

        private void BatchStartLevelDown_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(BatchStartLevelTextBox.Text, out int value))
            {
                value = Math.Max(value - 1, MIN_BATCH_START_LEVEL);
                BatchStartLevelTextBox.Text = value.ToString();
                UpdateSummary();
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (_isUpdatingFromPlc || !this.IsLoaded)
                return;

            if (sender is RadioButton radioButton)
            {
                // Handle Batch Start Type
                if (radioButton.GroupName == "BatchType")
                {
                    var batchTypeButtons = FindVisualChildren<RadioButton>(this)
                        .Where(rb => rb.GroupName == "BatchType")
                        .ToList();

                    int index = batchTypeButtons.IndexOf(radioButton);
                    if (index >= 0 && index <= 2)
                    {
                        // 0=No Batching, 1=Single Batch, 2=Continuous Batching
                        WriteToPlc("Start_Type", index, _startTypeWriter);
                        Console.WriteLine($"[Lowmidtank] Batch Start Type changed to: {radioButton.Content} (value: {index})");
                    }
                }
                // Handle Splitting Type A
                else if (radioButton.GroupName == "SplitA")
                {
                    var splitAButtons = FindVisualChildren<RadioButton>(this)
                        .Where(rb => rb.GroupName == "SplitA")
                        .ToList();

                    int index = splitAButtons.IndexOf(radioButton);
                    if (index >= 0 && index <= 3)
                    {
                        // 0=No Splitting, 1=Tank2, 2=Tank3, 3=Tank4
                        WriteToPlc("Splitting_A", index, _splittingAWriter);
                        Console.WriteLine($"[Lowmidtank] Splitting Type A changed to: {radioButton.Content} (value: {index})");
                    }
                }
                // Handle Splitting Type B
                else if (radioButton.GroupName == "SplitB")
                {
                    var splitBButtons = FindVisualChildren<RadioButton>(this)
                        .Where(rb => rb.GroupName == "SplitB")
                        .ToList();

                    int index = splitBButtons.IndexOf(radioButton);
                    if (index >= 0 && index <= 3)
                    {
                        // 0=No Splitting, 1=Tank2, 2=Tank3, 3=Tank4
                        WriteToPlc("Splitting_B", index, _splittingBWriter);
                        Console.WriteLine($"[Lowmidtank] Splitting Type B changed to: {radioButton.Content} (value: {index})");
                    }
                }
            }

            UpdateSummary();
        }

        private void StartBatch_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (!int.TryParse(BatchVolumeTextBox.Text, out int batchVolume) || 
                batchVolume < MIN_BATCH_VOLUME || batchVolume > MAX_BATCH_VOLUME)
            {
                MessageBox.Show($"Batch Volume must be between {MIN_BATCH_VOLUME} and {MAX_BATCH_VOLUME} gallons.", 
                              "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(BatchStartLevelTextBox.Text, out int batchStartLevel) || 
                batchStartLevel < MIN_BATCH_START_LEVEL || batchStartLevel > MAX_BATCH_START_LEVEL)
            {
                MessageBox.Show($"Batch Start Level must be between {MIN_BATCH_START_LEVEL} and {MAX_BATCH_START_LEVEL} gallons.", 
                              "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get selected formula
            string selectedFormula = "Formula 1";
            if (FormulaListBox.SelectedItem is ListBoxItem selectedItem)
            {
                selectedFormula = selectedItem.Content.ToString();
            }

            // Show confirmation
            var result = MessageBox.Show(
                $"Start batch with the following settings?\n\n" +
                $"Formula: {selectedFormula}\n" +
                $"Batch Volume: {batchVolume} gallons\n" +
                $"Batch Start Level: {batchStartLevel} gallons",
                "Confirm Batch Start",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("Batch started successfully!", "Batch Start", 
                              MessageBoxButton.OK, MessageBoxImage.Information);
                // TODO: Implement actual batch start logic
            }
        }
    }
}
