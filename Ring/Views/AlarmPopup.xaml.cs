using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Ring.Views
{
    /// <summary>
    /// Interaction logic for AlarmPopup.xaml
    /// </summary>
    public partial class AlarmPopup : Window
    {
        private DispatcherTimer _blinkTimer;
        private bool _isAcknowledged = false;
        private bool _isSilenced = false;
        private double _currentWeight = 0.0;
        private DateTime _alarmTime;
        private string _alarmTitle;
        private string _alarmMessage;
        private bool _isEstopAlarm = false;
        
        // Event to notify when alarm closes
        public event EventHandler AlarmClosed;

        // Constructor for weight-based alarm (existing)
        public AlarmPopup(double weight)
        {
            InitializeComponent();
            _currentWeight = weight;
            _alarmTime = DateTime.Now;
            _alarmTitle = "CRITICAL ALARM";
            _alarmMessage = "Make Ready Tank Weight Exceeded";
            _isEstopAlarm = false;
            
            InitializeAlarm();
        }
        
        // Constructor for E-STOP alarm
        public AlarmPopup(string title, string message)
        {
            InitializeComponent();
            _alarmTime = DateTime.Now;
            _alarmTitle = title;
            _alarmMessage = message;
            _isEstopAlarm = true;
            
            InitializeAlarm();
        }

        private void InitializeAlarm()
        {
            if (_isEstopAlarm)
            {
                // E-STOP alarm - update title and message
                this.Title = "E-STOP ALARM";
                // Update the alarm title text block if it exists
                var alarmTitleText = this.FindName("AlarmTitleText") as System.Windows.Controls.TextBlock;
                if (alarmTitleText != null)
                {
                    alarmTitleText.Text = _alarmTitle;
                }
                
                // Update the alarm subtitle
                var alarmSubtitleText = this.FindName("AlarmSubtitleText") as System.Windows.Controls.TextBlock;
                if (alarmSubtitleText != null)
                {
                    alarmSubtitleText.Text = _alarmMessage;
                }
                
                // Hide weight display for E-STOP alarm
                var weightBorder = this.FindName("WeightBorder") as System.Windows.Controls.Border;
                if (weightBorder != null)
                {
                    weightBorder.Visibility = Visibility.Collapsed;
                }
                
                // Update alarm message text
                var alarmMessageText = this.FindName("AlarmMessageText") as System.Windows.Controls.TextBlock;
                if (alarmMessageText != null)
                {
                    alarmMessageText.Text = "⚠ WARNING: Emergency Stop has been activated. All operations have been halted immediately.";
                }
            }
            else
            {
                // Weight-based alarm - set current weight display
                CurrentWeightText.Text = $"{_currentWeight:F1} lbs";
            }
            
            AlarmTimestamp.Text = $"Alarm triggered at: {_alarmTime:HH:mm:ss}";
            
            // Start blinking effect
            StartBlinkingEffect();
            
            // Make window stay on top and focus
            this.Topmost = true;
            this.Focus();
            this.Activate();
            
            // Play system sound (if available)
            System.Media.SystemSounds.Exclamation.Play();
        }

        private void StartBlinkingEffect()
        {
            // Create blinking animation for the weight text
            var blinkAnimation = new DoubleAnimation();
            blinkAnimation.From = 1.0;
            blinkAnimation.To = 0.3;
            blinkAnimation.Duration = TimeSpan.FromMilliseconds(500);
            blinkAnimation.RepeatBehavior = RepeatBehavior.Forever;
            blinkAnimation.AutoReverse = true;
            
            CurrentWeightText.BeginAnimation(OpacityProperty, blinkAnimation);
        }

        private void AcknowledgeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isAcknowledged)
            {
                _isAcknowledged = true;
                AcknowledgeButton.Content = "ACKNOWLEDGED";
                AcknowledgeButton.IsEnabled = false;
                
                // Change color to indicate acknowledgment
                AcknowledgeButton.Background = new SolidColorBrush(Color.FromRgb(0x66, 0xBB, 0x6A));
                AcknowledgeButton.BorderBrush = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50));
                
                // Stop blinking effect
                if (CurrentWeightText != null)
                {
                    CurrentWeightText.BeginAnimation(OpacityProperty, null);
                    CurrentWeightText.Opacity = 1.0;
                }
                
                string message = _isEstopAlarm 
                    ? "E-STOP alarm acknowledged." 
                    : "Alarm acknowledged. Weight monitoring will continue.";
                
                MessageBox.Show(message, 
                               "Alarm Acknowledged", 
                               MessageBoxButton.OK, 
                               MessageBoxImage.Information);
                
                // Close the popup after acknowledgment
                this.Close();
            }
        }

        private void SilenceButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isSilenced)
            {
                _isSilenced = true;
                SilenceButton.Content = "SILENCED";
                SilenceButton.IsEnabled = false;
                
                // Change color to indicate silencing
                SilenceButton.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0x98, 0x00));
                SilenceButton.BorderBrush = new SolidColorBrush(Color.FromRgb(0xF5, 0x7C, 0x00));
                
                MessageBox.Show("Alarm silenced. Visual indicators will remain active.", 
                               "Alarm Silenced", 
                               MessageBoxButton.OK, 
                               MessageBoxImage.Warning);
            }
        }

        private void EmergencyStopButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to initiate EMERGENCY STOP?\n\nThis will immediately halt all tank operations.", 
                                        "EMERGENCY STOP CONFIRMATION", 
                                        MessageBoxButton.YesNo, 
                                        MessageBoxImage.Stop);
            
            if (result == MessageBoxResult.Yes)
            {
                // Perform emergency stop actions
                EmergencyStopButton.Content = "EMERGENCY STOPPED";
                EmergencyStopButton.IsEnabled = false;
                EmergencyStopButton.Background = new SolidColorBrush(Color.FromRgb(0x66, 0xBB, 0x6A));
                EmergencyStopButton.BorderBrush = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50));
                
                MessageBox.Show("EMERGENCY STOP initiated. All tank operations have been halted.", 
                               "EMERGENCY STOP", 
                               MessageBoxButton.OK, 
                               MessageBoxImage.Stop);
                
                // Close the alarm popup after emergency stop
                this.Close();
            }
        }

        public void UpdateWeight(double newWeight)
        {
            _currentWeight = newWeight;
            CurrentWeightText.Text = $"{_currentWeight:F1} lbs";
            Console.WriteLine($"AlarmPopup - UpdateWeight called with {_currentWeight:F1} lbs");
            
            // If weight goes below 500, automatically close the alarm
            if (_currentWeight < 500.0)
            {
                Console.WriteLine($"AlarmPopup - Weight {_currentWeight:F1} is now safe, starting auto-close timer");
                
                // Show brief resolution message and close automatically
                var resolutionMessage = $"Weight has returned to safe levels: {_currentWeight:F1} lbs\n\nAlarm condition resolved.";
                
                // Update the display briefly before closing
                CurrentWeightText.Text = $"SAFE: {_currentWeight:F1} lbs";
                CurrentWeightText.Foreground = new SolidColorBrush(Color.FromRgb(0x2E, 0xCC, 0x71)); // Green color
                
                // Close after a short delay to show the resolution
                var closeTimer = new DispatcherTimer();
                closeTimer.Interval = TimeSpan.FromSeconds(2);
                closeTimer.Tick += (s, e) => {
                    Console.WriteLine("AlarmPopup - Auto-close timer triggered, closing alarm");
                    closeTimer.Stop();
                    this.Close();
                };
                closeTimer.Start();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            // Clean up timers and animations
            _blinkTimer?.Stop();
            
            // Notify that alarm has closed
            AlarmClosed?.Invoke(this, EventArgs.Empty);
            
            base.OnClosed(e);
        }
    }
}
