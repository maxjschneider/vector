using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace charmap
{
    class Helpers
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out Point lpPoint);

        public static void SetSliderAmount(double value, double stack)
        {
            double length = Properties.Coordinates.Default.sliderTopLeft.X - Properties.Coordinates.Default.sliderBottomRight.X;
            double midY = Math.Abs((Properties.Coordinates.Default.sliderTopLeft.Y - Properties.Coordinates.Default.sliderBottomRight.Y) / 2);

            PInvoke.SetCursorPos(Convert.ToInt32(Properties.Coordinates.Default.sliderTopLeft.X - (length * (value / stack))), Convert.ToInt32(Properties.Coordinates.Default.sliderTopLeft.Y + midY));

            PInvoke.LeftClick();
            Thread.Sleep(50);
        }

        private static void ResetLarge(double midY, double slotWidth)
        {
            PInvoke.SetCursorPos(Convert.ToInt32(Properties.Coordinates.Default.largeFurnaceTopLeft.X + (slotWidth / 2)), Convert.ToInt32(Properties.Coordinates.Default.largeFurnaceTopLeft.Y + midY));

            PInvoke.LeftClick();
            Thread.Sleep(20);

            PInvoke.SetCursorPos(Convert.ToInt32(Properties.Coordinates.Default.largeFurnaceTopLeft.X + (slotWidth + slotWidth / 2)), Convert.ToInt32(Properties.Coordinates.Default.largeFurnaceTopLeft.Y + midY));

            PInvoke.LeftClick();
            Thread.Sleep(20);
        }

        public static void SplitLarge()
        {
            double stack = 1000;
            double amount = 63;

            double midY = Math.Abs((Properties.Coordinates.Default.largeFurnaceBottomRight.Y - Properties.Coordinates.Default.largeFurnaceTopLeft.Y) / 6);
            double slotHeight = Math.Abs((Properties.Coordinates.Default.largeFurnaceBottomRight.Y - Properties.Coordinates.Default.largeFurnaceTopLeft.Y) / 3);
            double slotWidth = (Properties.Coordinates.Default.smallFurnaceBottomRight.X - Properties.Coordinates.Default.largeFurnaceTopLeft.X) / 6;

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 6; col++)
                {
                    if (row == 0 && col == 0) col = 2;
                    if (row == 2 && col == 4) return;

                    ResetLarge(midY, slotWidth);

                    SetSliderAmount(amount, stack);

                    SetSliderAmount(1100, 1000);

                    Thread.Sleep(20);
                    PInvoke.LeftDown();
                    Thread.Sleep(20);

                    PInvoke.SetCursorPos(Convert.ToInt32(Properties.Coordinates.Default.largeFurnaceTopLeft.X + ((slotWidth * col) + slotWidth / 2)), Convert.ToInt32(Properties.Coordinates.Default.largeFurnaceTopLeft.Y + midY + (midY * 2 * row)));

                    Thread.Sleep(20);
                    PInvoke.LeftUp();
                    Thread.Sleep(20);

                    stack -= amount;
                }
            }
        }

        public static void SplitSmall()
        {
            double midY = Math.Abs((Properties.Coordinates.Default.smallFurnaceBottomRight.Y - Properties.Coordinates.Default.smallFurnaceTopLeft.Y) / 2);
            double slotWidth = (Properties.Coordinates.Default.smallFurnaceBottomRight.X - Properties.Coordinates.Default.smallFurnaceTopLeft.X) / 6;

            PInvoke.SetCursorPos(Convert.ToInt32(Properties.Coordinates.Default.smallFurnaceTopLeft.X + (slotWidth + slotWidth / 2)), Convert.ToInt32(Properties.Coordinates.Default.smallFurnaceBottomRight.Y - midY));

            PInvoke.LeftDown();
            PInvoke.LeftUp();
            Thread.Sleep(50);

            SetSliderAmount(333, 1000);

            SetSliderAmount(1100, 1000);

            Thread.Sleep(50);
            PInvoke.LeftDown();
            Thread.Sleep(50);

            PInvoke.SetCursorPos(Convert.ToInt32(Properties.Coordinates.Default.smallFurnaceTopLeft.X + ((slotWidth * 2) + slotWidth / 2)), Convert.ToInt32(Properties.Coordinates.Default.smallFurnaceBottomRight.Y - midY));

            Thread.Sleep(50);
            PInvoke.LeftUp();
            Thread.Sleep(50);

            SetSliderAmount(1100, 1000);

            Thread.Sleep(50);
            PInvoke.LeftDown();
            Thread.Sleep(50);

            PInvoke.SetCursorPos(Convert.ToInt32(Properties.Coordinates.Default.smallFurnaceTopLeft.X + ((slotWidth * 3) + slotWidth / 2)), Convert.ToInt32(Properties.Coordinates.Default.smallFurnaceBottomRight.Y - midY));

            Thread.Sleep(50);
            PInvoke.LeftUp();
            Thread.Sleep(50);
        }

        public static void AutoCode(string _code)
        {
            char[] code = _code.ToCharArray();
            if (code.Length != 4) return;

            for (int i = 0; i < code.Length; i++)
            {
                try
                {
                    Convert.ToInt32(code[i]);
                } catch
                {
                    return;
                }
            }

            PInvoke.SendKeyDown(0x45);

            Thread.Sleep(300);

            PInvoke.SetCursorPos(1500, 500);

            Thread.Sleep(10);

            PInvoke.LeftClick();

            Thread.Sleep(10);

            PInvoke.SendKeyUp(0x45);

            for (int i=0; i<code.Length; i++)
            {
                PInvoke.SendKeyDown(Convert.ToByte(48 + Convert.ToInt32(code[i])));
                PInvoke.SendKeyUp(Convert.ToByte(48 + Convert.ToInt32(code[i])));

                Thread.Sleep(20);
            }
        }

        public static void AutoUpgrade(string type)
        {
            Point coords = new Point(1500, 500);

            switch (type)
            {
                case "wood":
                    {
                        coords.X = 1024;
                        coords.Y = 25;

                        break;
                    }
                case "stone":
                    {
                        coords.X = 1500;
                        coords.Y = 500;

                        break;
                    }
                case "metal":
                    {
                        coords.X = 1335;
                        coords.Y = 845;

                        break;
                    }
                case "hqm":
                    {
                        coords.X = 824;
                        coords.Y = 927;

                        break;
                    }
            }

            PInvoke.RightDown();
            Thread.Sleep(30);
            PInvoke.SetCursorPos(coords.X, coords.Y);

            Thread.Sleep(50);
            PInvoke.LeftClick();

            PInvoke.RightUp();
        }
    }
}
