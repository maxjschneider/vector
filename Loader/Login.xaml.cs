using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Loader
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        bool initialized = false;

        public Login()
        {
            InitializeComponent();

            initialized = true;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseOver) this.DragMove();
        }

        private void exitButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }

        private void username_GotFocus(object sender, RoutedEventArgs e)
        {
            if (username.Text == "") username.Text = "username";
            else if (username.Text == "username") username.Text = "";
        }

        private void password_GotFocus(object sender, RoutedEventArgs e)
        {
            if (password.Text == "") password.Text = "password";
            else if (password.Text == "password") password.Text = "";
        }

        [Obfuscation(Feature = "virtualization", Exclude = false)]
        private void submit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((string)submit.Content == "login")
            {
                if (auth_instance.validate.login(username.Text, password.Text)) {
                    Reg.createUserData(username.Text, password.Text);

                    MessageBox.Show("Login successful and user data has been saved.\nPlease restart the loader.", "vector", MessageBoxButton.OK, MessageBoxImage.Information);

                    Environment.Exit(0);
                }
            }
        }
    }
}
