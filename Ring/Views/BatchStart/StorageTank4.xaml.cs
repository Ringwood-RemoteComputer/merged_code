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

namespace Ring.Views.BatchStart
{
    /// <summary>
    /// Interaction logic for StorageTank4.xaml
    /// </summary>
    public partial class StorageTank4 : Window
    {
        public StorageTank4()
        {
            InitializeComponent();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FormulaListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
