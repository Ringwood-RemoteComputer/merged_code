using System.Windows;
using Ring.ViewModels;
using Ring.Services;

namespace Ring.Views
{
    /// <summary>
    /// Interaction logic for BatchQueryView.xaml
    /// </summary>
    public partial class BatchQueryView : Window
    {
        /// <summary>
        /// Initializes a new instance of the BatchQueryView
        /// </summary>
        public BatchQueryView()
        {
            InitializeComponent();
            DataContext = new BatchQueryViewModel();
        }

        /// <summary>
        /// Test print button click handler
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private async void TestPrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var exportService = new ExportService();
                await exportService.TestPrintDialogAsync();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Test print failed: {ex.Message}", "Test Print Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
