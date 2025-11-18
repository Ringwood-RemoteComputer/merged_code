using System.Windows;
using System.Windows.Media;

namespace Ring
{
    public partial class ShiftControlWindow : Window
    {
        public ShiftControlWindow()
        {
            InitializeComponent();
        }

        // Start Shift Events
        private void StartShift1_Click(object sender, RoutedEventArgs e)
        {
            Shift1Status.Fill = Brushes.Green;
        }

        private void StartShift2_Click(object sender, RoutedEventArgs e)
        {
            Shift2Status.Fill = Brushes.Green;
        }

        private void StartShift3_Click(object sender, RoutedEventArgs e)
        {
            Shift3Status.Fill = Brushes.Green;
        }

        // End Shift Events
        private void EndShift1_Click(object sender, RoutedEventArgs e)
        {
            Shift1Status.Fill = Brushes.Red;
        }

        private void EndShift2_Click(object sender, RoutedEventArgs e)
        {
            Shift2Status.Fill = Brushes.Red;
        }

        private void EndShift3_Click(object sender, RoutedEventArgs e)
        {
            Shift3Status.Fill = Brushes.Red;
        }

        // Tank Inclusion Events
        private void IncludeTank_Click(object sender, RoutedEventArgs e)
        {
            Tank1Status.Fill = Brushes.Green;
            Tank2Status.Fill = Brushes.Green;
            Tank3Status.Fill = Brushes.Green;
            Tank4Status.Fill = Brushes.Green;
        }

        private void RemoveTank_Click(object sender, RoutedEventArgs e)
        {
            Tank1Status.Fill = Brushes.Red;
            Tank2Status.Fill = Brushes.Red;
            Tank3Status.Fill = Brushes.Red;
            Tank4Status.Fill = Brushes.Red;
        }

        // Close Window
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
