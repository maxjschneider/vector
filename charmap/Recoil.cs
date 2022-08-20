using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace charmap
{
    public static class Values
    {
        public static ViewAngles angles = null;

        public static int xcontrol = 100;
        public static int ycontrol = 100;
        public static int random = 0;
        public static int shake = 0;

        public static bool rapid = false;

        public static bool realistic = false;

        public static bool humanization = false;
    }

    public class ViewAngles
    {
        public double[] ak_x;
        public double[] ak_y;
        public double ak_time;

        public double[] lr_x;
        public double[] lr_y;
        public double lr_time;

        public double[] mp5_x;
        public double[] mp5_y;
        public double mp5_time;

        public double[] m2_x;
        public double[] m2_y;
        public double m2_time;

        public double[] thompson_x;
        public double[] thompson_y;
        public double thompson_time;

        public double[] smg_x;
        public double[] smg_y; 
        public double smg_time;

        public double[] sar_x;
        public double[] sar_y;
        public double sar_time;

        public double[] python_x;
        public double[] python_y;
        public double python_time;

        public double[] m39_x;
        public double[] m39_y;
        public double m39_time;

        public double[] p2_x;
        public double[] p2_y;
        public double p2_time;

        public double[] m92_x;
        public double[] m92_y;
        public double m92_time;

        public double[] revolver_x;
        public double[] revolver_y;
        public double revolver_time;
    }

    public class Data
    {
        public void fillArray(List<double> arr, double value, int num)
        {
            for (int i = 0; i < num; i++)
                arr.Add(value);
        }

        public double[] x = null;
        public double[] y = null;

        public List<double> px = new List<double>();
        public List<double> py = new List<double>();
        public List<double> animations = new List<double>();

        public double relay = 0;

        public bool isSemi = false;

        public double scopeMult = 1.0D;
        public double barrelMult = 1.0D;

        public double animationMult = 1.0D;
        public double relayMult = 1.0D;

        public Data(string weapon, string scope, string barrel)
        {
            if (Values.angles == null) throw new Exception();

            switch (weapon)
            {
                case "assault rifle":
                    {
                        isSemi = false;

                        if (Properties.Settings.Default.legacyRecoil)
                        {
                            x = Values.angles.ak_x;
                            y = Values.angles.ak_y;
                        } else
                        {
                            List<double> x_vals = new List<double>();

                            fillArray(x_vals, 0.6, 5);
                            fillArray(x_vals, 1.4, 25);

                            x = x_vals.ToArray();

                            List<double> y_vals = new List<double>();

                            fillArray(y_vals, -2.283, 30);

                            y = y_vals.ToArray();
                        }

                        relay = Values.angles.ak_time;

                        break;
                    }
                case "lr-300":
                    {
                        isSemi = false;

                        if (Properties.Settings.Default.legacyRecoil)
                        {
                            x = Values.angles.lr_x;
                            y = Values.angles.lr_y;
                        }
                        else
                        {
                            List<double> x_vals = new List<double>();

                            fillArray(x_vals, 0, 30);

                            x = x_vals.ToArray();

                            List<double> y_vals = new List<double>();

                            fillArray(y_vals, -1.85, 30);

                            y = y_vals.ToArray();
                        }

                        relay = Values.angles.lr_time;

                        break;
                    }
                case "mp5":
                    {
                        isSemi = false;
                        if (Properties.Settings.Default.legacyRecoil)
                        {
                            x = Values.angles.mp5_x;
                            y = Values.angles.mp5_y;
                        }
                        else
                        {
                            List<double> x_vals = new List<double>();

                            fillArray(x_vals, 0, 30);

                            x = x_vals.ToArray();

                            List<double> y_vals = new List<double>();

                            fillArray(y_vals, -1, 30);

                            y = y_vals.ToArray();
                        }

                        relay = Values.angles.mp5_time;

                        break;
                    }
                case "m249":
                    {
                        isSemi = false;

                        if (Properties.Settings.Default.legacyRecoil)
                        {
                            x = Values.angles.m2_x;
                            y = Values.angles.m2_y;
                        }
                        else
                        {
                            List<double> x_vals = new List<double>();

                            fillArray(x_vals, 0, 100);

                            x = x_vals.ToArray();

                            List<double> y_vals = new List<double>();

                            fillArray(y_vals, -1.45, 100);

                            y = y_vals.ToArray();
                        }

                        relay = Values.angles.m2_time;

                        break;
                    }
                case "thompson":
                    {
                        isSemi = false;

                        if (Properties.Settings.Default.legacyRecoil)
                        {
                            x = Values.angles.thompson_x;
                            y = Values.angles.thompson_y;
                        }
                        else
                        {
                            List<double> x_vals = new List<double>();

                            fillArray(x_vals, 0, 30);

                            x = x_vals.ToArray();

                            List<double> y_vals = new List<double>();

                            fillArray(y_vals, -0.9, 30);

                            y = y_vals.ToArray();
                        }

                        relay = Values.angles.thompson_time;

                        break;
                    }
                case "custom smg":
                    {
                        isSemi = false;

                        if (Properties.Settings.Default.legacyRecoil)
                        {
                            x = Values.angles.smg_x;
                            y = Values.angles.smg_y;
                        }
                        else
                        {
                            List<double> x_vals = new List<double>();

                            fillArray(x_vals, 0, 30);

                            x = x_vals.ToArray();

                            List<double> y_vals = new List<double>();

                            fillArray(y_vals, -0.9, 30);

                            y = y_vals.ToArray();
                        }

                        relay = Values.angles.smg_time;

                        break;
                    }
                case "sar":
                    {
                        isSemi = true;

                        if (Properties.Settings.Default.legacyRecoil)
                        {
                            x = Values.angles.sar_x;
                            y = Values.angles.sar_y;
                        }
                        else
                        {
                            List<double> x_vals = new List<double>();

                            fillArray(x_vals, 0, 1);

                            x = x_vals.ToArray();

                            List<double> y_vals = new List<double>();

                            fillArray(y_vals, -1.4, 1);

                            y = y_vals.ToArray();
                        }

                        relay = Values.angles.sar_time;

                        break;
                    }
                case "python":
                    {
                        isSemi = true;

                        x = Values.angles.python_x;
                        y = Values.angles.python_y;

                        relay = Values.angles.python_time;

                        break;
                    }
                case "m39":
                    {
                        isSemi = true;

                        x = Values.angles.m39_x;
                        y = Values.angles.m39_y;

                        relay = Values.angles.m39_time;

                        break;
                    }
                case "p2":
                    {
                        isSemi = true;

                        x = Values.angles.p2_x;
                        y = Values.angles.p2_y;

                        relay = Values.angles.p2_time;
                        break;
                    }
                case "m92":
                    {
                        isSemi = true;

                        x = Values.angles.m92_x;
                        y = Values.angles.m92_y;

                        relay = Values.angles.m92_time;

                        break;
                    }
                case "revolver":
                    {
                        isSemi = true;

                        x = Values.angles.revolver_x;
                        y = Values.angles.revolver_y;

                        relay = Values.angles.revolver_time;

                        break;
                    }
                default:
                    {
                        x = new double[] { 0 };
                        y = new double[] { 0 };

                        break;
                    }
            }

            switch (scope)
            {
                case "8x scope":
                    {
                        if (Properties.Settings.Default.legacyRecoil)
                            scopeMult = 3.83721D;
                        else
                            scopeMult = 4.7;
                        animationMult = 0.8D;

                        break;
                    }
                case "16x scope":
                    {
                        scopeMult = 7.65116D;
                        animationMult = 0.8D;

                        break;
                    }
                case "holo sight":
                    {
                        scopeMult = 1.18605D;

                        break;
                    }
                case "simple sight":
                    {
                        scopeMult = 0.8D;

                        break;
                    }
                default:
                    {
                        scopeMult = 1.0D;

                        break;
                    }
            }

            switch (barrel)
            {
                case "muzzle boost":
                    {
                        relayMult = 0.9D;

                        break;
                    }
                case "muzzle brake":
                    {
                        barrelMult = 0.5D;

                        break;
                    }
                case "silencer":
                    {
                        if (scope != "8x scope" && scope != "16x scope")
                        {
                            barrelMult = 0.8D;
                        }

                        break;
                    }
            }

            if (x == null || y == null) return;

            double mult = -0.03D * (Properties.Settings.Default.sensitivity * 3.0D) * (Properties.Settings.Default.fov / 100.0D);

            for (int i = 0; i < x.Length; i++)
            {
                double dx, dy;

                if (i == 0 || !Properties.Settings.Default.legacyRecoil)
                {
                    dx = Fix(weapon, scope, x[i]);
                    dy = Fix(weapon, scope, y[i]);
                }
                else
                {
                    dx = Fix(weapon, scope, x[i] - x[i - 1]);
                    dy = Fix(weapon, scope, y[i] - y[i - 1]);
                }

                double animation;

                if (weapon == "m249" || isSemi) animation = relay * 10000;
                else animation = (Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)) / 0.02D) * 10000;

                px.Add(Convert.ToInt32(dx / mult * scopeMult * barrelMult));
                py.Add(Convert.ToInt32(dy / mult * scopeMult * barrelMult));

                animations.Add(animation * animationMult * relayMult);
            }

            relay *= relayMult;
            relay *= 10000;
        }

        private double Fix(string weapon, string scope, double val)
        {
            string current = weapon;

            if (current.Equals("thompson") || current.Equals("custom smg"))
            {
                switch (scope)
                {
                    case "8x scope":
                        {
                            return val;
                        }
                    case "16x scope":
                        {

                            return val;
                        }
                    case "simple sight":
                        {

                            return val * 0.9D;
                        }
                    case "holo sight":
                        {

                            return val;
                        }
                    default:
                        {
                            return val * 0.83D;
                        }
                }
            }
            else if (current.Equals("m249"))
            {
                switch (scope)
                {
                    case "8x scope":
                        {
                            return val;
                        }
                    case "16x scope":
                        {

                            return val;
                        }
                    case "simple sight":
                        {

                            return val * 0.83D;
                        }
                    case "holo sight":
                        {

                            return val * 0.98D;
                        }
                    default:
                        {
                            return val * 1.0D;
                        }
                }
            }
            else if (current.Equals("python"))
            {
                switch (scope)
                {
                    case "8x scope":
                        {
                            return val;
                        }
                    case "16x scope":
                        {

                            return val;
                        }
                    case "simple sight":
                        {

                            return val * 0.83D;
                        }
                    case "holo sight":
                        {

                            return val * 0.98D;
                        }
                    default:
                        {
                            return val * 0.75D;
                        }
                }
            }
            else if (current.Equals("m39"))
            {
                switch (scope)
                {
                    case "8x scope":
                        {
                            return val;
                        }
                    case "16x scope":
                        {

                            return val;
                        }
                    case "simple sight":
                        {

                            return val * 0.83D;
                        }
                    case "holo sight":
                        {

                            return val * 0.98D;
                        }
                    default:
                        {
                            return val * 0.75D;
                        }
                }
            }
            else if (current.Equals("m92"))
            {
                switch (scope)
                {
                    case "8x scope":
                        {
                            return val;
                        }
                    case "16x scope":
                        {

                            return val;
                        }
                    case "simple sight":
                        {

                            return val * 0.83D;
                        }
                    case "holo sight":
                        {

                            return val * 0.98D;
                        }
                    default:
                        {
                            return val * 0.6D;
                        }
                }
            }
            else if (current.Equals("p2"))
            {
                switch (scope)
                {
                    case "8x scope":
                        {
                            return val;
                        }
                    case "16x scope":
                        {

                            return val;
                        }
                    case "simple sight":
                        {

                            return val * 0.83D;
                        }
                    case "holo sight":
                        {

                            return val * 0.98D;
                        }
                    default:
                        {
                            return val * 0.7D;
                        }
                }
            }
            else
            {
                return val;
            }
        }
    }

    public class Recoil
    {
        public Data data = null;

        public string weapon = "none";
        public string scope = "none";
        public string barrel = "none";

        public Recoil(string _weapon, string _scope, string _barrel)
        {
            weapon = _weapon;
            scope = _scope;
            barrel = _barrel;

            data = new Data(_weapon, _scope, _barrel);
        }

        public double Smooth(int i, double excess)
        {
            double x = data.px[i];
            double y = data.py[i];

            double originalX = x;
            double originalY = y;

            if (Values.humanization)
            {
                x = RandomizePixel(x);
                y = RandomizePixel(y);

                x *= (Convert.ToDouble(Values.xcontrol) / 100);
                y *= (Convert.ToDouble(Values.ycontrol) / 100);

                originalX = x;
                originalY = y;

                x += (Convert.ToDouble(Values.shake) / 100.0D) * x;
                y += (Convert.ToDouble(Values.shake) / 100.0D) * y;
            }
            
            Delay.Chrono timer = new Delay.Chrono();
            timer.Start();

            if (data.isSemi && Values.rapid && PInvoke.IsKey(0x01)) PInvoke.SendKeyPress();

            if (Properties.Settings.Default.hipfire)
            {
                double scopeMult = 1D;

                if (!PInvoke.IsKey(0x02))
                {
                    switch (scope)
                    {
                        case "8x scope":
                            {
                                scopeMult = 3.83721D;

                                break;
                            }
                        case "16x scope":
                            {
                                scopeMult = 7.65116D;

                                break;
                            }
                        case "holo sight":
                            {
                                scopeMult = 1.18605D;

                                break;
                            }
                        case "simple sight":
                            {
                                scopeMult = 0.8D;

                                break;
                            }
                    }

                    x = Convert.ToInt32((x / scopeMult) * 0.6D);
                    y = Convert.ToInt32((y / scopeMult) * 0.6D);
                }
            }

            double relay = data.relay * data.relayMult;
            double animation = relay - 10000 * 10;

            if (Properties.Settings.Default.legacyRecoil)
                animation = RandomizeTime(data.animations[i] * data.animationMult * data.relayMult, 5);
            
            if (Values.realistic && Values.humanization) animation = relay - 100000;

            if (!Properties.Settings.Default.legacyRecoil)
            {
                if (PInvoke.IsKey(Properties.Keys.Default.crouch))
                {
                    x = Convert.ToInt32(x * 0.54D);
                    y = Convert.ToInt32(y * 0.5D);
                }
            }

            if ((data.isSemi || weapon == "m249") && Properties.Settings.Default.legacyRecoil)
            {
                if (PInvoke.IsKey(Properties.Keys.Default.crouch))
                {
                    x = Convert.ToInt32(x * 0.5D);
                    y = Convert.ToInt32(y * 0.5D);

                    if (PInvoke.IsKey(0x57) || PInvoke.IsKey(0x41) || PInvoke.IsKey(0x53) || PInvoke.IsKey(0x44))
                    {
                        x = Convert.ToInt32(x * 1.75D);
                        y = Convert.ToInt32(y * 1.75D);
                    }
                }
                else if (PInvoke.IsKey(0x57) || PInvoke.IsKey(0x41) || PInvoke.IsKey(0x53) || PInvoke.IsKey(0x44))
                {
                    x = Convert.ToInt32(x * 2.2D);
                    y = Convert.ToInt32(y * 2.2D);
                }
            }

            double tX = 0, tY = 0;
            int px = 0, py = 0;

            animation -= excess;
            relay -= excess;

            double elapsed = timer.Elapsed();

            while (elapsed < animation)
            {
                double t = elapsed / animation;

                int lerpX = Convert.ToInt32(x * t);
                int lerpY = Convert.ToInt32(y * t);

                PInvoke.Move(lerpX - px, lerpY - py);

                tX += lerpX - px;
                tY += lerpY - py;

                px = lerpX;
                py = lerpY;

                Delay.Wait(10000);

                elapsed = timer.Elapsed();
            }

            if (Values.shake != 0)
            {
                PInvoke.Move(Convert.ToInt32(originalX - tX), Convert.ToInt32(originalY - tY));
            }

            if (relay - elapsed > 0)
            {
                Delay.Wait(relay - timer.Elapsed());

                return timer.Elapsed() - relay;
            } else
            {
                return timer.Elapsed() - animation;
            }
        }

        private double RandomizeTime(double value, int percent)
        {
            Random random = new Random();
            return value - ((Convert.ToDouble(random.Next(0, percent)) / 100D) * value);
        }

        private double RandomizePixel(double value)
        {
            Random random = new Random();

            int factor = Values.random;

            int rand = random.Next(factor * -1, factor);

            return (value * Convert.ToDouble(rand) / 100.0D) + value;
        }
    }

    [SuppressUnmanagedCodeSecurity]
    class Delay
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
        [DllImport("kernel32.dll", SetLastError = false)]
        private static extern bool QueryPerformanceFrequency(out long lpPerformanceFreq);

        public class Chrono
        {
            private static long freq;

            static Chrono()
            {
                if (!QueryPerformanceFrequency(out freq))
                {
                    throw new System.ComponentModel.Win32Exception();
                }
            }

            private long startTime, endTime;

            private bool timerStarted;

            public void Start()
            {
                if (timerStarted)
                {
                    throw new ApplicationException("Can't start, already started");
                }

                timerStarted = true;
                QueryPerformanceCounter(out startTime);
            }

            public double Elapsed()
            {
                QueryPerformanceCounter(out endTime);

                if (!timerStarted)
                {
                    throw new ApplicationException("Can't start, already started");
                }

                return endTime - startTime;
            }
        }

        /*
        public static void Wait(double ticks)
        {
            Chrono timer = new Chrono();
            timer.Start();

            double elapsed = timer.Elapsed();

            while (elapsed < ticks)
            {
                elapsed = timer.Elapsed();
            }
        }
        */
        public static void Wait(double ticks)
        {
            System.Threading.Thread.Sleep(Convert.ToInt32(ticks / 10000));
        }
    }
}
