using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Navigation;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using VectorAuth;

namespace ManagedStub
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void Main()
        {
            Thread thread = new Thread(Login);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        [Obfuscation(Feature = "virtualization", Exclude = false)]
        internal class auth_instance
        {
            internal static API api = new API(
                "18",
                "EU8oCF1smw5Y7jK7SwPYmNSwaeghrle0zg+RTjxgQCM=",
                "hBd+SKHdBsuGgsxDTWJAvg=="
            );
        }

        [Obfuscation(Feature = "virtualization", Exclude = false)]
        private static void Validate(string username, string password)
        {
            if (auth_instance.api.Login(username, password))
            {
                ExecuteMain();
            }
        }

        [Obfuscation(Feature = "virtualization", Exclude = false)]
        private static void ExecuteMain()
        {
            byte[] raw = auth_instance.api.LoadAssembly("charmap");

            var assembly = Assembly.Load(raw);

            var app = typeof(System.Windows.Application);

            var field = app.GetField("_resourceAssembly", BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, assembly);

            var helper = typeof(BaseUriHelper);
            var property = helper.GetProperty("ResourceAssembly", BindingFlags.NonPublic | BindingFlags.Static);
            property.SetValue(null, assembly, null);

            var handle = GetConsoleWindow();
            ShowWindow(handle, 0);

            var modules = assembly.GetModules();

            assembly.EntryPoint.Invoke(null, null);

        }

        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public static void Login()
        {
            NameValueCollection values;

            try
            {
                values = Reg.getUserData();

                if (values == null)
                {
                    goto failure;
                }
            }
            catch
            {
                goto failure;
            }

            if (values == null) goto failure;

            string username = values["username"];
            string password = values["password"];

            if (username == null || password == null) goto failure;

            Validate(username, password);

        failure:
            Console.WriteLine("Automatic login failed. Please login manually.\nUsername: ");

            string manualuser = Console.ReadLine();

            Console.WriteLine("Password: ");

            string manualpassword = Console.ReadLine();

            Validate(manualuser, manualpassword);
        }
    }
}
