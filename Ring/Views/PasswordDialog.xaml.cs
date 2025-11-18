using System;
using System.Windows;
using System.Windows.Input;

namespace Ring.Views
{
    public partial class PasswordDialog : Window
    {
        // Password constants
        private const string SUPERVISOR_PASSWORD = "9999";
        
        // Admin password: rw004301 + [current month + current day*2 + 1] as last two digits
        private string GetAdminPassword()
        {
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            int lastTwoDigits = month + (day * 2) + 1;
            return $"rw004301{lastTwoDigits:D2}"; // D2 ensures 2 digits with leading zero if needed
        }

        public bool IsPasswordCorrect { get; private set; }
        public string PasswordType { get; private set; }

        public PasswordDialog()
        {
            InitializeComponent();
            PasswordBox.Focus();
            IsPasswordCorrect = false;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ValidatePassword();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsPasswordCorrect = false;
            this.DialogResult = false;
            this.Close();
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ValidatePassword();
            }
            else if (e.Key == Key.Escape)
            {
                CancelButton_Click(sender, e);
            }
        }

        private void ValidatePassword()
        {
            string enteredPassword = PasswordBox.Password;
            string selectedType = ((System.Windows.Controls.ComboBoxItem)PasswordTypeComboBox.SelectedItem)?.Content?.ToString() ?? "Supervisor";

            string correctPassword = selectedType == "Supervisor" ? SUPERVISOR_PASSWORD : GetAdminPassword();

            if (enteredPassword == correctPassword)
            {
                IsPasswordCorrect = true;
                PasswordType = selectedType;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                ErrorMessageText.Text = "Incorrect password. Please try again.";
                ErrorMessageText.Visibility = Visibility.Visible;
                PasswordBox.Clear();
                PasswordBox.Focus();
            }
        }
    }
}

