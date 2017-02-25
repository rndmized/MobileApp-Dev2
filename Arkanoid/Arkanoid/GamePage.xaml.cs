using Arkanoid.Classes;
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
        DispatcherTimer _timer;
        Accelerometer _acc;
        Rectangle[,] _rects;
        Brick[,] _bricks;

        public GamePage()
        {
            this.InitializeComponent();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            this.setupGameField1(3,10);
        }

        private void setupGameField0(int rows, int columns)
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

        private void setupGameField1(int rows, int columns)
        {
            //Canvas size width 600 height 300

            int height = (int)GameCanvas.Width / 20;
            int width = (int)GameCanvas.Width / columns;
            
            _bricks = new Brick[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int x = (int)width * j;
                    int y = (int)height * i;
                    Brick brick = new Brick(x, y, width, height);

                    Rectangle rect = new Rectangle();
                    rect.Stroke = new SolidColorBrush(Colors.Black);
                    rect.Fill = new SolidColorBrush(Colors.Magenta);
                    rect.Height = height;
                    rect.Width = width;
                    rect.Tapped += Rect_Tapped;
                    brick.setRectangle(rect);

                    Canvas.SetLeft(brick.getBrick(), x);
                    Canvas.SetTop(brick.getBrick(), y);
                    _bricks[i, j] = brick;

                    GameCanvas.Children.Add(_bricks[i,j].getBrick());
                }
            }
        }



        
        private void Rect_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Test Sample
            Rectangle rect = (Rectangle)sender;
            GameCanvas.Children.Remove(rect);

        }
        
    }
}
