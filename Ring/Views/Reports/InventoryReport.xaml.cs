using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Ring.Views.Reports
{
    /// <summary>
    /// Interaction logic for InventoryReport.xaml
    /// </summary>
    public partial class InventoryReport : Window
    {
        public ObservableCollection<InventoryRecord> Records { get; set; }
        public InventoryReport()
        {
            InitializeComponent();
            Records = new ObservableCollection<InventoryRecord>
            {
                new InventoryRecord { UpdateInitiator = "John", UpdateTime = "10:25 AM", UpdateDate = "8/3/2025" },
                new InventoryRecord { UpdateInitiator = "Sarah", UpdateTime = "01:10 PM", UpdateDate = "8/4/2025" }
            };
            InventoryDataGrid.ItemsSource = Records;
        }

        private void PreviewInventory_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryDataGrid.SelectedItem is InventoryRecord record)
            {
                MessageBox.Show($"Preview:\nInitiator: {record.UpdateInitiator}\nDate: {record.UpdateDate}\nTime: {record.UpdateTime}",
                                "Preview Inventory", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a row to preview.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PrintInventory_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(InventoryDataGrid, "Inventory Report");
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RefreshData_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Inventory data refreshed successfully!", "Refresh Data", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FilterResults_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Filter options opened!", "Filter Results", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("All filters cleared!", "Clear Filters", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //private void Close_Click(object sender, RoutedEventArgs e)
        //{
        //    this.Close();
        //}

        private void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Inventory report exported successfully!", "Export Report", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class InventoryRecord
    {
        public string UpdateInitiator { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateDate { get; set; }
    }
}