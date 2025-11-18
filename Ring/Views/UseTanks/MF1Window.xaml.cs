using System.Windows;
using System.Windows.Controls;

namespace Ring.Views.UseTanks
{
    /// <summary>
    /// Interaction logic for MF1Window.xaml
    /// </summary>
    public partial class MF1Window : UserControl
    {
        public MF1Window()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
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
    }
}