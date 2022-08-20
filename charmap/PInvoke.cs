using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace charmap
{
    class PInvoke
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(int flags, int x, int y, int data, int extra);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern ushort GetAsyncKeyState(int key);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint MM_BeginPeriod(uint uMilliseconds);

        public static void Move(int dx, int dy)
        {
            mouse_event(0x0001, dx, dy, 0, 0);
        }

        public static bool IsKey(int key)
        {
            return 0 != (GetAsyncKeyState(key) & 0x8000);
        }

        public static void SendKeyPress()
        {
            keybd_event(0xE2, 0x45, 0x0001 | 0, 0);
            keybd_event(0xE2, 0x45, 0x0002 | 0, 0);
        }

        public static void SendKeyDown(byte key)
        {
            keybd_event(key, 0x45, 0x0001 | 0, 0);
        }

        public static void SendKeyUp(byte key)
        {
            keybd_event(key, 0x45, 0x0002 | 0, 0);
        }

        public static void LeftDown()
        {
            Thread.Sleep(1);
            mouse_event(0x0002, 0, 0, 0, 0);
            Thread.Sleep(1);
        }

        public static void LeftUp()
        {
            Thread.Sleep(1);
            mouse_event(0x0004, 0, 0, 0, 0);
            Thread.Sleep(1);
        }

        public static void RightDown()
        {
            Thread.Sleep(1);
            mouse_event(0x0008, 0, 0, 0, 0);
            Thread.Sleep(1);
        }

        public static void RightUp()
        {
            Thread.Sleep(1);
            mouse_event(0x0010, 0, 0, 0, 0);
            Thread.Sleep(1);
        }

        public static void LeftClick()
        {
            LeftDown();
            LeftUp();
        }
    }

    #region cursor info
    public static class CursorExtensions
    {

        [StructLayout(LayoutKind.Sequential)]
        struct PointStruct
        {
            public Int32 x;
            public Int32 y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct CursorInfoStruct
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public PointStruct pt;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(ref CursorInfoStruct pci);

        public static bool IsVisible(this Cursor cursor)
        {
            CursorInfoStruct pci = new CursorInfoStruct();
            pci.cbSize = Marshal.SizeOf(typeof(CursorInfoStruct));
            GetCursorInfo(ref pci);
            const Int32 showing = 0x01;
            bool isVisible = ((pci.flags & showing) != 0);
            return isVisible;
        }

    }
    #endregion cursor info
}
