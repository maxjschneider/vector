using System;
using System.Collections.Specialized;
using System.Threading;
using System.Windows;

namespace charmap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Hide();

            string[] userdata = null;

            Thread thread = new Thread(() => userdata = GetUser());
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (userdata != null)
            {
                if (!Login(userdata[0], userdata[1])) {
                    this.Show();
                }
            } else
            {
                this.Show();
            }

            Protections.Initialize();
        }

        private string[] GetUser()
        {
            NameValueCollection values = null;

            try
            {
                values = Reg.getUserData();
            }
            catch
            {
                goto failure;
            }

            if (values == null) goto failure;

            string username = values["username"];
            string password = values["password"];

            if (username == null || password == null) goto failure;
            
            try
            {
                return new string[] { username, password };
            } catch 
            {
                return null;
            }
            

        failure:
            return null;
        }

        private bool Login(string username, string password)
        {
            return Validate.Login(username, password);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Validate.Login(this.username.Text, this.password.Text))
            {
                this.Hide();
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Protections.thread.Abort();

            Environment.Exit(0);
        }
    }
}
