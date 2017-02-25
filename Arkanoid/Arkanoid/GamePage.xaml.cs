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
using Windows.UI.Core;
using Windows.UI.Popups;
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
        Accelerometer _myAcc;
        Rectangle[,] _rects;
        Brick[,] _bricks;

        Brick paddle; 

        public GamePage()
        {
            this.InitializeComponent();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            
            this.Loaded += MainPage_Loaded;
            this.Unloaded += MainPage_Unloaded;
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

            //Adding Paddle
            paddle = new Brick((int)(GameCanvas.Width / 2) - 50, (int)(GameCanvas.Height) - 10, 100, 10);
            Rectangle paddleRect = new Rectangle();
            paddleRect.Height = paddle.getHeight();
            paddleRect.Width = paddle.getWidth();
            paddleRect.Stroke = new SolidColorBrush(Colors.Black);
            paddleRect.Fill = new SolidColorBrush(Colors.Black);
            
            paddle.setRectangle(paddleRect);
            Canvas.SetLeft(paddle.getBrick(), paddle.getX());
            Canvas.SetTop(paddle.getBrick(), paddle.getY());
            GameCanvas.Children.Add(paddle.getBrick());
            
            //Adding Ball
            Ellipse elBall = new Ellipse();
            elBall.Height = 10;
            elBall.Width = 10;
            elBall.Stroke = new SolidColorBrush(Colors.Black);
            elBall.Fill  = new SolidColorBrush(Colors.Yellow);
            Canvas.SetLeft(elBall, paddle.getX()+(paddle.getWidth()/2));
            Canvas.SetTop(elBall, paddle.getY()-10);
            GameCanvas.Children.Add(elBall);


        }



        
        private void Rect_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Test Sample
            Rectangle rect = (Rectangle)sender;
            GameCanvas.Children.Remove(rect);

        }

        private uint _desiredReportInterval;

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Left)
            {
                updateEllipsePosition("left");
            }
            else if (args.VirtualKey == Windows.System.VirtualKey.Right)
            {
                updateEllipsePosition("right");
            }
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            // tidy up and leave things as you found them.
            // could save state here
            if (_myAcc != null)
            {
                _myAcc.ReadingChanged -= _myAcc_ReadingChanged;
                _myAcc.Shaken -= _myAcc_Shaken;
            }
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.setupGameField1(3, 10);
            // check first if there is an accelerometer
            await checkForAccelerometer();

            // centre the ellipse

           // elMonday.SetValue(Canvas.LeftProperty, (cvsMonday.ActualWidth - elMonday.Width) / 2);
        } // end loaded function


        private async System.Threading.Tasks.Task checkForAccelerometer()
        {
            _myAcc = Accelerometer.GetDefault();
            MessageDialog msgDialog = new MessageDialog("Standard Message");
            if (_myAcc == null)
            {
                // no accelerometer available, do something else
                // add the eventhandler for the key press here rather than in 
                // constructor
                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

                msgDialog.Content = "no accelerometer available!";
                await msgDialog.ShowAsync();
            }
            else
            {
                // create the event handlers for readings
                // changed and the shaken event.
                _myAcc.ReadingChanged += _myAcc_ReadingChanged;
                _myAcc.Shaken += _myAcc_Shaken;
                // set the report intervals in milliseconds
                uint minReportInterval = _myAcc.MinimumReportInterval;
                /*
                 * compare the min report interval against the requested 
                 * interval of 100 milliseconds
                 * if the minimum possible value is > 100, then use the 
                 * accelerometer min value, otherwise use the requested value
                 * of 100 milliseconds.
                 */
                _desiredReportInterval = minReportInterval > 100 ? minReportInterval : 100;
                _myAcc.ReportInterval = _desiredReportInterval;
            }
        }

        #region Timers Information
        private void setupTimers()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = new TimeSpan(0, 0, 0, 0, 100); // 100 ms
                _timer.Tick += _timer_Tick;
            }
        }

        private void _timer_Tick(object sender, object e)
        {
            // move the block in a while, catch the key down event
            // create a key listener event

        }


        #endregion

        private async void _myAcc_Shaken(Accelerometer sender, AccelerometerShakenEventArgs args)
        {
            // tell the user not to shake the phone;
            MessageDialog msgDialog = new MessageDialog("that's not a good idea");
            await msgDialog.ShowAsync();
        }

        private async void _myAcc_ReadingChanged(Accelerometer sender,
            AccelerometerReadingChangedEventArgs args)
        {
            // update the UI with the reading.
            /*
             * The () => {} construct is called a lambda expression
             * This is an anonymous function that can be used to create delegate methods
             * of passed as arguments or returned as the value of a function call.
             * In this case, the Accelerometer thread is asking the UI thread 
             * to update some display information from the readings.
             */
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    // update the UI from here.
                    AccelerometerReading reading = args.Reading;
                    updateUI(reading);
                }
            );

        }

        private void updateUI(AccelerometerReading reading)
        {
          
            updateEllipsePosition(reading);
        }

        private void updateEllipsePosition(AccelerometerReading reading)
        {

            // use the AccelerationX: if >0 move right
            //                        if <0 move left

            if (reading.AccelerationX < 0)
            {
                if (!((double)paddle.getBrick().GetValue(Canvas.LeftProperty) <= 0))
                {
                    // move left
                    paddle.getBrick().SetValue(Canvas.LeftProperty,
                    (double)paddle.getBrick().GetValue(Canvas.LeftProperty) - increment);
                }
            }
            else
            {
                if (!((double)paddle.getBrick().GetValue(Canvas.LeftProperty) >= (GameCanvas.ActualWidth - paddle.getBrick().Width)))
                {
                    // move right
                    paddle.getBrick().SetValue(Canvas.LeftProperty,
                    (double)paddle.getBrick().GetValue(Canvas.LeftProperty) + increment);
                }
            }
        }

        double increment = 5;


        private void updateEllipsePosition(string direction)
        {
            if (direction == "left")
            {
                if (!((double)paddle.getBrick().GetValue(Canvas.LeftProperty) <= 0))
                {
                    // move left
                    paddle.getBrick().SetValue(Canvas.LeftProperty,
                    (double)paddle.getBrick().GetValue(Canvas.LeftProperty) - increment);
                }
            }
            else if (direction == "right")
            {
                if (!((double)paddle.getBrick().GetValue(Canvas.LeftProperty) >=
                        (GameCanvas.ActualWidth - paddle.getBrick().Width)
                      )
                   )
                {
                    // move right
                    paddle.getBrick().SetValue(Canvas.LeftProperty,
                    (double)paddle.getBrick().GetValue(Canvas.LeftProperty) + increment);
                }
            }
            // add the up and down arrow keys to change the Canvas.Top
        } // end update ellipse
    }
}
