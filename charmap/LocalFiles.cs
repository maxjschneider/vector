using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Forms;

namespace charmap
{
    class steam
    {
        public static bool EnableRapid()
        {
            string path = GetRustPath() + @"\cfg\keys.cfg";

            if (FindString(path, "bind backslash \"+attack\""))
            {
                return true;
            } else
            {
                return AddString(path, "bind backslash \"+attack\"");
            }
        }

        #region path fetchers 
        private static string GetSteamPath()
        {
            string path = null;

            try
            {
                path = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamPath", null) as string;
            }
            catch
            {
                MessageBox.Show("Failed to find Steam installation folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return path;
        }

        public static string GetRustPath()
        {
            string path = null;

            try
            {
                string steampath = GetSteamPath();

                if (File.Exists(steampath + @"\steamapps\appmanifest_252490.acf"))
                {
                    path = steampath + @"\steamapps\common\Rust";
                }
                else
                {
                    if (MessageBox.Show("Rust installation folder could be found.\nPlease select the folder in the folder browser.", null, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                    {
                        using (FolderBrowserDialog folder = new FolderBrowserDialog())
                        {
                            DialogResult result = folder.ShowDialog();

                            if (result == DialogResult.OK && !string.IsNullOrEmpty(folder.SelectedPath)) path = folder.SelectedPath;
                        }
                    }
                    else return null;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to find Rust installation folder.\n\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return path;
        }
        #endregion

        public static bool AddString(string path, string entry)
        {
            try
            {
                File.AppendAllText(path, "\n" + entry);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool FindString(string path, string text)
        {
            foreach (var line in File.ReadAllLines(path))
            {
                if (line.Contains(text)) return true;
            }

            return false;
        }
    }
}
