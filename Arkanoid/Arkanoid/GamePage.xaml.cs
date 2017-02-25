using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Arkanoid
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {

        Accelerometer _acc;
        Rectangle[,] _rects;

        public GamePage()
        {
            this.InitializeComponent();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            this.setupGameField(3,10);
        }

        private void setupGameField(int rows, int columns)
        {
            //Canvas size width 600 height 300
            _rects = new Rectangle[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Height = (GameCanvas.Width / 20);
                    rect.Width = (GameCanvas.Width / columns);
                    rect.Stroke = new SolidColorBrush(Colors.Black);
                    rect.Fill = new SolidColorBrush(Colors.Magenta);
                    rect.Tapped += Rect_Tapped;
                    Canvas.SetLeft(rect, rect.Width * j);
                    Canvas.SetTop(rect, rect.Height * i);
                    _rects[i,j] = rect;
                    GameCanvas.Children.Add(_rects[i, j]);
                }
            }
        }




        private void Rect_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Test Sample
            Rectangle rect = (Rectangle)sender;
            Size rectSize = new Size(rect.Width, rect.Height);
            Point loc = new Point();
            Rect re = new Rect(loc, rectSize);
            re.Intersect(re);
            GameCanvas.Children.Remove(rect);

        }
    }
}
