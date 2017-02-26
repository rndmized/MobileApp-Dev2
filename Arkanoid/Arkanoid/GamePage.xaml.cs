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
        Brick[,] _bricks;
        List<Brick> _bricks2;
        Paddle paddle;
        Ball ball;

        bool isStarted = false;
        double xAxis = -5;
        double yAxis = -5;

        

        public GamePage()
        {
            this.InitializeComponent();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            
            this.Loaded += MainPage_Loaded;
            this.Unloaded += MainPage_Unloaded;
        }

        private void setupGameField1(int rows, int columns)
        {
            //Canvas size width 600 height 350

            int height = (int)GameCanvas.Width / 20;
            int width = (int)GameCanvas.Width / columns;
            
            //Adding Bricks 1.0
            /*
            _bricks = new Brick[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int x = (int)width * j;
                    int y = (int)height * i;
                    Brick brick = new Brick(x, y, width, height);
                    brick.getBrick().Tapped += Rect_Tapped;
                    Canvas.SetLeft(brick.getBrick(), x);
                    Canvas.SetTop(brick.getBrick(), y);
                    _bricks[i, j] = brick;

                    GameCanvas.Children.Add(_bricks[i,j].getBrick());
                }
            }
            */
            //Adding bricks 2.0
            _bricks2 = new List<Brick>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int x = (int)width * j;
                    int y = (int)height * i;
                    Brick brick = new Brick(x, y, width, height);
                    brick.getBrick().Tapped += Rect_Tapped;
                    Canvas.SetLeft(brick.getBrick(), x);
                    Canvas.SetTop(brick.getBrick(), y);
                    _bricks2.Add(brick);
                }
            }
            foreach (Brick brick in _bricks2)
            {
                GameCanvas.Children.Add(brick.getBrick());
            }

            //Adding Paddle 1.0
            paddle = new Paddle((int)(GameCanvas.Width / 2) - 50, (int)(GameCanvas.Height) - 6, 100, 6);
            Rectangle paddleRect = new Rectangle();
            Canvas.SetLeft(paddle.getPaddle(), paddle.getX());
            Canvas.SetTop(paddle.getPaddle(), paddle.getY());
            GameCanvas.Children.Add(paddle.getPaddle());

            //Adding Ball 1.0
            ball = new Ball(paddle.getX() + (paddle.getWidth() / 2), paddle.getY() - 10, 10,10);
            Canvas.SetLeft(ball.getBall(), ball.getX());
            Canvas.SetTop(ball.getBall(), ball.getY());
            GameCanvas.Children.Add(ball.getBall());

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
                updatePaddlePosition("left");
            }
            else if (args.VirtualKey == Windows.System.VirtualKey.Right)
            {
                updatePaddlePosition("right");
            }
            else if (args.VirtualKey == Windows.System.VirtualKey.Space)
            {
                startGame();

            }
        }

        private void startGame()
        {
            isStarted = true;
            _timer.Start();
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
            setupTimers();
            
        } 

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
                _timer.Interval = new TimeSpan(0, 0, 0, 0, 20); // 100 ms
                _timer.Tick += _timer_Tick;
            }
        }

        private void _timer_Tick(object sender, object e)
        {
            // move the block in a while, catch the key down event
            // create a key listener event
            //updateEllipsePosition();
            updateBallPosition();

        }


        #endregion

        private async void _myAcc_Shaken(Accelerometer sender, AccelerometerShakenEventArgs args)
        {
            // tell the user not to shake the phone;
            MessageDialog msgDialog = new MessageDialog("that's not a good idea");
            await msgDialog.ShowAsync();
        }

        private async void _myAcc_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
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

        private void updateBallPosition()
        {

            if (isStarted)
            {
                double xPos = ball.getX();
                double yPos = ball.getY();

                if (xPos > GameCanvas.Width - ball.getWidth())
                {
                    Canvas.SetLeft(ball.getBall(), GameCanvas.ActualWidth - ball.getWidth());
                    xAxis = xAxis * -1;
                }
                if (xPos < 0)
                {
                    Canvas.SetLeft(ball.getBall(), 0);
                    xAxis = xAxis * -1;
                }
                if (yPos > GameCanvas.Height - ball.getHeight())
                {
                    Canvas.SetTop(ball.getBall(), GameCanvas.ActualHeight - ball.getHeight());
                    yAxis = yAxis * -1;
                }
                if (yPos < 0)
                {
                    Canvas.SetTop(ball.getBall(), 0);
                    yAxis = yAxis * -1;
                }
                foreach (Brick brick in _bricks2)
                {
    
                        if (brick.collides(ball.getHitBox()))
                        {
                            brick.Break();
                            if (brick.isBrickBroken())
                            {
                                _bricks2.Remove(brick);
                                //(Rectangle)GameCanvas.FindName(brick.getX().ToString() + "_" + brick.getY().ToString())
                                GameCanvas.Children.Remove(brick.getBrick());
                            }
                        if (ball.getX() >= brick.getX() && ball.getX() <= brick.getX() + brick.getWidth() || (ball.getX() * ball.getHeight()) >= brick.getX() && (ball.getX() * ball.getHeight()) <= brick.getX() + brick.getWidth())
                        {
                             yAxis*= -1;
                        }
                        else if (ball.getY() >= brick.getY() && ball.getY() <= brick.getY() + brick.getHeight() || (ball.getY() * ball.getHeight()) >= brick.getY() && (ball.getY() * ball.getHeight()) <= brick.getY() + brick.getWidth())
                        {
                            xAxis *= -1;
                        }
                        break;
                    }
 
                }

                tblStats.Text = "xPos: " + xPos.ToString() + "yPos: " + yPos.ToString() + "xAxis vector: " + xAxis.ToString() + "yAxis vector: " + yAxis.ToString();
                xPos += xAxis;
                yPos += yAxis;
                ball.setX((int)xPos);
                ball.setY((int)yPos);
                Canvas.SetTop(ball.getBall(), ball.getY());
                Canvas.SetLeft(ball.getBall(), ball.getX());

            }
        }

        private void updateUI(AccelerometerReading reading)
        {
          
            updatePaddlePosition(reading);
        }

        private void updatePaddlePosition(AccelerometerReading reading)
        {

            // use the AccelerationX: if >0 move right
            //                        if <0 move left

            if (reading.AccelerationY < 0)
            {
                if (!((double)paddle.getPaddle().GetValue(Canvas.LeftProperty) <= 0))
                {
                    // move left
                    paddle.getPaddle().SetValue(Canvas.LeftProperty, (double)paddle.getPaddle().GetValue(Canvas.LeftProperty) - increment);
                }
            }
            else
            {
                if (!((double)paddle.getPaddle().GetValue(Canvas.LeftProperty) >= (GameCanvas.ActualWidth - paddle.getPaddle().Width)))
                {
                    // move right
                    paddle.getPaddle().SetValue(Canvas.LeftProperty, (double)paddle.getPaddle().GetValue(Canvas.LeftProperty) + increment);
                }
            }
        }

        int increment = 5;

        private void updatePaddlePosition(string direction)
        {
            if (isStarted)
            {
                if (direction == "left")
                {
                    if (!((double)paddle.getPaddle().GetValue(Canvas.LeftProperty) <= 0))
                    {
                        // move left
                        paddle.getPaddle().SetValue(Canvas.LeftProperty, (double)paddle.getPaddle().GetValue(Canvas.LeftProperty) - increment);
                    }
                }
                else if (direction == "right")
                {
                    if (!((double)paddle.getPaddle().GetValue(Canvas.LeftProperty) >=
                            (GameCanvas.ActualWidth - paddle.getPaddle().Width)
                          )
                       )
                    {
                        // move right
                        paddle.getPaddle().SetValue(Canvas.LeftProperty, (double)paddle.getPaddle().GetValue(Canvas.LeftProperty) + increment);
                    }
                }

            } else
            {
                if (direction == "left")
                {
                    if (!((double)paddle.getPaddle().GetValue(Canvas.LeftProperty) <= 0))
                    {
                        // move left
                        ball.setX(ball.getX()-increment);
                        Canvas.SetLeft(ball.getBall(), ball.getX());
                        paddle.getPaddle().SetValue(Canvas.LeftProperty, (double)paddle.getPaddle().GetValue(Canvas.LeftProperty) - increment);
                    }
                }
                else if (direction == "right")
                {
                    if (!((double)paddle.getPaddle().GetValue(Canvas.LeftProperty) >=
                            (GameCanvas.ActualWidth - paddle.getPaddle().Width)
                          )
                       )
                    {
                        // move right
                        ball.setX(ball.getX() + increment);
                        Canvas.SetLeft(ball.getBall(), ball.getX());
                       paddle.getPaddle().SetValue(Canvas.LeftProperty, (double)paddle.getPaddle().GetValue(Canvas.LeftProperty) + increment);
                    }
                }
            }     
        } 
    }
}
