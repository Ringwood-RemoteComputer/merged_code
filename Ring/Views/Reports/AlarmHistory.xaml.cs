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
    /// Interaction logic for AlarmHistory.xaml
    /// </summary>
    public partial class AlarmHistory : Window
    {
        public AlarmHistory()
        {
            InitializeComponent();
        }
        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Previewing Usage Report...");
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Printing Usage Report...");
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
