using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Ring
{
    /// <summary>
    /// Interaction logic for TVCStorageTank2Window.xaml
    /// </summary>
    public partial class TVCStorageTank2Window : Window
    {
        public TVCStorageTank2Window()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // Agitator Timer On Spinners
        private void AgitatorTimerOnUp_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(AgitatorTimerOnTextBox.Text, out int value))
            {
                value = Math.Min(value + 1, 999);
                AgitatorTimerOnTextBox.Text = value.ToString();
            }
        }

        private void AgitatorTimerOnDown_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(AgitatorTimerOnTextBox.Text, out int value))
            {
                value = Math.Max(value - 1, 0);
                AgitatorTimerOnTextBox.Text = value.ToString();
            }
        }

        // Agitator Timer Off Spinners
        private void AgitatorTimerOffUp_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(AgitatorTimerOffTextBox.Text, out int value))
            {
                value = Math.Min(value + 1, 999);
                AgitatorTimerOffTextBox.Text = value.ToString();
            }
        }

        private void AgitatorTimerOffDown_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(AgitatorTimerOffTextBox.Text, out int value))
            {
                value = Math.Max(value - 1, 0);
                AgitatorTimerOffTextBox.Text = value.ToString();
            }
        }

        // Tank Heating Temperature Spinners
        private void TankHeatingTempUp_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TankHeatingTempTextBox.Text, out int value))
            {
                value = Math.Min(value + 1, 200);
                TankHeatingTempTextBox.Text = value.ToString();
            }
        }

        private void TankHeatingTempDown_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TankHeatingTempTextBox.Text, out int value))
            {
                value = Math.Max(value - 1, 0);
                TankHeatingTempTextBox.Text = value.ToString();
            }
        }

        // Tank Cooling Temperature Spinners
        private void TankCoolingTempUp_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TankCoolingTempTextBox.Text, out int value))
            {
                value = Math.Min(value + 1, 200);
                TankCoolingTempTextBox.Text = value.ToString();
            }
        }

        private void TankCoolingTempDown_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TankCoolingTempTextBox.Text, out int value))
            {
                value = Math.Max(value - 1, 0);
                TankCoolingTempTextBox.Text = value.ToString();
            }
        }

        // Main Heating Temperature Spinners
        private void MainHeatingTempUp_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(MainHeatingTempTextBox.Text, out int value))
            {
                value = Math.Min(value + 1, 200);
                MainHeatingTempTextBox.Text = value.ToString();
            }
        }

        private void MainHeatingTempDown_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(MainHeatingTempTextBox.Text, out int value))
            {
                value = Math.Max(value - 1, 0);
                MainHeatingTempTextBox.Text = value.ToString();
            }
        }

        // Main Cooling Temperature Spinners
        private void MainCoolingTempUp_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(MainCoolingTempTextBox.Text, out int value))
            {
                value = Math.Min(value + 1, 200);
                MainCoolingTempTextBox.Text = value.ToString();
            }
        }

        private void MainCoolingTempDown_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(MainCoolingTempTextBox.Text, out int value))
            {
                value = Math.Max(value - 1, 0);
                MainCoolingTempTextBox.Text = value.ToString();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
