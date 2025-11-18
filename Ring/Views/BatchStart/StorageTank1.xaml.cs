using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ring.Views.BatchStart
{
    /// <summary>
    /// Interaction logic for StorageTank1.xaml
    /// </summary>
    public partial class StorageTank1 : Window, INotifyPropertyChanged
    {
        // Data binding properties
        private double _tankLevel = 65.5;
        private double _tankVolume = 1625;
        private double _temperature = 67.2;
        private double _viscosity = 245;
        private double _pressure = 32.8;
        private string _currentBatchId = "BATCH-2025-001";
        private string _currentFormula = "Formula 1 - Standard Mix";
        private double _batchProgress = 45.5;
        private DateTime _lastUpdateTime = DateTime.Now;
        private List<AlarmInfo> _activeAlarms;

        public event PropertyChangedEventHandler PropertyChanged;

        public StorageTank1()
        {
            InitializeComponent();
            InitializeData();
            DataContext = this;
        }

        private void InitializeData()
        {
            // Initialize sample alarm data
            _activeAlarms = new List<AlarmInfo>
            {
                new AlarmInfo { Timestamp = DateTime.Now.AddMinutes(-5), Description = "Temperature above setpoint", Priority = "Medium", Status = "Active" },
                new AlarmInfo { Timestamp = DateTime.Now.AddMinutes(-12), Description = "Agitator motor overload", Priority = "High", Status = "Acknowledged" },
                new AlarmInfo { Timestamp = DateTime.Now.AddMinutes(-25), Description = "Low tank level warning", Priority = "Low", Status = "Active" }
            };

            // Start timer for real-time updates
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Simulate real-time data updates
            LastUpdateTime = DateTime.Now;
            
            // Simulate tank level changes
            var random = new Random();
            TankLevel = Math.Max(0, Math.Min(100, TankLevel + (random.NextDouble() - 0.5) * 2));
            TankVolume = TankLevel * 25; // 2500 gallons max capacity
            
            // Simulate temperature changes
            Temperature = Math.Max(20, Math.Min(80, Temperature + (random.NextDouble() - 0.5) * 1));
            
            // Simulate viscosity changes
            Viscosity = Math.Max(100, Math.Min(500, Viscosity + (random.NextDouble() - 0.5) * 10));
            
            // Simulate pressure changes
            Pressure = Math.Max(25, Math.Min(45, Pressure + (random.NextDouble() - 0.5) * 0.5));
        }

        #region Properties

        public double TankLevel
        {
            get => _tankLevel;
            set
            {
                _tankLevel = value;
                OnPropertyChanged();
            }
        }

        public double TankVolume
        {
            get => _tankVolume;
            set
            {
                _tankVolume = value;
                OnPropertyChanged();
            }
        }

        public double Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                OnPropertyChanged();
            }
        }

        public double Viscosity
        {
            get => _viscosity;
            set
            {
                _viscosity = value;
                OnPropertyChanged();
            }
        }

        public double Pressure
        {
            get => _pressure;
            set
            {
                _pressure = value;
                OnPropertyChanged();
            }
        }

        public string CurrentBatchId
        {
            get => _currentBatchId;
            set
            {
                _currentBatchId = value;
                OnPropertyChanged();
            }
        }

        public string CurrentFormula
        {
            get => _currentFormula;
            set
            {
                _currentFormula = value;
                OnPropertyChanged();
            }
        }

        public double BatchProgress
        {
            get => _batchProgress;
            set
            {
                _batchProgress = value;
                OnPropertyChanged();
            }
        }

        public DateTime LastUpdateTime
        {
            get => _lastUpdateTime;
            set
            {
                _lastUpdateTime = value;
                OnPropertyChanged();
            }
        }

        public List<AlarmInfo> ActiveAlarms
        {
            get => _activeAlarms;
            set
            {
                _activeAlarms = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Event Handlers

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AgitatorToggle_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button.Content.ToString() == "OFF")
            {
                button.Content = "ON";
                button.Background = Brushes.Green;
                MessageBox.Show("Agitator started successfully!", "Control Action", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                button.Content = "OFF";
                button.Background = Brushes.LightGray;
                MessageBox.Show("Agitator stopped.", "Control Action", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void HeatingToggle_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button.Content.ToString() == "OFF")
            {
                button.Content = "ON";
                button.Background = Brushes.Orange;
                MessageBox.Show("Heating system activated!", "Control Action", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                button.Content = "OFF";
                button.Background = Brushes.LightGray;
                MessageBox.Show("Heating system deactivated.", "Control Action", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void FillValveToggle_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button.Content.ToString() == "CLOSED")
            {
                button.Content = "OPEN";
                button.Background = Brushes.Blue;
                MessageBox.Show("Fill valve opened!", "Control Action", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                button.Content = "CLOSED";
                button.Background = Brushes.LightGray;
                MessageBox.Show("Fill valve closed.", "Control Action", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DischargeValveToggle_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button.Content.ToString() == "CLOSED")
            {
                button.Content = "OPEN";
                button.Background = Brushes.Red;
                MessageBox.Show("Discharge valve opened!", "Control Action", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                button.Content = "CLOSED";
                button.Background = Brushes.LightGray;
                MessageBox.Show("Discharge valve closed.", "Control Action", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EmergencyStop_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to activate EMERGENCY STOP?\nThis will immediately stop all tank operations!", 
                                       "EMERGENCY STOP", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                // Reset all buttons
                AgitatorToggle.Content = "OFF";
                AgitatorToggle.Background = Brushes.LightGray;
                
                HeatingToggle.Content = "OFF";
                HeatingToggle.Background = Brushes.LightGray;
                
                FillValveToggle.Content = "CLOSED";
                FillValveToggle.Background = Brushes.LightGray;
                
                DischargeValveToggle.Content = "CLOSED";
                DischargeValveToggle.Background = Brushes.LightGray;
                
                MessageBox.Show("EMERGENCY STOP ACTIVATED!\nAll tank operations have been stopped.", 
                              "EMERGENCY STOP", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void StartBatch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FormulaComboBox.Text))
            {
                MessageBox.Show("Please select a formula before starting the batch.", "Batch Start", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Start batch with formula: {FormulaComboBox.Text}?\nVolume: {BatchVolumeTextBox.Text} gallons\nStart Level: {StartLevelTextBox.Text} gallons", 
                                       "Start Batch", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                CurrentFormula = FormulaComboBox.Text;
                BatchProgress = 0;
                MessageBox.Show("Batch started successfully!", "Batch Start", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void StopBatch_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to stop the current batch?", "Stop Batch", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                BatchProgress = 0;
                MessageBox.Show("Batch stopped.", "Stop Batch", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ResetAlarms_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Reset all active alarms?", "Reset Alarms", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                ActiveAlarms.Clear();
                OnPropertyChanged(nameof(ActiveAlarms));
                MessageBox.Show("All alarms have been reset.", "Reset Alarms", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void FormulaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormulaComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                CurrentFormula = selectedItem.Content.ToString();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SaveConfiguration_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Configuration saved successfully!", "Save Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadConfiguration_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Configuration loaded successfully!", "Load Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Report exported successfully!", "Export Report", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    /// Data class for alarm information
    /// </summary>
    public class AlarmInfo
    {
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
    }
}
