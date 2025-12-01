using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Ring.Views;

namespace Ring
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            try
            {
                // Show splash screen
                var splashScreen = new Ring.Views.SplashScreen();
                splashScreen.Show();
                
                // Use a timer instead of Thread.Sleep to avoid blocking UI
                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(2);
                timer.Tick += (sender, args) => {
                    timer.Stop();
                    
                    // Show main window after splash screen
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    
                    // Set as main window so application shuts down when it closes
                    this.MainWindow = mainWindow;
                    
                    // Close splash screen
                    splashScreen.Close();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during application startup: {ex.Message}", 
                              "Startup Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
                Shutdown(1);
            }
        }
    }
}