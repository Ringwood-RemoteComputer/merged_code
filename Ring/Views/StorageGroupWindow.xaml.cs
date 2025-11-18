using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ring
{
    public partial class StorageGroupWindow : Window
    {
        private DispatcherTimer _updateTimer;
        private Random _random = new Random();

        public StorageGroupWindow()
        {
            InitializeComponent();
            InitializeTimer();
            UpdateTankValues();
        }

        private void InitializeTimer()
        {
            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromSeconds(2);
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateTankValues();
        }

        private void UpdateTankValues()
        {
            // Simulate tank level changes
            UpdateTankLevel(1, _random.Next(0, 101));
            UpdateTankLevel(2, _random.Next(0, 101));
            UpdateTankLevel(3, _random.Next(0, 101));
            UpdateTankLevel(4, _random.Next(0, 101));
        }

        private void UpdateTankLevel(int tankNumber, double level)
        {
            try
            {
                switch (tankNumber)
                {
                    case 1:
                        if (Tank1Level != null && Tank1LevelText != null)
                        {
                            double height1 = (level / 100.0) * 160.0;
                            Tank1Level.Height = height1;
                            Tank1Level.SetValue(Canvas.TopProperty, 260.0 - height1);
                            Tank1LevelText.Text = $"{level:F0}%";
                        }
                        break;
                    case 2:
                        if (Tank2Level != null && Tank2LevelText != null)
                        {
                            double height2 = (level / 100.0) * 160.0;
                            Tank2Level.Height = height2;
                            Tank2Level.SetValue(Canvas.TopProperty, 260.0 - height2);
                            Tank2LevelText.Text = $"{level:F0}%";
                        }
                        break;
                    case 3:
                        if (Tank3Level != null && Tank3LevelText != null)
                        {
                            double height3 = (level / 100.0) * 160.0;
                            Tank3Level.Height = height3;
                            Tank3Level.SetValue(Canvas.TopProperty, 260.0 - height3);
                            Tank3LevelText.Text = $"{level:F0}%";
                        }
                        break;
                    case 4:
                        if (Tank4Level != null && Tank4LevelText != null)
                        {
                            double height4 = (level / 100.0) * 160.0;
                            Tank4Level.Height = height4;
                            Tank4Level.SetValue(Canvas.TopProperty, 260.0 - height4);
                            Tank4LevelText.Text = $"{level:F0}%";
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating tank level visualization: {ex.Message}");
            }
        }

        private void ShowTank1_Click(object sender, RoutedEventArgs e)
        {
            ShowTankDetails(1, "Storage Tank 1");
        }

        private void ShowTank2_Click(object sender, RoutedEventArgs e)
        {
            ShowTankDetails(2, "Storage Tank 2");
        }

        private void ShowTank3_Click(object sender, RoutedEventArgs e)
        {
            ShowTankDetails(3, "LOW/MID D/B TANK");
        }

        private void ShowTank4_Click(object sender, RoutedEventArgs e)
        {
            ShowTankDetails(4, "Storage Tank 4");
        }

        private void ShowTankDetails(int tankNumber, string tankName)
        {
            try
            {
                if (TankDetailPanel != null && TankDetailTitle != null)
                {
                    TankDetailTitle.Text = tankName;
                    TankDetailPanel.Visibility = Visibility.Visible;
                    
                    // Update tank button states
                    UpdateTankButtonStates();
                    
                    // Simulate tank data
                    if (ActualTemperature != null) ActualTemperature.Text = $"{_random.Next(20, 80):F1}";
                    if (PresetTemperature != null) PresetTemperature.Text = $"{_random.Next(60, 70)}";
                    if (ActualLevel != null) ActualLevel.Text = $"{_random.Next(0, 100):F1}";
                    if (RequestLevel != null) RequestLevel.Text = $"{_random.Next(1000, 1500)}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing tank details: {ex.Message}", "Error", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTankButtonStates()
        {
            try
            {
                // Update button states to show which tank is selected
                if (Tank1Button != null) Tank1Button.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                if (Tank2Button != null) Tank2Button.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                if (Tank3Button != null) Tank3Button.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                if (Tank4Button != null) Tank4Button.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating tank button states: {ex.Message}");
            }
        }

        private void HideTank_Click(object sender, RoutedEventArgs e)
        {
            if (TankDetailPanel != null)
            {
                TankDetailPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void CloseTankDetail_Click(object sender, RoutedEventArgs e)
        {
            if (TankDetailPanel != null)
            {
                TankDetailPanel.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _updateTimer?.Stop();
            base.OnClosed(e);
        }
    }
}




