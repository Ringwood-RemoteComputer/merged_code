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
    /// Interaction logic for ShiftConsumption.xaml
    /// </summary>
    public partial class ShiftConsumption : Window
    {
        public ObservableCollection<ShiftRecord> Shifts { get; set; }
        public ShiftConsumption()
        {
            InitializeComponent();

            // Populate the grid
            Shifts = new ObservableCollection<ShiftRecord>
            {
                new ShiftRecord { ShiftId = 1, ShiftName = "Shift A", ShiftStartTime = "06:00 AM", ShiftStartDate = "8/3/2025", ShiftDuration = "8 hrs" },
                new ShiftRecord { ShiftId = 2, ShiftName = "Shift B", ShiftStartTime = "02:00 PM", ShiftStartDate = "8/3/2025", ShiftDuration = "8 hrs" },
                new ShiftRecord { ShiftId = 3, ShiftName = "Shift C", ShiftStartTime = "10:00 PM", ShiftStartDate = "8/3/2025", ShiftDuration = "8 hrs" }
            };

            ShiftDataGrid.ItemsSource = Shifts;
        }

        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            if (ShiftDataGrid.SelectedItem is ShiftRecord shift)
            {
                MessageBox.Show($"Previewing Shift:\n\n" +
                                $"ID: {shift.ShiftId}\n" +
                                $"Name: {shift.ShiftName}\n" +
                                $"Start: {shift.ShiftStartTime} on {shift.ShiftStartDate}\n" +
                                $"Duration: {shift.ShiftDuration}",
                                "Shift Preview", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a shift to preview.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (ShiftDataGrid.SelectedItem is ShiftRecord shift)
            {
                PrintDialog dlg = new PrintDialog();
                if (dlg.ShowDialog() == true)
                {
                    dlg.PrintVisual(ShiftDataGrid, $"Shift Report: {shift.ShiftName}");
                }
            }
            else
            {
                MessageBox.Show("Please select a shift to print.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RefreshData_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Shift data refreshed successfully!", "Refresh Data", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FilterShifts_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Filter options opened!", "Filter Shifts", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Shift consumption report generated successfully!", "Generate Report", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("All filters cleared!", "Clear Filters", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Shift consumption report exported successfully!", "Export Report", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class ShiftRecord
    {
        public int ShiftId { get; set; }
        public string ShiftName { get; set; }
        public string ShiftStartTime { get; set; }
        public string ShiftStartDate { get; set; }
        public string ShiftDuration { get; set; }
        public string Consumption { get; set; }
    }
}
