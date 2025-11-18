using System.Windows;

namespace Ring
{
    public partial class TVCStorageTank1Window : Window
    {
        public TVCStorageTank1Window()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}