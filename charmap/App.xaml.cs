using System;
using System.Net;
using System.Reflection;
using System.Windows;

namespace charmap
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow login;

        private void EntryPoint(object sender, StartupEventArgs e)
        {
            Images.images.Add("ak", charmap.Properties.Resources.ak);
            Images.images.Add("lr", charmap.Properties.Resources.lr);
            Images.images.Add("mp5", charmap.Properties.Resources.mp5);
            Images.images.Add("m2", charmap.Properties.Resources.m2);
            Images.images.Add("thompson", charmap.Properties.Resources.thompson);
            Images.images.Add("smg", charmap.Properties.Resources.custom);
            Images.images.Add("sar", charmap.Properties.Resources.sar);
            Images.images.Add("python", charmap.Properties.Resources.python);

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods(BindingFlags.DeclaredOnly |
                                    BindingFlags.NonPublic |
                                    BindingFlags.Public | BindingFlags.Instance |
                                    BindingFlags.Static))
                {
                    try
                    {
                        System.Runtime.CompilerServices.RuntimeHelpers.PrepareMethod(method.MethodHandle);
                    } catch (Exception error) {
                        Console.WriteLine(error.Message);
                    }
                   
                }
            }

            login = new MainWindow();
        }


        private void LogError(object source, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        { 
            try
            {
                WebClient wc = new WebClient();
                string embed = "{\"username\": \"Vector Error Logger\",\"embeds\":[    {\"description\":\"" + e.Exception.Message.ToString() + "\", \"title\":\"Error\", \"color\":13047173}]  }";

                wc.Headers[HttpRequestHeader.ContentType] = "application/json";

                wc.UploadString("https://ptb.discordapp.com/api/webhooks/815611052661669919/ssK6N1DzcTZ11wisIVBjZVG3L15JsOYghqQydu-KxADER6IspsxYyCfpsOasN-3ttmqn", embed);

            } catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
            
        }
    }
}
