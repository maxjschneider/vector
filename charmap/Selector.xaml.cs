using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
    /// Interaction logic for Selector.xaml
    /// </summary>
    public partial class Selector : Window
    {
        public Point topLeft;
        public Point bottomRight;

        private Rectangle rect;
        private Point pos;

        public Selector()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            this.Topmost = true;
            this.Opacity = 0.4;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(this);

            rect = new Rectangle();
            
            rect.Stroke = new SolidColorBrush(Colors.Black);
            rect.Fill = new SolidColorBrush(Colors.Black);

            rect.Opacity = 0.75;

            canvas.Children.Add(rect);

            pos = point;

            Canvas.SetLeft(rect, pos.X);
            Canvas.SetTop(rect, pos.Y);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (rect != null && PInvoke.IsKey(0x01))
            {
                Point point = e.GetPosition(rect);
                Point screencoords = e.GetPosition(this);

                Console.WriteLine(point);
                if (screencoords.X > pos.X)
                {
                    rect.Width = point.X;
                } else
                {
                    Canvas.SetLeft(rect, screencoords.X);

                    rect.Width = Math.Abs(screencoords.X - pos.X);
                }

                if (screencoords.Y > pos.Y)
                {
                    rect.Height = point.Y;
                } else
                {
                    Canvas.SetTop(rect, screencoords.Y);

                    rect.Height = Math.Abs(screencoords.Y - pos.Y);
                }
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point screencoords = e.GetPosition(this);

            if (screencoords.X > pos.X)
            {
                topLeft.X = pos.X;
                bottomRight.X = screencoords.X;
            }
            else
            {
                topLeft.X = screencoords.X;
                bottomRight.X = pos.X;
            }

            if (screencoords.Y > pos.Y)
            {
                topLeft.Y = pos.Y;
                bottomRight.Y = screencoords.Y;
            }
            else
            {
                topLeft.Y = screencoords.Y;
                bottomRight.Y = pos.Y;
            }

            SystemSounds.Asterisk.Play();

            this.Close();
        }

    }
}
