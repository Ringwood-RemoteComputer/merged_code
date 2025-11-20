using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ring.Views.UserControls
{
    /// <summary>
    /// Interaction logic for NavBar.xaml
    /// </summary>
    public partial class NavBar : UserControl
    {
        public NavBar()
        {
            InitializeComponent();
            
            // Close popup when clicking outside of it
            this.PreviewMouseDown += NavBar_PreviewMouseDown;
            
            // Close popup when window loses focus
            this.Loaded += NavBar_Loaded;
            
            // Close popup when Escape key is pressed
            this.PreviewKeyDown += NavBar_PreviewKeyDown;
        }

        private void NavBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) != null)
            {
                Window.GetWindow(this).Deactivated += Window_Deactivated;
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            // Close all popups when window loses focus
            CloseAllPopups();
        }

        private void NavBar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Only close popups if any are currently open
            if (IsAnyPopupOpen())
            {
                // Check if the click is on the nav bar background (not on buttons or popup content)
                if (IsClickOnNavBarBackground(e))
                {
                    CloseAllPopups();
                }
            }
        }

        private void NavBar_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseAllPopups();
                e.Handled = true;
            }
        }

        /// Handles all button clicks in the NavBar
        private void HandleButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string buttonName = button.Name;
                
                // Handle popup toggle buttons
                if (buttonName == "ProcessButton")
                {
                    TogglePopup(ProcessSubmenuPopup, MainScreenSubmenuPopup, ReportsSubmenuPopup, BatchStartSubmenuPopup, TvcControlSubmenuPopup, UseTanksSubmenuPopup, ShiftsSubmenuPopup, LanguageSubmenuPopup, SetupSubmenuPopup, HelpSubmenuPopup);
                    return;
                }
                else if (buttonName == "MainScreenButton")
                {
                    TogglePopup(MainScreenSubmenuPopup, ProcessSubmenuPopup, ReportsSubmenuPopup, BatchStartSubmenuPopup, TvcControlSubmenuPopup, UseTanksSubmenuPopup, ShiftsSubmenuPopup, LanguageSubmenuPopup, SetupSubmenuPopup, HelpSubmenuPopup);
                    return;
                }
                else if (buttonName == "ReportsButton")
                {
                    TogglePopup(ReportsSubmenuPopup, ProcessSubmenuPopup, MainScreenSubmenuPopup, BatchStartSubmenuPopup, TvcControlSubmenuPopup, UseTanksSubmenuPopup, ShiftsSubmenuPopup, LanguageSubmenuPopup, SetupSubmenuPopup, HelpSubmenuPopup);
                    return;
                }
                else if (buttonName == "BatchStartButton")
                {
                    TogglePopup(BatchStartSubmenuPopup, ProcessSubmenuPopup, MainScreenSubmenuPopup, ReportsSubmenuPopup, TvcControlSubmenuPopup, UseTanksSubmenuPopup, ShiftsSubmenuPopup, LanguageSubmenuPopup, SetupSubmenuPopup, HelpSubmenuPopup);
                    return;
                }
                else if (buttonName == "TvcControlButton")
                {
                    TogglePopup(TvcControlSubmenuPopup, ProcessSubmenuPopup, MainScreenSubmenuPopup, ReportsSubmenuPopup, BatchStartSubmenuPopup, UseTanksSubmenuPopup, ShiftsSubmenuPopup, LanguageSubmenuPopup, SetupSubmenuPopup, HelpSubmenuPopup);
                    return;
                }
                else if (buttonName == "UseTanksButton")
                {
                    TogglePopup(UseTanksSubmenuPopup, ProcessSubmenuPopup, MainScreenSubmenuPopup, ReportsSubmenuPopup, BatchStartSubmenuPopup, TvcControlSubmenuPopup, ShiftsSubmenuPopup, LanguageSubmenuPopup, SetupSubmenuPopup, HelpSubmenuPopup);
                    return;
                }
                else if (buttonName == "ShiftsButton")
                {
                    TogglePopup(ShiftsSubmenuPopup, ProcessSubmenuPopup, MainScreenSubmenuPopup, ReportsSubmenuPopup, BatchStartSubmenuPopup, TvcControlSubmenuPopup, UseTanksSubmenuPopup, LanguageSubmenuPopup, SetupSubmenuPopup, HelpSubmenuPopup);
                    return;
                }
                else if (buttonName == "LanguageButton")
                {
                    TogglePopup(LanguageSubmenuPopup, ProcessSubmenuPopup, MainScreenSubmenuPopup, ReportsSubmenuPopup, BatchStartSubmenuPopup, TvcControlSubmenuPopup, UseTanksSubmenuPopup, ShiftsSubmenuPopup, SetupSubmenuPopup, HelpSubmenuPopup);
                    return;
                }
                else if (buttonName == "SetupButton")
                {
                    TogglePopup(SetupSubmenuPopup, ProcessSubmenuPopup, MainScreenSubmenuPopup, ReportsSubmenuPopup, BatchStartSubmenuPopup, TvcControlSubmenuPopup, UseTanksSubmenuPopup, ShiftsSubmenuPopup, LanguageSubmenuPopup, HelpSubmenuPopup);
                    return;
                }
                else if (buttonName == "HelpButton")
                {
                    TogglePopup(HelpSubmenuPopup, ProcessSubmenuPopup, MainScreenSubmenuPopup, ReportsSubmenuPopup, BatchStartSubmenuPopup, TvcControlSubmenuPopup, UseTanksSubmenuPopup, ShiftsSubmenuPopup, LanguageSubmenuPopup, SetupSubmenuPopup);
                    return;
                }
                else if (buttonName == "DashboardButton")
                {
                    OpenDashboard();
                    return;
                }
                
                // Handle popup submenu buttons (close popup after action)
                if (buttonName == "MakeReadyTankButton")
                {
                    OpenMakeReadyTankWindow();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "StorageTank1Button")
                {
                    OpenStorageTank1();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "StorageTank2Button")
                {
                    OpenStorageTank2();
                    CloseAllPopups();
                    return;
                }
                // Process submenu buttons
                else if (buttonName == "AlarmsButton")
                {
                    OpenAlarms();
                    CloseAllPopups();
                    return;
                }
                // MainScreen submenu buttons
                else if (buttonName == "StorageTankGroupButton")
                {
                    OpenStorageTankGroup();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "UseTankGroupButton")
                {
                    OpenUseTankGroup();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "BoraxCausticTankButton")
                {
                    OpenBoraxCausticTank();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "TvcButton")
                {
                    OpenTvc();
                    CloseAllPopups();
                    return;
                }
                // Reports submenu buttons
                else if (buttonName == "BatchReportButton")
                {
                    OpenBatchReport();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "UsageReportButton")
                {
                    OpenUsageReport();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "BatchHistoryReportButton")
                {
                    OpenBatchHistoryReport();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "BatchUsageReportButton")
                {
                    OpenBatchHistoryUsageReport();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "AlarmHistoryButton")
                {
                    OpenAlarmHistory();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "InventoryReportButton")
                {
                    OpenInventoryReport();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "FormulaReportButton")
                {
                    OpenFormulaReport();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "ShiftConsumptionButton")
                {
                    OpenShiftConsumption();
                    CloseAllPopups();
                    return;
                }
                // TVC Control submenu buttons
                else if (buttonName == "TvcStorageTank1Button")
                {
                    OpenTvcStorageTank1();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "TvcStorageTank2Button")
                {
                    OpenTvcStorageTank2();
                    CloseAllPopups();
                    return;
                }
                // Shifts submenu buttons
                else if (buttonName == "ShiftsControlButton")
                {
                    OpenShiftsControl();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "Shift1Button")
                {
                    OpenShift1();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "Shift2Button")
                {
                    OpenShift2();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "Shift3Button")
                {
                    OpenShift3();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "TwentyFourHourButton")
                {
                    OpenTwentyFourHourShift();
                    CloseAllPopups();
                    return;
                }
                // Help submenu buttons
                else if (buttonName == "ContactUsButton")
                {
                    OpenContactUs();
                    CloseAllPopups();
                    return;
                }
                // Use Tanks submenu buttons
                else if (buttonName == "Mf1Button")
                {
                    OpenMf1();
                    CloseAllPopups();
                    return;
                }
                // Setup submenu buttons
                else if (buttonName == "PasswordButton")
                {
                    OpenPasswordView();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "MakeReadyTankInventoryEditButton")
                {
                    OpenMakeReadyTankInventoryEditScreen();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "MakeReadyTankFormulaEditButton")
                {
                    OpenMakeReadyTankFormulaEditScreen();
                    CloseAllPopups();
                    return;
                }
                else if (buttonName == "AlarmAlertsButton")
                {
                    OpenAlarmAlertsView();
                    CloseAllPopups();
                    return;
                }
                // Other buttons that don't have specific Views yet
                else if (buttonName == "HoldButton" || 
                    buttonName == "DatabaseExportButton" || buttonName == "Mf2Button" ||
                    buttonName == "DoubleBackerButton" ||
                    buttonName == "EnglishUsButton" ||
                    buttonName == "EspanolArgentinaButton" ||
                    buttonName == "MakeReadyTankFormulaExchangeButton" || buttonName == "ContentsButton" ||
                    buttonName == "AboutButton" ||
                    buttonName == "LanguageSetupButton" ||
                    buttonName == "MakeReadyTankNamesEditButton" ||
                    buttonName == "StorageGroupSystemNamesEditButton" ||
                    buttonName == "UseGroupSystemNamesEditButton" ||
                    buttonName == "UseTankSupervisorEditButton" ||
                    buttonName == "ShiftNamesAndSpansButton" ||
                    buttonName == "EditPasswordsButton" ||
                    buttonName == "CommunicationMonitorButton" ||
                    buttonName == "CommunicationDiskLoggingButton" ||
                    buttonName == "UpdateProcessorTimeDateButton")
                {
                    ShowButtonClickMessage(sender);
                    CloseAllPopups();
                    return;
                }
                
                // Handle all other buttons
                ShowButtonClickMessage(sender);
            }
        }

        #region Helper Methods

        /// Loads the Make Ready Tank UserControl into the main content area
        private void OpenMakeReadyTankWindow()
        {
            try
            {
                var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
                if (mainWindow != null)
                {
                    var mainContentArea = mainWindow.FindName("MainContentArea") as ContentControl;
                    if (mainContentArea != null)
                    {
                        mainContentArea.Content = new Ring.Views.MakeReadyTank();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Make Ready Tank: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Dashboard UserControl into the main content area
        private void OpenDashboard()
        {
            try
            {
                var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
                if (mainWindow != null)
                {
                    var mainContentArea = mainWindow.FindName("MainContentArea") as ContentControl;
                    if (mainContentArea != null)
                    {
                        mainContentArea.Content = new Ring.Views.Dashboard.DashboardView();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Dashboard: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Storage Tank 1 UserControl into the main content area
        private void OpenStorageTank1()
        {
            try
            {
                var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
                if (mainWindow != null)
                {
                    var mainContentArea = mainWindow.FindName("MainContentArea") as ContentControl;
                    if (mainContentArea != null)
                    {
                        mainContentArea.Content = new Ring.Views.BatchStart.StorageTank1();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Storage Tank 1: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Storage Tank 2 UserControl into the main content area
        private void OpenStorageTank2()
        {
            try
            {
                var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
                if (mainWindow != null)
                {
                    var mainContentArea = mainWindow.FindName("MainContentArea") as ContentControl;
                    if (mainContentArea != null)
                    {
                        mainContentArea.Content = new Ring.Views.BatchStart.StorageTank2();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Storage Tank 2: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Opens the Alarms Window (separate window, not in content area)
        private void OpenAlarms()
        {
            try
            {
                var alarmWindow = new Ring.AlarmWindow();
                alarmWindow.Show(); // Opens the AlarmWindow (non-modal so it can refresh)
                Console.WriteLine("[NavBar] AlarmWindow opened");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NavBar] Error opening AlarmWindow: {ex.Message}");
                MessageBox.Show($"Error opening Alarms window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Storage Tank Group UserControl into the main content area
        private void OpenStorageTankGroup()
        {
            try
            {
                var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
                if (mainWindow != null)
                {
                    var mainContentArea = mainWindow.FindName("MainContentArea") as ContentControl;
                    if (mainContentArea != null)
                    {
                        mainContentArea.Content = new Ring.Views.MainScreen.StorageTankGroup();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Storage Tank Group: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Use Tank Group UserControl into the main content area
        private void OpenUseTankGroup()
        {
            try
            {
                var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
                if (mainWindow != null)
                {
                    var mainContentArea = mainWindow.FindName("MainContentArea") as ContentControl;
                    if (mainContentArea != null)
                    {
                        mainContentArea.Content = new Ring.Views.MainScreen.UseTankGroup();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Use Tank Group: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Batch Report Window (opens as dialog)
        private void OpenBatchReport()
        {
            try
            {
                var batchReportWindow = new Ring.BatchReportWindow();
                batchReportWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Batch Report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Usage Report Window (opens as dialog)
        private void OpenUsageReport()
        {
            try
            {
                var usageReportWindow = new Ring.UsageReportWindow();
                usageReportWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Usage Report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Batch History Report Window (opens as dialog)
        private void OpenBatchHistoryReport()
        {
            try
            {
                var batchHistoryReport = new Ring.BatchHistoryReport();
                batchHistoryReport.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Batch History Report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Batch History Usage Report Window (opens as dialog)
        private void OpenBatchHistoryUsageReport()
        {
            try
            {
                var batchHistoryUsageReport = new Ring.BatchHistoryUsageReport();
                batchHistoryUsageReport.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Batch History Usage Report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Alarm History Window (opens as dialog)
        private void OpenAlarmHistory()
        {
            try
            {
                var alarmHistoryWindow = new Ring.AlarmHistory();
                alarmHistoryWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Alarm History: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Inventory Report Window (opens as dialog)
        private void OpenInventoryReport()
        {
            try
            {
                var inventoryReport = new Ring.Views.Reports.InventoryReport();
                var window = new Window
                {
                    Title = "Inventory Report",
                    Content = inventoryReport,
                    Width = 1000,
                    Height = 700,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    ResizeMode = ResizeMode.CanResize
                };
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Inventory Report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Formula Report Window (opens as dialog)
        private void OpenFormulaReport()
        {
            try
            {
                var formulaReport = new Ring.Views.Reports.FormulaReport();
                var window = new Window
                {
                    Title = "Formula Report",
                    Content = formulaReport,
                    Width = 1000,
                    Height = 700,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    ResizeMode = ResizeMode.CanResize
                };
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Formula Report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Shift Consumption Window (opens as dialog)
        private void OpenShiftConsumption()
        {
            try
            {
                var shiftConsumption = new Ring.Views.Reports.ShiftConsumption();
                var window = new Window
                {
                    Title = "Shift Consumption",
                    Content = shiftConsumption,
                    Width = 1000,
                    Height = 700,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    ResizeMode = ResizeMode.CanResize
                };
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Shift Consumption: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the TVC Storage Tank 1 Window
        private void OpenTvcStorageTank1()
        {
            try
            {
                var tvcStorageTank1Window = new Ring.TVCStorageTank1Window();
                tvcStorageTank1Window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading TVC Storage Tank 1: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the TVC Storage Tank 2 Window
        private void OpenTvcStorageTank2()
        {
            try
            {
                var tvcStorageTank2Window = new Ring.Views.TVCcontrol.TVCStorageTank2Window();
                tvcStorageTank2Window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading TVC Storage Tank 2: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Shifts Control Window
        private void OpenShiftsControl()
        {
            try
            {
                var shiftControlWindow = new Ring.ShiftControlWindow();
                shiftControlWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Shifts Control: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Shift 1 UserControl into the main content area
        private void OpenShift1()
        {
            try
            {
                var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
                if (mainWindow != null)
                {
                    var mainContentArea = mainWindow.FindName("MainContentArea") as ContentControl;
                    if (mainContentArea != null)
                    {
                        mainContentArea.Content = new Ring.Views.Shifts.Shift1();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Shift 1: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Shift 2 UserControl into the main content area
        private void OpenShift2()
        {
            try
            {
                var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
                if (mainWindow != null)
                {
                    var mainContentArea = mainWindow.FindName("MainContentArea") as ContentControl;
                    if (mainContentArea != null)
                    {
                        mainContentArea.Content = new Ring.Views.Shifts.Shift2();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Shift 2: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the Shift 3 UserControl into the main content area
        private void OpenShift3()
        {
            try
            {
                var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
                if (mainWindow != null)
                {
                    var mainContentArea = mainWindow.FindName("MainContentArea") as ContentControl;
                    if (mainContentArea != null)
                    {
                        mainContentArea.Content = new Ring.Views.Shifts.Shift3();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Shift 3: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenTwentyFourHourShift()
        {
            // TODO: Implement 24 Hour Shift view
            MessageBox.Show("24 Hour Shift button clicked - functionality to be implemented");
        }

        /// Loads the Contact Us UserControl into the main content area
        private void OpenContactUs()
        {
            try
            {
                var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
                if (mainWindow != null)
                {
                    var mainContentArea = mainWindow.FindName("MainContentArea") as ContentControl;
                    if (mainContentArea != null)
                    {
                        mainContentArea.Content = new Ring.Views.Help.ContactUs();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Contact Us: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Loads the MF 1 UserControl into the main content area
        private void OpenMf1()
        {
            try
            {
                var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
                if (mainWindow != null)
                {
                    var mainContentArea = mainWindow.FindName("MainContentArea") as ContentControl;
                    if (mainContentArea != null)
                    {
                        mainContentArea.Content = new Ring.Views.UseTanks.MF1Window();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading MF 1: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Opens the Password Dialog
        private void OpenPasswordView()
        {
            try
            {
                var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
                if (mainWindow != null)
                {
                    var passwordDialog = new Ring.Views.PasswordDialog
                    {
                        Owner = mainWindow
                    };
                    passwordDialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Password View: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// Placeholder for Make Ready Tank Inventory Edit
        private void OpenMakeReadyTankInventoryEditScreen()
        {
            MessageBox.Show("Make Ready Tank Inventory Edit - Feature to be implemented", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// Placeholder for Make Ready Tank Formula Edit
        private void OpenMakeReadyTankFormulaEditScreen()
        {
            MessageBox.Show("Make Ready Tank Formula Edit - Feature to be implemented", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// Placeholder for Alarm Alerts View
        private void OpenAlarmAlertsView()
        {
            MessageBox.Show("Alarm Alerts - Feature to be implemented", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// Placeholder method for Borax/Caustic Tank button
        private void OpenBoraxCausticTank()
        {
            // TODO: Implement Borax/Caustic Tank functionality
            MessageBox.Show("Borax/Caustic Tank button clicked - functionality to be implemented");
        }

        /// Placeholder method for TVC button
        private void OpenTvc()
        {
            // TODO: Implement TVC functionality
            MessageBox.Show("TVC button clicked - functionality to be implemented");
        }

        /// Shows a message box with the button's content text
        private void ShowButtonClickMessage(object sender)
        {
            if (sender is Button button)
            {
                string buttonText = button.Content?.ToString() ?? "Unknown Button";
                MessageBox.Show($"{buttonText} button clicked");
            }
        }

        /// Toggles a popup and closes other popups
        private void TogglePopup(Popup targetPopup, params Popup[] otherPopups)
        {
            // Close all other popups if open
            foreach (var popup in otherPopups)
            {
                popup.IsOpen = false;
            }
            // Toggle the target popup
            targetPopup.IsOpen = !targetPopup.IsOpen;
        }

        /// Closes all popups
        private void CloseAllPopups()
        {
            ProcessSubmenuPopup.IsOpen = false;
            MainScreenSubmenuPopup.IsOpen = false;
            ReportsSubmenuPopup.IsOpen = false;
            BatchStartSubmenuPopup.IsOpen = false;
            TvcControlSubmenuPopup.IsOpen = false;
            UseTanksSubmenuPopup.IsOpen = false;
            ShiftsSubmenuPopup.IsOpen = false;
            LanguageSubmenuPopup.IsOpen = false;
            SetupSubmenuPopup.IsOpen = false;
            HelpSubmenuPopup.IsOpen = false;
        }

        /// Public method to close all popups (called from MainWindow)
        public void CloseAllPopupsPublic()
        {
            CloseAllPopups();
        }

        /// Public method to check if any popup is open
        public bool IsAnyPopupOpen()
        {
            return ProcessSubmenuPopup.IsOpen ||
                   MainScreenSubmenuPopup.IsOpen ||
                   ReportsSubmenuPopup.IsOpen ||
                   BatchStartSubmenuPopup.IsOpen ||
                   TvcControlSubmenuPopup.IsOpen ||
                   UseTanksSubmenuPopup.IsOpen ||
                   ShiftsSubmenuPopup.IsOpen ||
                   LanguageSubmenuPopup.IsOpen ||
                   SetupSubmenuPopup.IsOpen ||
                   HelpSubmenuPopup.IsOpen;
        }

        /// Checks if the click is on the nav bar background (not on buttons or popup content)
        private bool IsClickOnNavBarBackground(MouseButtonEventArgs e)
        {
            // Check if click is on any popup content
            if (ProcessSubmenuPopup.IsMouseOver || MainScreenSubmenuPopup.IsMouseOver ||
                ReportsSubmenuPopup.IsMouseOver || BatchStartSubmenuPopup.IsMouseOver ||
                TvcControlSubmenuPopup.IsMouseOver || UseTanksSubmenuPopup.IsMouseOver ||
                ShiftsSubmenuPopup.IsMouseOver || LanguageSubmenuPopup.IsMouseOver ||
                SetupSubmenuPopup.IsMouseOver || HelpSubmenuPopup.IsMouseOver)
            {
                return false; // Click is on popup content, don't close
            }

            // Check if click is on any navigation button
            if (ProcessButton.IsMouseOver || MainScreenButton.IsMouseOver ||
                ReportsButton.IsMouseOver || BatchStartButton.IsMouseOver ||
                TvcControlButton.IsMouseOver || UseTanksButton.IsMouseOver ||
                ShiftsButton.IsMouseOver || LanguageButton.IsMouseOver ||
                SetupButton.IsMouseOver || HelpButton.IsMouseOver)
            {
                return false; // Click is on a button, don't close (button handler will handle it)
            }

            // Click is on nav bar background, close popups
            return true;
        }


        #endregion
    }
}

