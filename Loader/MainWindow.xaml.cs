using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;

namespace Loader
{
    public class auth_instance
    {
        public static Validate validate = new Validate();
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly BackgroundWorker worker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }

        [Obfuscation(Feature = "virtualization", Exclude = false)]
        private void Msg(object sender, DoWorkEventArgs e)
        {
            while (!Status.injected)
            {
                statusMessage.Dispatcher.Invoke(new Action(delegate ()
                {
                    statusMessage.Content = Status.message;
                }));

                Thread.Sleep(1);
            }

            statusMessage.Dispatcher.Invoke(new Action(delegate ()
            {
                statusMessage.Content = Status.message;
            }));
        }

        [Obfuscation(Feature = "virtualization", Exclude = false)]
        private void mainThread(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker msg = new BackgroundWorker();
            msg.DoWork += Msg;

            msg.RunWorkerAsync();

            NameValueCollection values = Reg.getUserData();

            string username = null;
            string password = null;

            if (values != null)
            {
                username = values["username"];
                password = values["password"];
            }
            

            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                Thread.Sleep(500);

                Status.message = "user data not found";

                Thread.Sleep(500);

                Application.Current.Dispatcher.Invoke(new Action(delegate ()
                {
                    this.Hide();

                    Login login = new Login();
                    login.Show();
                }));
            } else
            {
                bool result = auth_instance.validate.login(username, password);

                if (result)
                {
                    Status.message = "authentication successful";

                    if (Validate.auth_instance.api.GrabVariable("version") != "3.0.2")
                    {
                        MessageBox.Show("Please download the most up to date loader at vector.rip", "Update Available", MessageBoxButton.OK, MessageBoxImage.Error);

                        Environment.Exit(0);
                    }

                    Thread.Sleep(250);

                    Status.message = "preparing injection";

                    if (!Directory.Exists("C://Windows/System32/x64"))
                    {
                        Directory.CreateDirectory("C://Windows/System32/x64");
                    
                        byte[] cvextern = (new WebClient()).DownloadData("https://cdn.discordapp.com/attachments/808122904709562388/816484057466011648/cvextern.dll");
                        byte[] opencv = (new WebClient()).DownloadData("https://cdn.discordapp.com/attachments/808122904709562388/816484061131833374/opencv_ffmpeg310_64.dll");

                        File.WriteAllBytes("C://Windows/System32/x64/cvextern.dll", cvextern);
                        File.WriteAllBytes("C://Windows/System32/x64/opencv_ffmpeg310_64.dll", opencv);
                    }

                    if (!File.Exists("C://Windows/System32/Emgu.CV.World.dll"))
                    {
                        byte[] emgu = (new WebClient()).DownloadData("https://cdn.discordapp.com/attachments/808122904709562388/816485513635692554/Emgu.CV.World.dll");

                        File.WriteAllBytes("C://Windows/System32/Emgu.CV.World.dll", emgu);
                    }

                    byte[] native = Validate.auth_instance.api.LoadAssembly("native");

                    Thread.Sleep(250);

                    Status.message = "starting processes";

                    Thread.Sleep(50);

                    foreach (Process process in Process.GetProcessesByName("charmap"))
                    {
                        process.Kill();
                    }

                    Process charmap = new Process();
                    charmap.StartInfo.FileName = "charmap.exe";
                    charmap.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    charmap.Start();

                    Status.message = "injecting...";

                    Thread.Sleep(500);

                    Mapper.Map(native, charmap.Id);

                    Thread.Sleep(5000);

                    Environment.Exit(0);
                } else
                {
                    Status.message = "login failed";

                    Thread.Sleep(500);

                    Application.Current.Dispatcher.Invoke(new Action(delegate ()
                    {
                        this.Hide();

                        Login login = new Login();
                        login.Show();
                    }));
                }
            }
        }

        [Obfuscation(Feature = "virtualization", Exclude = false)]
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            worker.DoWork += mainThread;

            worker.RunWorkerAsync();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
