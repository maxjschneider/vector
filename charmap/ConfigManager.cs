using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;

namespace charmap
{
    public class Config
    {
        public string name { get; set; }

        public int weapon { get; set; }
        public int scope { get; set; }
        public int barrel { get; set; }

        public int xcontrol { get; set; }
        public int ycontrol { get; set; }
        public int random { get; set; }
        public int shake { get; set; }
        public bool realistic { get; set; }

        public bool saveguns { get; set; }

        public bool saverandom { get; set; }

        public bool humanization { get; set; }

        public int keybind { get; set; }
    }

    class ConfigManager
    {
        string json;

        public ConfigManager(Config config)
        {
            json = JsonConvert.SerializeObject(config);
        }

        public void Save()
        {
            if (Properties.Config.Default.config == "" || Properties.Config.Default.config == null)
            {
                List<string> array = new List<string>();
                array.Add(json);

                string jarray = JsonConvert.SerializeObject(array);

                Properties.Config.Default.config = jarray;
                Properties.Config.Default.Save();
            } else
            {
                try
                {
                    List<string> array = JsonConvert.DeserializeObject<List<string>>(Properties.Config.Default.config);

                    array.Add(json);

                    string jarray = JsonConvert.SerializeObject(array);

                    Properties.Config.Default.config = jarray;
                    Properties.Config.Default.Save();
                } catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Failed to load config!\n\n" + ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                    Properties.Config.Default.config = "";
                    Properties.Config.Default.Save();
                }
            }

        }

    }
}
