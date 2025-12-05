using System.Windows;
using System.Windows.Controls;
using Ring.ViewModels.MainScreen;
using Ring.Views.Dashboard;

namespace Ring.Views.MainScreen
{
    public partial class TVC : UserControl
    {
        public TVC()
        {
            InitializeComponent();
            DataContext = new TVCViewModel();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
            if (mainWindow != null)
            {
                var mainContentArea = mainWindow.FindName("MainContentArea") as ContentControl;
                if (mainContentArea != null)
                {
                    mainContentArea.Content = new DashboardView();
                }
            }
        }
    }
}

