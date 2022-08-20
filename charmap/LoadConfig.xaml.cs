using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace charmap
{
    /// <summary>
    /// Interaction logic for LoadConfig.xaml
    /// </summary>
    public partial class LoadConfig : Window
    {
        List<string> configs;

        public Config ret;

        public LoadConfig()
        {
            InitializeComponent();

            UpdateCombo();
        }

        private void UpdateCombo()
        {
            try
            {
                ConfigCombo.Items.Clear();

                configs = new List<string>();

                configs = JsonConvert.DeserializeObject<List<string>>(Properties.Config.Default.config);

                for (int i = 0; i < configs.Count; i++)
                {
                    Config config = JsonConvert.DeserializeObject<Config>(configs[i]);

                    ConfigCombo.Items.Add(config.name);
                }
            }
            catch
            {
                MessageBox.Show("Could not load configs!\n\n", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                Properties.Config.Default.config = "";
                Properties.Config.Default.Save();

                this.Close();
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (configs.Count == 0) return;

            ret = JsonConvert.DeserializeObject<Config>(configs[ConfigCombo.SelectedIndex]);

            this.Close();
        }

        private void ConfigCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (configs.Count == 0) return;

            configs.RemoveAt(ConfigCombo.SelectedIndex);

            Properties.Config.Default.config = JsonConvert.SerializeObject(configs);
            Properties.Config.Default.Save();

            UpdateCombo();
        }

}
}
