using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace Loader
{
    public class Protections
    {
        public static Thread thread;

        [DllImport("kernel32.dll")] private static extern unsafe bool VirtualProtect(byte* lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("Kernel32.dll", EntryPoint = "RtlZeroMemory", SetLastError = false)]
        public static extern void ZeroMemory(IntPtr dest, IntPtr size);

        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public static unsafe void Initialize()
        {
            thread = (new Thread(() =>
            {
                while (true)
                {
                    Process[] processes = Process.GetProcesses();

                    foreach (Process process in processes)
                    {
                        string name = process.ProcessName;
                        string windowtitle = process.MainWindowTitle;

                        foreach (string blacklisted in blacklist)
                        {
                            if (Regex.IsMatch(name, blacklisted, RegexOptions.IgnoreCase) || Regex.IsMatch(windowtitle, blacklisted, RegexOptions.IgnoreCase))
                            {
                                TerminateAndBan("A malicious program has been detected running on your computer, your account has been suspended. Please contact Dufresne#8634. ID: " + blacklisted);
                            }
                        }
                    }

                    Thread.Sleep(1);
                }
            }));

            //thread.Start();
        }

        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public static void TerminateAndBan(string message = null)
        {
            if (message != null)
            {
                ProcessStartInfo pi = new ProcessStartInfo();
                pi.Arguments = "/k echo " + message;
                pi.FileName = "cmd.exe";
                pi.WindowStyle = ProcessWindowStyle.Maximized;

                Process.Start(pi);
            }

            Validate.auth_instance.api.BanUser();

            Process.GetCurrentProcess().Kill();
        }

        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public static void CreateList()
        {
            blacklist.Add("dumper");
            blacklist.Add("ExtremeDumper");
            blacklist.Add("ollydbg");
            blacklist.Add("dnspy");
            blacklist.Add("dotpeek");
            blacklist.Add("ilspy");
            blacklist.Add("ida64");
            blacklist.Add("idag");
            blacklist.Add("idag64");
            blacklist.Add("idaw");
            blacklist.Add("idaw64");
            blacklist.Add("idaq");
            blacklist.Add("idaq64");
            blacklist.Add("idau");
            blacklist.Add("idau64");
            blacklist.Add("scylla");
            blacklist.Add("scylla_x64");
            blacklist.Add("scylla_x86");
            blacklist.Add("protection_id");
            blacklist.Add("x64dbg");
            blacklist.Add("x32dbg");
            blacklist.Add("windbg");
            blacklist.Add("reshacker");
            blacklist.Add("ImportREC");
            blacklist.Add("IMMUNITYDEBUGGER");
            blacklist.Add("MegaDumper");
            blacklist.Add("OLLYDBG");
            blacklist.Add("disassembly");
            blacklist.Add("scylla");
            blacklist.Add("Debug");
            blacklist.Add("Immunity");
            blacklist.Add("WinDbg");
            blacklist.Add("x32dbg");
            blacklist.Add("x64dbg");
            blacklist.Add("Import reconstructor");
            blacklist.Add("MegaDumper");
            blacklist.Add("debugger");
            blacklist.Add("debug");
            blacklist.Add("cheat engine");
        }

        private static HashSet<string> blacklist = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    }
}
