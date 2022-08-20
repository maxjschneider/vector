using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace charmap
{
    /// <summary>
    /// Interaction logic for FlatSlider.xaml
    /// </summary>
    public partial class FlatSlider : UserControl
    {
        public event EventHandler SliderMoved;

        private int value = 50;
        
        public int Value
        {
            get
            {
                return this.value;
            } set
            {
                this.Slider.Width = (Convert.ToDouble(value) / 100D) * Container.Width;
                this.value = value;

                SliderMoved(this, EventArgs.Empty);
            }
        }

        private bool down = false;
        private bool inBounds = false;

        public FlatSlider()
        {
            InitializeComponent();
        }

        private double Clamp(double val)
        {
            if (val < 0.01) return 0.0D;
            else if (val > 1) return 1D;

            return val;
        }

        private void MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            down = true;

            Point point = e.GetPosition(Container);
            double val = Convert.ToDouble(point.X) / Convert.ToDouble(Container.Width);

            Slider.Width = Convert.ToInt32(val * Convert.ToDouble(Container.Width));

            Value = Convert.ToInt32(val * 100);
        }

        private void MouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            down = false;
        }

        private void Container_MouseMove(object sender, MouseEventArgs e)
        {
            if (down && inBounds)
            {
                Point point = e.GetPosition(Container);
                double val = Convert.ToDouble(point.X) / Convert.ToDouble(Container.Width);
                val = Clamp(val);

                Slider.Width = Convert.ToInt32(val * Convert.ToDouble(Container.Width));

                Value = Convert.ToInt32(val * 100);
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            inBounds = false;

            if (!PInvoke.IsKey(0x01)) down = false;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            inBounds = true;

            if (!PInvoke.IsKey(0x01)) down = false;
        }
    }
}
