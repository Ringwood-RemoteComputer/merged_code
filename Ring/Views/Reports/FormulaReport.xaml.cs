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
    /// Interaction logic for FormulaReport.xaml
    /// </summary>
    public partial class FormulaReport : Window
    {
        public ObservableCollection<FormulaItem> Formulas { get; set; }
        public FormulaReport()
        {
            InitializeComponent();

            Formulas = new ObservableCollection<FormulaItem>
            {
                new FormulaItem { FormulaNumber = 1, FormulaName = "Formula 1", FormulaProcessorName = "1" },
                new FormulaItem { FormulaNumber = 2, FormulaName = "Formula 2", FormulaProcessorName = "2" },
                new FormulaItem { FormulaNumber = 3, FormulaName = "Formula 3", FormulaProcessorName = "3" },
                new FormulaItem { FormulaNumber = 4, FormulaName = "Formula 4", FormulaProcessorName = "4" },
                new FormulaItem { FormulaNumber = 5, FormulaName = "Formula 5", FormulaProcessorName = "5" },
                new FormulaItem { FormulaNumber = 6, FormulaName = "Formula 6", FormulaProcessorName = "6" }
            };

            FormulaDataGrid.ItemsSource = Formulas;
        }

        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            if (FormulaDataGrid.SelectedItem is FormulaItem selected)
            {
                MessageBox.Show($"Previewing:\n\nFormula #{selected.FormulaNumber}\nName: {selected.FormulaName}\nProcessor: {selected.FormulaProcessorName}",
                                "Formula Preview", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a formula to preview.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (FormulaDataGrid.SelectedItem is FormulaItem selected)
            {
                PrintDialog dlg = new PrintDialog();
                if (dlg.ShowDialog() == true)
                {
                    dlg.PrintVisual(FormulaDataGrid, $"Formula Report: {selected.FormulaName}");
                }
            }
            else
            {
                MessageBox.Show("Please select a formula to print.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RefreshFormulas_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Formulas refreshed successfully!", "Refresh Formulas", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddFormula_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add Formula dialog opened!", "Add Formula", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditFormula_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Edit Formula dialog opened!", "Edit Formula", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteFormula_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Formula deleted successfully!", "Delete Formula", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Formula report exported successfully!", "Export Report", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class FormulaItem
    {
        public int FormulaNumber { get; set; }
        public string FormulaName { get; set; }
        public string FormulaProcessorName { get; set; }
        public string Status { get; set; }
        public string LastModified { get; set; }
    }
}
