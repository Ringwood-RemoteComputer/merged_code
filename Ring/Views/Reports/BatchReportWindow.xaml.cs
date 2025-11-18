using System.Windows;

namespace Ring
{
    public partial class BatchReportWindow : Window
    {
        public BatchReportWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
