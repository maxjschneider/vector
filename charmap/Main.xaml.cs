using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace charmap
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        #region Variables
        UpdateInfo updateInfo;

        Thread main;
        Thread heartbeat;
        Thread auto;

        Recoil recoil;
        Server server;
        KeyboardHook hook;

        Binds binds;

        private bool toggle = false;
        private bool autodetect = false;
        #endregion

        #region Init
        public Main()
        {
            PInvoke.MM_BeginPeriod(1);

            hook = new KeyboardHook();

            hook.KeyPressed += KeyPressed;

            RegisterHotkeys();

            updateInfo = new UpdateInfo();

            server = new Server(System.Net.IPAddress.Parse("127.0.0.1"), 8000);

            InitializeComponent();

            try
            {
                DateTime date = DateTime.Parse(auth_instance.api.user.expiry);
                TimeSpan remaining = date - DateTime.Now;

                mainTitle.Content = "vector - " + remaining.Days.ToString() + " days left";
            } catch
            {

            }

            //heartbeat = new Thread(HeartBeat);
            //heartbeat.Start();

            main = new Thread(MainThread);
            main.Start();

            auto = new Thread(AutoDetect);
            auto.Start();

            this.xControlSlider.Value = 100;
            this.yControlSlider.Value = 100;
            this.randomControlSlider.Value = 0;
            this.shakeControlSlider.Value = 0;

            this.SensitivityBox.Text = Properties.Settings.Default.sensitivity.ToString();
            this.FovBox.Text = Properties.Settings.Default.fov.ToString();

            this.CursorCheck.IsChecked = Properties.Settings.Default.cursorcheck;
            this.SoundCheck.IsChecked = Properties.Settings.Default.sounds;
            this.ADSHipfireCheck.IsChecked = Properties.Settings.Default.hipfire;
            this.AlwaysHipfireCheck.IsChecked = Properties.Settings.Default.alwayshipfire;
            this.RapidFireCheck.IsChecked = Properties.Settings.Default.rapid;
        }

        private void RegisterHotkeys()
        {
            hook.dictionary = new Dictionary<int, string>();

            string json = Properties.Keys.Default.json;

            try
            {
                if (json == "" || json == null)
                {
                    binds = new Binds();

                    Properties.Keys.Default.json = JsonConvert.SerializeObject(binds);
                    Properties.Keys.Default.Save();
                } else
                {
                    binds = JsonConvert.DeserializeObject<Binds>(json);
                }
            } catch (Exception e)
            {
                MessageBox.Show(e.Message);

                binds = new Binds();

                Properties.Keys.Default.json = JsonConvert.SerializeObject(binds);
                Properties.Keys.Default.Save();
            }

            foreach (PropertyInfo info in typeof(Binds).GetProperties())
            {
                if (Convert.ToInt32(info.GetValue(binds, null)) != 0)
                {
                    hook.dictionary.Add(Convert.ToInt32(info.GetValue(binds, null)), info.Name);
                }
            }
        }

        private void KeyPressed(object sender, KeyPressEvent e)
        {
            Sounds.Beep();

            int weaponIndex = -1;
            int scopeIndex = -1;
            int barrelIndex = -1;

            switch (e.name)
            {
                case "ak":
                    {
                        weaponIndex = 1;

                        break;
                    }
                case "lr":
                    {
                        weaponIndex = 2;

                        break;
                    }
                case "mp5":
                    {
                        weaponIndex = 3;

                        break;
                    }
                case "m2":
                    {
                        weaponIndex = 4;

                        break;
                    }
                case "thompson":
                    {
                        weaponIndex = 5;

                        break;
                    }
                case "smg":
                    {
                        weaponIndex = 6;

                        break;
                    }
                case "sar":
                    {
                        weaponIndex = 7;

                        break;
                    }
                case "python":
                    {
                        weaponIndex = 8;

                        break;
                    }
                case "m39":
                    {
                        weaponIndex = 9;

                        break;
                    }
                case "m92":
                    {
                        weaponIndex = 10;

                        break;
                    }
                case "p2":
                    {
                        weaponIndex = 11;

                        break;
                    }
                case "revolver":
                    {
                        weaponIndex = 12;

                        break;
                    }
                case "eight":
                    {
                        scopeIndex = 1;

                        break;
                    }
                case "sixteen":
                    {
                        scopeIndex = 2;

                        break;
                    }
                case "holo":
                    {
                        scopeIndex = 3;

                        break;
                    }
                case "simple":
                    {
                        scopeIndex = 4;

                        break;
                    }
                case "boost":
                    {
                        barrelIndex = 1;

                        break;
                    }
                case "brake":
                    {
                        barrelIndex = 2;

                        break;
                    }
                case "silencer":
                    {
                        barrelIndex = 3;

                        break;
                    }
                case "splitSmall":
                    {
                        Helpers.SplitSmall();

                        break;
                    }
                case "splitLarge":
                    {
                        Helpers.SplitLarge();

                        break;
                    }
                case "autoCode":
                    {
                        Helpers.AutoCode(autoCodeBox.Text);

                        break;
                    }
                case "autoUpgrade":
                    {
                        Helpers.AutoUpgrade(getUpgrade());

                        break;
                    }
                case "toggle":
                    {
                        ToggleCheck.IsChecked = !ToggleCheck.IsChecked;

                        break;
                    }
                case "toggleoverlay":
                    {
                        updateInfo.focused = !updateInfo.focused;
                        UpdateOverlay();

                        break;
                    }
            }

            if (weaponIndex != -1)
            {
                if (weaponIndex == WeaponCombo.SelectedIndex)
                {
                    WeaponCombo.SelectedIndex = 0;
                } else
                {
                    WeaponCombo.SelectedIndex = weaponIndex;
                }
            }

            if (scopeIndex != -1)
            {
                if (scopeIndex == ScopeCombo.SelectedIndex)
                {
                    ScopeCombo.SelectedIndex = 0;
                }
                else
                {
                    ScopeCombo.SelectedIndex = scopeIndex;
                }
            }

            if (barrelIndex != -1)
            {
                if (barrelIndex == BarrelCombo.SelectedIndex)
                {
                    BarrelCombo.SelectedIndex = 0;
                }
                else
                {
                    BarrelCombo.SelectedIndex = barrelIndex;
                }
            }
        }

        #endregion

        #region Threads
        private void HeartBeat()
        {
            while (true)
            {
                if (!auth_instance.api.Login(auth_instance.api.user.username, auth_instance.api.user.password))
                {
                    Process.GetCurrentProcess().Kill();
                }

                Thread.Sleep(5000);
            }
        }

        private void AutoDetect()
        {
            while (true)
            {
                if (autodetect)
                {
                    int width = Properties.Coordinates.Default.slotBottomRight.X - Properties.Coordinates.Default.slotTopLeft.X;
                    int height = Properties.Coordinates.Default.slotBottomRight.Y - Properties.Coordinates.Default.slotTopLeft.Y;

                    Bitmap source = ImageProc.CaptureScreen(Properties.Coordinates.Default.slotTopLeft, new System.Drawing.Size(width, height));

                    foreach (string key in Images.images.Keys)
                    {
                        Bitmap template = Images.images[key];

                        if (ImageProc.IsMatch(source, template))
                        {
                            int index = -1;

                            switch (key)
                            {
                                case "ak":
                                    {
                                        index = 1;

                                        break;
                                    }
                                case "lr":
                                    {
                                        index = 2;

                                        break;
                                    }
                                case "mp5":
                                    {
                                        index = 3;

                                        break;
                                    }
                                case "m2":
                                    {
                                        index = 4;

                                        break;
                                    }
                                case "thompson":
                                    {
                                        index = 5;

                                        break;
                                    }
                                case "smg":
                                    {
                                        index = 6;

                                        break;
                                    }
                                case "sar":
                                    {
                                        index = 7;

                                        break;
                                    }
                                case "python":
                                    {
                                        index = 8;

                                        break;
                                    }
                            }

                            if (index != -1)
                            {
                                this.Dispatcher.Invoke(new Action(() =>
                                {
                                    this.WeaponCombo.SelectedIndex = index;
                                }));
                            }
                        }
                    }

                    Thread.Sleep(100);
                } else
                {
                    Thread.Sleep(1000);
                }
            }
            
        }

        private void MainThread()
        {
            while (true)
            {
                if (PInvoke.IsKey(0x2D))
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        updateInfo.focused = !updateInfo.focused;
                        UpdateOverlay();
                    }));

                    while (PInvoke.IsKey(0x2D)) { }
                }

                bool m1 = PInvoke.IsKey(0x01) && (!System.Windows.Forms.Cursor.Current.IsVisible() || !Properties.Settings.Default.cursorcheck);
                bool m2 = PInvoke.IsKey(0x02) || (Properties.Settings.Default.hipfire && Properties.Settings.Default.alwayshipfire);

                if (m1 && m2 && toggle)
                {
                    double excess = 10000;

                    for (int i = 0; i < recoil.data.x.Length; i++)
                    {
                    smooth:
                        if (i >= recoil.data.x.Length) break;

                        excess = recoil.Smooth(i, excess);

                        if (!PInvoke.IsKey(0x01) || (!PInvoke.IsKey(0x02) && !Properties.Settings.Default.hipfire)) goto exit;
                        else { i++; goto smooth; }
                    exit:
                        Stopwatch timer = new Stopwatch();
                        timer.Start();

                        double elapsed = timer.ElapsedMilliseconds;

                        while (elapsed < recoil.data.relay / 10000)
                        {
                            if (PInvoke.IsKey(0x01) && (PInvoke.IsKey(0x02) || (Properties.Settings.Default.hipfire))) { i++; goto smooth; };

                            elapsed = timer.ElapsedMilliseconds;
                        }

                        break;
                    }

                    if (recoil.data.isSemi && Values.rapid && PInvoke.IsKey(0x01)) { Delay.Wait(20 * 10000); PInvoke.SendKeyPress(); continue; }

                    while (PInvoke.IsKey(0x01)) Thread.Sleep(1);
                }

                Thread.Sleep(1);
            }
        }

        #endregion

        #region Window Functunality

        private void UpdateOverlay()
        {
            if (MenuCheck != null && WaterMarkCheck != null && crosshairCombo != null)
            {
                updateInfo.menu = (bool)MenuCheck.IsChecked;
                updateInfo.watermark = (bool)WaterMarkCheck.IsChecked;

                if (crosshairCombo.SelectedIndex != 0)
                {
                    updateInfo.crosshair = true;
                }
            }

            server.SendMessage(JsonConvert.SerializeObject(updateInfo));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            main.Abort();
            auto.Abort();
            heartbeat.Abort();

            hook.Dispose();

            Process.GetCurrentProcess().Kill();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        #endregion

        #region Misc Functions
        private void Update()
        {
            if (WeaponCombo != null && ScopeCombo != null && BarrelCombo != null)
            {
                if (WeaponCombo.SelectedIndex != -1 && ScopeCombo.SelectedIndex != -1 && BarrelCombo.SelectedIndex != -1)
                {
                    string weapon = ((ComboBoxItem)WeaponCombo.SelectedItem).Content.ToString();
                    string scope = ((ComboBoxItem)ScopeCombo.SelectedItem).Content.ToString();
                    string barrel = ((ComboBoxItem)BarrelCombo.SelectedItem).Content.ToString();

                    updateInfo.weapon = weapon;
                    updateInfo.scope = scope;
                    updateInfo.barrel = barrel;

                    recoil = new Recoil(weapon, scope, barrel);

                    UpdateOverlay();
                }
            }
        }

        private void ApplyConfig(Config config)
        {
            if (config.saveguns)
            {
                WeaponCombo.SelectedIndex = config.weapon;
                ScopeCombo.SelectedIndex = config.scope;
                BarrelCombo.SelectedIndex = config.barrel;
            }
            
            if (config.saverandom)
            {
                xControlSlider.Value = config.xcontrol;
                yControlSlider.Value = config.ycontrol;
                randomControlSlider.Value = config.random;

                realisticCheck.IsChecked = config.realistic;

                shakeControlSlider.Value = config.shake;
                HumanizationCheck.IsChecked = config.humanization;
            }

            Update();
        }
        #endregion

        #region Main Tab
        private void WeaponCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void BarrelCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void ScopeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            toggle = (bool)ToggleCheck.IsChecked;
        }

        private void HumanizationCheck_Checked(object sender, RoutedEventArgs e)
        {
            Values.humanization = (bool)HumanizationCheck.IsChecked;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig save = new SaveConfig();
            save.ShowDialog();

            Config config = new Config();

            config.name = save.name;
            config.weapon = WeaponCombo.SelectedIndex;
            config.scope = ScopeCombo.SelectedIndex;
            config.barrel = BarrelCombo.SelectedIndex;
            config.xcontrol = xControlSlider.Value;
            config.ycontrol = yControlSlider.Value;
            config.random = randomControlSlider.Value;
            config.shake = shakeControlSlider.Value;
            config.realistic = (bool)realisticCheck.IsChecked;
            config.humanization = (bool)HumanizationCheck.IsChecked;
            config.saveguns = save.guncheck;
            config.saverandom = save.randomcheck;
            config.keybind = 0;

            ConfigManager manager = new ConfigManager(config);
            manager.Save();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadConfig loadConfig = new LoadConfig();

            try
            {
                loadConfig.ShowDialog();
            } catch
            {
                return;
            }

            if (loadConfig.ret == null) return;

            Config config = loadConfig.ret;

            ApplyConfig(config);
        }
        #endregion

        #region Settings Tab

        #region General
        private void SoundCheck_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.sounds = (bool)SoundCheck.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void ProcessPriorityCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ProcessPriorityCombo.SelectedIndex)
            {
                case 0:
                    {
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

                        break;
                    }
                case 1:
                    {
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;

                        break;
                    }
                case 2:
                    {
                        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;

                        break;
                    }
            }
        }

        private void TaskbarCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            this.ShowInTaskbar = (bool)TaskbarCheck.IsChecked;
        }

        private void CursorCheck_Checked_1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.cursorcheck = (bool)CursorCheck.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void SensitivityBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double sens = Double.Parse(SensitivityBox.Text);

                if (sens < 0.01 || sens > 2) return;

                Properties.Settings.Default.sensitivity = sens;
                Properties.Settings.Default.Save();

                Update();
            }
            catch
            {
            }
        }

        private void FovBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int fov = Int32.Parse(FovBox.Text);

                if (fov < 50 || fov > 90) return;

                Properties.Settings.Default.fov = fov;
                Properties.Settings.Default.Save();

                Update();
            }
            catch
            {

            }
        }
        #endregion

        #region Recoil
        private void LegacyRecoilCheck_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.legacyRecoil = true;
            Properties.Settings.Default.Save();
        }

        private void LegacyRecoilCheck_Unchecked(object sender, RoutedEventArgs e)
        {

            Properties.Settings.Default.legacyRecoil = false;
            Properties.Settings.Default.Save();
        }

        private void ADSHipfireCheck_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.hipfire = (bool)ADSHipfireCheck.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void AlwaysHipfireCheck_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.alwayshipfire = (bool)AlwaysHipfireCheck.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void RapidFireCheck_Checked(object sender, RoutedEventArgs e)
        {
            RapidFireCheck.IsChecked = steam.EnableRapid();

            Values.rapid = (bool)RapidFireCheck.IsChecked;
        }

        private void RapidFireCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            Values.rapid = false;
        }
        #endregion

        #region Realism
        private void xControlSlider_SliderMoved(object sender, EventArgs e)
        {
            Values.xcontrol = xControlSlider.Value;
        }

        private void yControlSlider_SliderMoved(object sender, EventArgs e)
        {
            Values.ycontrol = yControlSlider.Value;
        }

        private void randomControlSlider_SliderMoved(object sender, EventArgs e)
        {
            Values.random = randomControlSlider.Value;
        }

        private void shakeControlSlider_SliderMoved(object sender, EventArgs e)
        {
            Values.shake = shakeControlSlider.Value;
        }

        private void realisticCheck_Checked(object sender, RoutedEventArgs e)
        {
            Values.realistic = (bool)realisticCheck.IsChecked;
        }
        #endregion

        #region Overlay
        private void MenuCheck_Checked(object sender, RoutedEventArgs e)
        {
            UpdateOverlay();
        }

        private void crosshairColorCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (crosshairColorCombo.SelectedIndex)
            {
                case 0:
                    {
                        updateInfo.crosshaircolor = "rgb(0, 255, 0)";

                        break;
                    }
                case 1:
                    {
                        updateInfo.crosshaircolor = "rgb(199,21,133)";

                        break;
                    }
                case 2:
                    {
                        updateInfo.crosshaircolor = "rgb(255, 0, 0)";

                        break;
                    }
                case 3:
                    {
                        updateInfo.crosshaircolor = "rgb(255,255,255)";

                        break;
                    }
            }

            UpdateOverlay();
        }

        private void crosshairCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (crosshairCombo.SelectedIndex)
            {
                case 0:
                    {
                        updateInfo.crosshairtype = "none";

                        break;
                    }
                case 1:
                    {
                        updateInfo.crosshairtype = "normal";

                        break;
                    }
                case 2:
                    {
                        updateInfo.crosshairtype = "dot";

                        break;
                    }
            }

            UpdateOverlay();
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to enable the Vector overlay?\n\n" +
                "This will disable the normal discord overlay.\n\n" +
                "You can revert back to the normal discord overlay\nat any time by clicking the \"uninstall\" button.", "Warning", System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                string discord = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Discord\";

                string[] currentversion = Directory.GetDirectories(discord, "app-1.*", SearchOption.TopDirectoryOnly);

                string[] files = Directory.GetFiles(currentversion[0], "host.js", SearchOption.AllDirectories);

                string jshost = files[0];

                if (File.Exists(jshost))
                {
                    using (StreamReader reader = new StreamReader(jshost))
                    {
                        string line;
                        int num = 0;

                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Contains("overlayURL: "))
                            {
                                reader.Close();

                                string[] lines = File.ReadAllLines(jshost);
                                lines[num] = "overlayURL: \"https://vector.rip/api/overlay/vector.html\" + \"?pid=\".concat(pid.toString()),";
                                File.WriteAllLines(jshost, lines);

                                return;
                            }

                            num++;
                        }
                    }
                } else
                {
                    System.Windows.MessageBox.Show("Could not find discord host file! \n\n" +
                        "Ensure you have the current version of Discord installed on your main drive.\n\n" +
                        "Also, ensure that discord overlay is enabled and that you have used it in game at least once.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }


            } catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to disable the Vector overlay?\n\n" +
                "This will revert the discord overlay back to normal.", "Warning", System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;
            
            try
            {
                string discord = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Discord\";

                string[] currentversion = Directory.GetDirectories(discord, "app-1.*", SearchOption.TopDirectoryOnly);

                string[] files = Directory.GetFiles(currentversion[0], "host.js", SearchOption.AllDirectories);

                string jshost = files[0];

                if (File.Exists(jshost))
                {
                    using (StreamReader reader = new StreamReader(jshost))
                    {
                        string line;
                        int num = 0;

                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Contains("overlayURL: "))
                            {
                                reader.Close();

                                string[] lines = File.ReadAllLines(jshost);
                                lines[num] = "    overlayURL: url,";
                                File.WriteAllLines(jshost, lines);

                                return;
                            }

                            num++;
                        }
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Could not find discord host file! \n\n" +
                        "Ensure you have the current version of Discord installed on your main drive.\n\n" +
                        "Also, ensure that discord overlay is enabled and that you have used it in game at least once.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }


            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }


        #endregion

        #endregion

        #region Helpers Tab
        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);


        private void AutoDetectCheck_Checked(object sender, RoutedEventArgs e)
        {
            autodetect = (bool)AutoDetectCheck.IsChecked;
        }

        private void CalibrateAutoDetection_Click(object sender, RoutedEventArgs e)
        {
            Selector slot = new Selector();

            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("Before clicking YES, make sure you have Rust open!\n\nIf you are ready, press YES.\nIf you need help, press NO.",
                "Calibration",
                System.Windows.MessageBoxButton.YesNoCancel,
                System.Windows.MessageBoxImage.Information);

            if (result == MessageBoxResult.No)
            {
                Process.Start("https://google.com");

                return;
            }
            else if (result != MessageBoxResult.Yes)
            {
                return;
            }

            IntPtr hWnd = IntPtr.Zero;

            try
            {
                hWnd = Process.GetProcessesByName("RustClient")[0].MainWindowHandle;
            }
            catch
            {
                System.Windows.MessageBox.Show("Rust window could not be found!", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                return;
            }

            if (hWnd == IntPtr.Zero)
            {
                System.Windows.MessageBox.Show("Rust window could not be found!", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                return;
            }

            SetForegroundWindow(hWnd);
            slot.ShowDialog();

            SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);

            Properties.Coordinates.Default.slotTopLeft = new System.Drawing.Point((int)slot.topLeft.X, (int)slot.topLeft.Y);
            Properties.Coordinates.Default.slotBottomRight = new System.Drawing.Point((int)slot.bottomRight.X, (int)slot.bottomRight.Y); 
            
            System.Windows.MessageBox.Show("Calibration complete!",
                 "Calibration",
                 System.Windows.MessageBoxButton.OK,
                 System.Windows.MessageBoxImage.Information);

            Properties.Coordinates.Default.Save();
        }

        private void CalibrateSplitters_Click(object sender, RoutedEventArgs e)
        {
            Selector slider = new Selector();
            Selector small = new Selector();
            Selector large = new Selector();

            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("Before clicking YES, make sure you have Rust open, and you have a furnace opened so that the ore slider is visible.\n\nIf you are ready, press YES.\nIf you need help, press NO.",
                "Calibration",
                System.Windows.MessageBoxButton.YesNoCancel,
                System.Windows.MessageBoxImage.Information);

            if (result == MessageBoxResult.No)
            {
                Process.Start("https://google.com");

                return;
            } else if (result != MessageBoxResult.Yes)
            {
                return;
            }

            IntPtr hWnd = IntPtr.Zero;

            try
            {
                hWnd = Process.GetProcessesByName("RustClient")[0].MainWindowHandle;
            } catch
            {
                System.Windows.MessageBox.Show("Rust window could not be found!", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                return;
            }

            if (hWnd == IntPtr.Zero)
            {
                System.Windows.MessageBox.Show("Rust window could not be found!", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                return;
            }

            SetForegroundWindow(hWnd);

            slider.ShowDialog();

            SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);

            Properties.Coordinates.Default.sliderTopLeft = new System.Drawing.Point((int)slider.topLeft.X, (int)slider.topLeft.Y);
            Properties.Coordinates.Default.sliderBottomRight = new System.Drawing.Point((int)slider.bottomRight.X, (int)slider.bottomRight.Y);

            System.Windows.MessageBox.Show("Before clicking OK, make sure you have Rust open, and you have a SMALL furnace opened.",
                "Calibration",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);

            SetForegroundWindow(hWnd);

            small.ShowDialog();

            SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);

            Properties.Coordinates.Default.smallFurnaceTopLeft = new System.Drawing.Point((int)small.topLeft.X, (int)small.topLeft.Y);
            Properties.Coordinates.Default.smallFurnaceBottomRight = new System.Drawing.Point((int)small.bottomRight.X, (int)small.bottomRight.Y);

            System.Windows.MessageBox.Show("Before clicking OK, make sure you have Rust open, and you have a LARGE furnace opened.",
                "Calibration",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);

            SetForegroundWindow(hWnd);

            large.ShowDialog();

            SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);

            Properties.Coordinates.Default.largeFurnaceTopLeft = new System.Drawing.Point((int)large.topLeft.X, (int)large.topLeft.Y);
            Properties.Coordinates.Default.largeFurnaceBottomRight = new System.Drawing.Point((int)large.bottomRight.X, (int)large.bottomRight.Y);

            System.Windows.MessageBox.Show("Calibration complete!",
                "Calibration",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);

            Properties.Coordinates.Default.Save();
        }

        private string getUpgrade()
        {
            if ((bool)woodRadio.IsChecked)
            {
                return "wood";
            }
            else if ((bool)stoneRadio.IsChecked)
            {
                return "stone";
            }
            else if ((bool)metalRadio.IsChecked)
            {

                return "metal";
            }
            else if ((bool)hqmRadio.IsChecked)
            {

                return "hqm";
            } else
            {
                return "wood";
            }
        }

        private void woodRadio_Checked(object sender, RoutedEventArgs e)
        {
            
        }
        #endregion

        #region Keybind Tab
        private void WeaponsKeybindBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            WeaponsKeybindBox.Text = "";
        }

        private void SaveBinds()
        {
            string json = JsonConvert.SerializeObject(binds);

            Properties.Keys.Default.json = json;
            Properties.Keys.Default.Save();
        }

        private bool CheckKey(Key k)
        {
            int key = KeyInterop.VirtualKeyFromKey(k);

            if (hook.dictionary.ContainsKey(key))
            {
                MessageBox.Show(k.ToString() + " is already bound to " + hook.dictionary[key]); ;

                return false;
            }
            else return true;
        }

        private void WeaponsKeybindBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!CheckKey(e.Key)) return;

            int key = KeyInterop.VirtualKeyFromKey(e.Key);

            if (binds == null) return;

            switch (WeaponsKeybindCombo.Text)
            {
                case "assault rifle":
                    {
                        binds.ak = key;

                        break;
                    }
                case "lr-300":
                    {
                        binds.lr = key;

                        break;
                    }
                case "mp5":
                    {
                        binds.mp5 = key;

                        break;
                    }
                case "m249":
                    {
                        binds.m2 = key;

                        break;
                    }
                case "thompson":
                    {
                        binds.thompson = key;

                        break;
                    }
                case "custom smg":
                    {
                        binds.smg = key;

                        break;
                    }
                case "sar":
                    {
                        binds.sar = key;

                        break;
                    }
                case "python":
                    {
                        binds.python = key;

                        break;
                    }
                case "m39":
                    {
                        binds.m39 = key;

                        break;
                    }
                case "m92":
                    {
                        binds.m92 = key;

                        break;
                    }
                case "p2":
                    {
                        binds.p2 = key;

                        break;
                    }
                case "revolver":
                    {
                        binds.revolver = key;

                        break;
                    }
                case "8x scope":
                    {
                        binds.eight = key;

                        break;
                    }
                case "16x scope":
                    {
                        binds.sixteen = key;

                        break;
                    }
                case "holo sight":
                    {
                        binds.holo = key;

                        break;
                    }
                case "simple sight":
                    {
                        binds.simple = key;

                        break;
                    }
                case "muzzle boost":
                    {
                        binds.boost = key;

                        break;
                    }
                case "muzzle brake":
                    {
                        binds.brake = key;

                        break;
                    }
                case "silencer":
                    {
                        binds.silencer = key;

                        break;
                    }
            }

            MessageBox.Show(WeaponsKeybindCombo.Text + " has been bound to " + e.Key.ToString());

            SaveBinds();
            RegisterHotkeys();
        }

        private void HelpersKeybindBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            HelpersKeybindBox.Text = "";
        }

        private void HelpersKeybindBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!CheckKey(e.Key)) return;

            int key = KeyInterop.VirtualKeyFromKey(e.Key);

            if (binds == null) return;

            switch (HelpersKeybindCombo.Text)
            {
                case "split large furnace":
                    {
                        binds.splitLarge = key;

                        break;
                    }
                case "split small furnace":
                    {
                        binds.splitSmall = key;

                        break;
                    }
                case "auto code lock":
                    {
                        binds.autoCode = key;

                        break;
                    }
                case "auto upgrade":
                    {
                        binds.autoUpgrade = key;

                        break;
                    }
            }

            MessageBox.Show(HelpersKeybindCombo.Text + " has been bound to " + e.Key.ToString());

            SaveBinds();
            RegisterHotkeys();
        }

        private void arrowKeyCycleCheck_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void MiscKeybindBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MiscKeybindBox.Text = "";
        }

        private void MiscKeybindBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!CheckKey(e.Key)) return;

            int key = KeyInterop.VirtualKeyFromKey(e.Key);

            if (binds == null) return;

            switch (MiscKeybindCombo.Text)
            {
                case "toggle recoil":
                    {
                        binds.toggle = key;

                        break;
                    }
                case "hide script":
                    {
                        binds.hide = key;

                        break;
                    }
                case "activate overlay":
                    {
                        MessageBox.Show("a");
                        binds.toggleoverlay = key;

                        break;
                    }
            }

            MessageBox.Show(MiscKeybindCombo.Text + " has been bound to " + e.Key.ToString());

            SaveBinds();
            RegisterHotkeys();
        }

        private void clearMiscBind_Click(object sender, RoutedEventArgs e)
        {
            MiscKeybindBox_KeyDown(null, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, 0));
        }

        private void clearHelperBind_Click(object sender, RoutedEventArgs e)
        {

            HelpersKeybindBox_KeyDown(null, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, 0));
        }

        private void clearWeaponBind_Click(object sender, RoutedEventArgs e)
        {
            WeaponsKeybindBox_KeyDown(null, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, 0));
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

    }
}