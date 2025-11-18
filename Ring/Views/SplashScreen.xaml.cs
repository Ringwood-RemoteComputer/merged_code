using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Ring.Views
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        private DispatcherTimer _loadingTimer;
        private int _currentStep = 0;
        private readonly string[] _loadingMessages = {
            "Initializing Application...",
            "Loading Configuration...",
            "Connecting to Database...",
            "Initializing PLC Services...",
            "Loading User Interface...",
            "Starting Control Systems...",
            "Ready!"
        };

        public SplashScreen()
        {
            InitializeComponent();
            InitializeLoading();
        }

        private void InitializeLoading()
        {
            // Set initial progress
            LoadingProgress.Value = 0;
            
            // Start loading timer
            _loadingTimer = new DispatcherTimer();
            _loadingTimer.Interval = TimeSpan.FromMilliseconds(800); // Update every 800ms
            _loadingTimer.Tick += LoadingTimer_Tick;
            _loadingTimer.Start();

            // Add entrance animation
            this.Opacity = 0;
            this.BeginAnimation(OpacityProperty, CreateFadeInAnimation());
        }

        private void LoadingTimer_Tick(object sender, EventArgs e)
        {
            if (_currentStep < _loadingMessages.Length)
            {
                // Update loading message
                LoadingText.Text = _loadingMessages[_currentStep];
                
                // Update progress bar
                double progress = ((double)_currentStep / (_loadingMessages.Length - 1)) * 100;
                LoadingProgress.Value = progress;
                
                // Add text fade animation
                LoadingText.BeginAnimation(OpacityProperty, CreateTextFadeAnimation());
                
                _currentStep++;
            }
            else
            {
                // Loading complete
                _loadingTimer.Stop();
                CompleteLoading();
            }
        }

        private void CompleteLoading()
        {
            // Fade out animation
            var fadeOutAnimation = CreateFadeOutAnimation();
            fadeOutAnimation.Completed += (s, e) => 
            {
                this.Close();
            };
            
            this.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }

        private DoubleAnimation CreateFadeInAnimation()
        {
            return new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
        }

        private DoubleAnimation CreateFadeOutAnimation()
        {
            return new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };
        }

        private DoubleAnimation CreateTextFadeAnimation()
        {
            return new DoubleAnimation
            {
                From = 0.3,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
        }

        /// <summary>
        /// Show splash screen and wait for completion
        /// </summary>
        public static async Task ShowSplashScreenAsync()
        {
            var splash = new SplashScreen();
            splash.Show();
            
            // Wait for loading to complete
            await Task.Delay(6000); // Total loading time: 6 seconds
            
            // Ensure the splash screen closes
            if (splash.IsLoaded)
            {
                splash.Close();
            }
        }

        /// <summary>
        /// Show splash screen with custom loading time
        /// </summary>
        public static async Task ShowSplashScreenAsync(int loadingTimeMs)
        {
            var splash = new SplashScreen();
            splash.Show();
            
            // Wait for specified loading time
            await Task.Delay(loadingTimeMs);
            
            // Ensure the splash screen closes
            if (splash.IsLoaded)
            {
                splash.Close();
            }
        }
    }
}