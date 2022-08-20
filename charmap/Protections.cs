using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace charmap
{
    public class Protections
    {
        public static Thread thread;

        [DllImport("kernel32.dll")] private static extern unsafe bool VirtualProtect(byte* lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("Kernel32.dll", EntryPoint = "RtlZeroMemory", SetLastError = false)]
        static extern void ZeroMemory(IntPtr dest, IntPtr size);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(int m);

       
        public static unsafe void Initialize()
        {
            
            Process current = Process.GetCurrentProcess();

            uint old = 0;
            var modules = current.Modules;

            Int64 base_address = GetModuleHandle(0).ToInt64();

            byte* pBaseAddr = (byte*)(base_address + 0x2000);

            int size = 4096;

            VirtualProtect(pBaseAddr, size, 0x40, out old);

            ZeroMemory((IntPtr)pBaseAddr, (IntPtr)size);
            
            CreateList();

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
                            if (Regex.IsMatch(name, blacklisted, RegexOptions.IgnoreCase))
                            {
                                TerminateAndBan("A malicious program has been detected running on your computer, your account has been suspended. Please contact Dufresne#8634. ID: " + blacklisted);
                            }
                        }
                    }

                    Thread.Sleep(2000);
                }
            }));

            thread.Start();
        }

        public static void TerminateAndBan(string message = null)
        {
            if (message != null)
            {
                ProcessStartInfo pi = new ProcessStartInfo();
                pi.Arguments = "/k echo " + message;
                pi.FileName = "cmd.exe";
                pi.WindowStyle = ProcessWindowStyle.Normal;

                Process.Start(pi);
            }

            auth_instance.api.BanUser();

            Process.GetCurrentProcess().Kill();
        }

        public static void CreateList()
        {
            blacklist.Add("dumper");
            blacklist.Add("ExtremeDumper");
            blacklist.Add("ollydbg");
            blacklist.Add("dnspy");
            blacklist.Add("dotpeek");
            blacklist.Add("ilspy");
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
            blacklist.Add("Immunity");
            blacklist.Add("WinDbg");
            blacklist.Add("x32dbg");
            blacklist.Add("x64dbg");
            blacklist.Add("Import reconstructor");
            blacklist.Add("MegaDumper");
           // blacklist.Add("debugger");
          //  blacklist.Add("debug");
            blacklist.Add("cheat engine");
        }

        private static HashSet<string> blacklist = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    }

}
