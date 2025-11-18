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

namespace Ring
{
    /// <summary>
    /// Interaction logic for UsageReport.xaml
    /// </summary>
    public partial class UsageReportWindow : Window
    {
        public UsageReportWindow()
        {
            InitializeComponent();
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Report generated successfully!", "Generate Report", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            StartTimeComboBox.SelectedIndex = 0;
            EndTimeComboBox.SelectedIndex = 0;
            ReportTypeComboBox.SelectedIndex = 0;
            TankComboBox.SelectedIndex = 0;
            StartDatePicker.SelectedDate = DateTime.Today;
            EndDatePicker.SelectedDate = DateTime.Today;
        }

        private void ExportPDF_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Report exported to PDF!", "Export PDF", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Report exported to Excel!", "Export Excel", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Report sent to printer!", "Print", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveReport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Report saved successfully!", "Save Report", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
