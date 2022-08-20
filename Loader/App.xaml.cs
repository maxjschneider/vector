using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Loader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Main(object sender, StartupEventArgs e)
        {
            Protections.Initialize();

            if (Process.GetProcessesByName("RustClient").Length != 0)
            {
                FailFast("Please close Rust.");
            } else
            {
                (new MainWindow()).Show();
            }

        }

        public static void FailFast(string message = null)
        {
            if (message != null)
            {
                ProcessStartInfo pi = new ProcessStartInfo();
                pi.Arguments = "/k echo " + message;
                pi.FileName = "cmd.exe";
                pi.WindowStyle = ProcessWindowStyle.Normal;

                Process.Start(pi);
            }

            Process.GetCurrentProcess().Kill();
        }
    }
}
