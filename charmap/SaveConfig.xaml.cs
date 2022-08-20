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

namespace charmap
{
    /// <summary>
    /// Interaction logic for SaveConfig.xaml
    /// </summary>
    public partial class SaveConfig : Window
    {
        public string name;
        public bool guncheck;
        public bool randomcheck;

        public SaveConfig()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool gun = (bool)GunsCheck.IsChecked;
            bool random = (bool)RandomCheck.IsChecked;

            if (NameTextBox.Text == null || NameTextBox.Text == "")
            {
                MessageBox.Show("Please enter a name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!gun && !random)
            {
                MessageBox.Show("Please select an option.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            name = NameTextBox.Text;
            guncheck = (bool)GunsCheck.IsChecked;
            randomcheck = (bool)RandomCheck.IsChecked;

            this.Close();
        }
    }
}
