using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace charmap
{
    public class Binds
    {
        public int ak { get; set; }
        public int lr { get; set; }
        public int mp5 { get; set; }
        public int m2 { get; set; }
        public int thompson { get; set; }
        public int smg { get; set; }
        public int sar { get; set; }
        public int python { get; set; }
        public int m39 { get; set; }
        public int m92 { get; set; }
        public int p2 { get; set; }
        public int revolver { get; set; }

        public int eight { get; set; }
        public int sixteen { get; set; }
        public int holo { get; set; }
        public int simple { get; set; }

        public int boost { get; set; }
        public int brake { get; set; }
        public int silencer { get; set; }

        public int splitSmall { get; set; }
        public int splitLarge { get; set; }
        public int autoCode { get; set; }
        public int autoUpgrade { get; set; }

        public int hide { get; set; }
        public int toggleoverlay { get; set; }
        public int toggle { get; set; }
    }

    public class Sounds
    {
        public static void Beep()
        {
            if (Properties.Settings.Default.sounds)
                Console.Beep(500, 100);
        }
    }


    public unsafe class KeyboardHook
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public uint* dwExtraInfo;
        }

        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;

        private const int WH_KEYBOARD_LL = 13;

        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public Dictionary<int, string> dictionary;

        public KeyboardHook()
        {
            dictionary = new Dictionary<int, string>();
            _proc = HookCallback;
            _hookID = SetHook(_proc);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, KBDLLHOOKSTRUCT* lParam);

        private unsafe IntPtr HookCallback(int nCode, IntPtr wParam, KBDLLHOOKSTRUCT* lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                KeyPressEvent args = new KeyPressEvent();
                int key = (int)lParam->vkCode;

                if (dictionary.ContainsKey(key))
                {
                    args.key = key;
                    args.name = dictionary[key];

                    OnKeyPressed(args);
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        protected void OnKeyPressed(KeyPressEvent e)
        {
            EventHandler<KeyPressEvent> handler = KeyPressed;

            if (handler != null)
            {
                handler(null, e);
            }
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(_hookID);
        }

        public event EventHandler<KeyPressEvent> KeyPressed;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, KBDLLHOOKSTRUCT* lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }

    public class KeyPressEvent : EventArgs
    {
        public int key { get; set; }
        public string name { get; set; }
    }
}
