using Arkanoid.Classes;
using System;
using System.Collections.Generic;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Arkanoid
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        #region Variables Declaration
        //
        private DispatcherTimer _timer;
        private Accelerometer _myAcc;
        private List<Brick> _bricks;
        private Paddle paddle;
        private Ball ball;
        private StackPanel powerUpController;
        public static Canvas GameCanvas;

        private bool isStarted = false;
        private bool winningCondition = false;
        private int increment = 10;
        private int speedBuff = 0;
        private int slowBuff = 0;
        private TextBlock tbxLives;
        private TextBlock tbxScore;

        #endregion

        public GamePage()
        {
            this.InitializeComponent();
            //Set screen orientation to Landscape mode.
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            //Create load and unload event handlers.
            this.Loaded += MainPage_Loaded;
            this.Unloaded += MainPage_Unloaded;
        }

        #region Load/Unload

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Set up game elements
            this.setupGameField(4, 10);
            //Set up information grid
            this.setupInfoGrid();
            // check first if there is an accelerometer
            await checkForAccelerometer();
            setupTimers();

        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            // tidy up and leave things as you found them.
            // could save state here
            if (_myAcc != null)
            {
                _myAcc.ReadingChanged -= _myAcc_ReadingChanged;
                
            }
            if (_timer != null)
            {
                _timer.Tick -= _timer_Tick;
            }
        }

        #endregion

        #region setup Environment


        private void setupInfoGrid()
        {
            tbxLives = new TextBlock();
            tbxLives.Text = "Lives goes here.";
            tbxLives.Foreground = new SolidColorBrush(Colors.Green);

            tbxScore = new TextBlock();
            tbxScore.Text = MainPage.scoreController.getScore().ToString();
            tbxScore.Foreground = new SolidColorBrush(Colors.Green);

            grdInfo.ColumnDefinitions.Insert(0, new ColumnDefinition());
            grdInfo.ColumnDefinitions.Insert(1, new ColumnDefinition());


            tbxLives.SetValue(Grid.ColumnProperty, 0);
            tbxLives.HorizontalAlignment = HorizontalAlignment.Left;

            tbxScore.SetValue(Grid.ColumnProperty, 1);
            tbxScore.HorizontalAlignment = HorizontalAlignment.Right;


            grdInfo.Children.Add(tbxLives);
            grdInfo.Children.Add(tbxScore);
        }

        private void setupGameField(int rows, int columns)
        {
            //Canvas size width 600 height 350
    
            GameCanvas = new Canvas();
            GameCanvas.Height = spCanvas.Height;
            GameCanvas.Width = spCanvas.Width;
            GameCanvas.Background = new SolidColorBrush(Colors.Black);
            spCanvas.Children.Add(GameCanvas);
            
            //Adding bricks 3.0
            _bricks = new List<Brick>();
            LevelBuilder lb = new LevelBuilder();
            _bricks = lb.getNewRandomLevelLayout(rows, columns, GameCanvas);

            foreach (Brick brick in _bricks)
            {
                GameCanvas.Children.Add(brick.getBrick());
            }

            //Adding Paddle 1.0
            paddle = new Paddle((int)(GameCanvas.Width / 2) - 50, (int)(GameCanvas.Height) - 6, 100, 12);
            Canvas.SetLeft(paddle.getPaddle(), paddle.getX());
            Canvas.SetTop(paddle.getPaddle(), paddle.getY());
            GameCanvas.Children.Add(paddle.getPaddle());

            //Adding Ball 1.0
            ball = new Ball(paddle.getX() + (paddle.getWidth() / 2), paddle.getY() - 11, 10, 10);
            ball.getBall().Tapped += GamePage_Tapped;
            Canvas.SetLeft(ball.getBall(), ball.getX());
            Canvas.SetTop(ball.getBall(), ball.getY());
            GameCanvas.Children.Add(ball.getBall());

            //Adding Power Up Controller
            powerUpController = new StackPanel();
            powerUpController.Name = "PowerUpPanel";
            powerUpController.Orientation = Orientation.Vertical;
            powerUpController.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            powerUpController.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;
            
            

        }

        private void GamePage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            startGame();
        }

        #endregion

        #region Sensor/ Controls setup

        private uint _desiredReportInterval;

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
            else if (args.VirtualKey == Windows.System.VirtualKey.S)
            {
                if (isStarted)
                {
                    if (Math.Abs(ball.getXVector()) < 8)
                    {
                        ball.setXVector((int)(ball.getXVector() * 1.5));
                        ball.setYVector((int)(ball.getYVector() * 1.5));
                    }
                    else
                    {
                        ball.setXVector((int)(ball.getXVector() * 0.5));
                        ball.setYVector((int)(ball.getYVector() * 0.5));
                    }
                    
                }

            }
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


        #endregion

        #region Timers Information
        private void setupTimers()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = new TimeSpan(1000); // 100 ms
                _timer.Tick += _timer_Tick;
            }
        }

        private void _timer_Tick(object sender, object e)
        {
            // move the block in a while, catch the key down event
            // create a key listener event
            //updateEllipsePosition();
            if (isStarted)
            {
                updateBallPosition();
            }
            else if (!isStarted && winningCondition)
            {
                stageOver();
            }
        }

        #endregion

        private void startGame()
        {
            if (ball.getX() > GameCanvas.Width/2)
            {
                ball.setXVector(ball.getXVector() * -1);
            }
            isStarted = true;
            _timer.Start();
        }

        #region Game over
        private void gameOver()
        {

            isStarted = false;
            GameCanvas.Children.Remove(ball.getBall());
            TextBlock tblGameOver = new TextBlock();
            tblGameOver.Text = "GAME OVER";
            tblGameOver.FontSize = 35;
            tblGameOver.Foreground = new SolidColorBrush(Colors.LimeGreen);
            Canvas.SetLeft(tblGameOver, (GameCanvas.Width / 2) - 100);
            Canvas.SetTop(tblGameOver, GameCanvas.Height / 2);
            GameCanvas.Children.Add(tblGameOver);
            _timer.Stop();
            //Back to menu button
            Button btnMenu = new Button();
            btnMenu.Tapped += BtnMenu_Tapped;
            btnMenu.Content = "Main Menu";
            btnMenu.Foreground = new SolidColorBrush(Colors.Green);
            btnMenu.BorderBrush = new SolidColorBrush(Colors.Green);
            btnMenu.BorderThickness = new Thickness(2);
            Canvas.SetLeft(btnMenu, (GameCanvas.Width / 2) - 75);
            Canvas.SetTop(btnMenu, (GameCanvas.Height / 2) + 60);
            GameCanvas.Children.Add(btnMenu);
            //Retry button
            Button btnRetry = new Button();
            btnRetry.Tapped += btnRetry_Tapped;
            btnRetry.Content = "Retry";
            btnRetry.Foreground = new SolidColorBrush(Colors.Green);
            btnRetry.BorderBrush = new SolidColorBrush(Colors.Green);
            btnRetry.BorderThickness = new Thickness(2);
            Canvas.SetLeft(btnRetry, (GameCanvas.Width / 2)+25);
            Canvas.SetTop(btnRetry, (GameCanvas.Height / 2) + 60);
            GameCanvas.Children.Add(btnRetry);

        }

        private void btnRetry_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Clear Score
            GameCanvas.Children.Clear();
            this.Frame.Navigate(typeof(GamePage));
        }

        private void BtnMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
        #endregion

        #region StageOver
        private void stageOver()
        {
            _timer.Stop();
            winningCondition = false;
            GameCanvas.Children.Remove(ball.getBall());
            TextBlock tblstageOver = new TextBlock();
            tblstageOver.Text = "STAGE CLEARED";
            tblstageOver.FontSize = 35;
            tblstageOver.Foreground = new SolidColorBrush(Colors.LimeGreen);
            Button btnStageOver = new Button();
            btnStageOver.Content = "Next Stage";
            btnStageOver.Foreground = new SolidColorBrush(Colors.LimeGreen);
            btnStageOver.Background = new SolidColorBrush(Colors.Black);
            btnStageOver.BorderBrush = new SolidColorBrush(Colors.LimeGreen);
            btnStageOver.BorderThickness = new Thickness(1);
            btnStageOver.Tapped += BtnStageOver_Tapped;

            Canvas.SetLeft(tblstageOver, (GameCanvas.Width / 2) - 90);
            Canvas.SetTop(tblstageOver, GameCanvas.Height / 2);
            Canvas.SetLeft(btnStageOver, (GameCanvas.Width / 2) - 100);
            Canvas.SetTop(btnStageOver, (GameCanvas.Height / 2) - 40);
            GameCanvas.Children.Add(tblstageOver);
            GameCanvas.Children.Add(btnStageOver);

        }

        private void BtnStageOver_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GameCanvas.Children.Clear();
            this.Frame.Navigate(typeof(GamePage));
        }
        #endregion

        #region GUI Updates

        private void updateBallPosition()
        {

            if (isStarted)
            {
                if (_bricks.Count <= 0)
                {
                    winningCondition = true;
                    isStarted = false;

                }
                else
                {

                
                float xPos = ball.getX();
                float yPos = ball.getY();

                if (xPos > GameCanvas.Width - ball.getWidth())
                {
                    Canvas.SetLeft(ball.getBall(), GameCanvas.ActualWidth - ball.getWidth());
                        ball.setXVector(ball.getXVector()*-1);
                }
                if (xPos < 0)
                {
                    Canvas.SetLeft(ball.getBall(), 0);
                        ball.setXVector(ball.getXVector() * -1);
                }
                if (yPos > GameCanvas.Height - ball.getHeight())
                {
                    gameOver();
                }
                if (yPos < 0)
                {
                    Canvas.SetTop(ball.getBall(), 0);
                        ball.setYVector(ball.getYVector() * -1);
                }
                if (ball.getY() < 150)
                {
                    foreach (Brick brick in _bricks)
                    {

                        if (brick.collides(ball.getHitBox()))
                        {
                            brick.Break();
                            this.impactEffect(brick);
                            this.calculateScore();
                            if (brick.isBrickBroken())
                            {
                                _bricks.Remove(brick);
                                GameCanvas.Children.Remove(brick.getBrick());
                            }
                            if (ball.getX() >= brick.getX() && ball.getX() <= brick.getX() + brick.getWidth() || (ball.getX() * ball.getHeight()) >= brick.getX() && (ball.getX() * ball.getHeight()) <= brick.getX() + brick.getWidth())
                            {
                                    ball.setYVector(ball.getYVector() * -1);
                            }
                            else if (ball.getY() >= brick.getY() && ball.getY() <= brick.getY() + brick.getHeight() || (ball.getY() * ball.getHeight()) >= brick.getY() && (ball.getY() * ball.getHeight()) <= brick.getY() + brick.getWidth())
                            {
                                    ball.setXVector(ball.getXVector() * -1);
                            }
                            break;
                        }
                    }
                }

                if (ball.getY() > 200)
                {
                    if (paddle.collides(ball.getHitBox()))
                    {
                        ball.setY(paddle.getY() - 11);
                        ball.setYVector(ball.getYVector() * -1);
                    }
                }


                xPos += ball.getXVector() * ball.getSpeed();
                yPos += ball.getYVector() * ball.getSpeed();
                ball.setX((int)xPos);
                ball.setY((int)yPos);
                Canvas.SetTop(ball.getBall(), ball.getY());
                Canvas.SetLeft(ball.getBall(), ball.getX());
                }
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

            if (isStarted)
            {
                if (reading.AccelerationY > 0.04)
                {
                    if (!((double)paddle.getPaddle().GetValue(Canvas.LeftProperty) <= 0))
                    {
                        // move left
                        paddle.setX(paddle.getX() - increment);
                        Canvas.SetLeft(paddle.getPaddle(), paddle.getX());
                    }
                }
                else if (reading.AccelerationY < -0.04)
                {
                    if (!((double)paddle.getPaddle().GetValue(Canvas.LeftProperty) >= (GameCanvas.ActualWidth - paddle.getPaddle().Width)))
                    {
                        // move right
                        paddle.setX(paddle.getX() + increment);
                        Canvas.SetLeft(paddle.getPaddle(), paddle.getX());
                    }
                }

            }
            else
            {
                if (reading.AccelerationY > 0.1)
                {
                    if (!((double)paddle.getPaddle().GetValue(Canvas.LeftProperty) <= 0))
                    {
                        // move left
                        ball.setX(ball.getX() - increment);
                        Canvas.SetLeft(ball.getBall(), ball.getX());
                        paddle.setX(paddle.getX() - increment);
                        Canvas.SetLeft(paddle.getPaddle(), paddle.getX());
                    }
                }
                else if (reading.AccelerationY < -0.1)
                {
                    if (!((double)paddle.getPaddle().GetValue(Canvas.LeftProperty) >= (GameCanvas.ActualWidth - paddle.getPaddle().Width)))
                    {
                        // move right
                        ball.setX(ball.getX() + increment);
                        Canvas.SetLeft(ball.getBall(), ball.getX());
                        paddle.setX(paddle.getX() + increment);
                        Canvas.SetLeft(paddle.getPaddle(), paddle.getX());
                    }
                }
            }

    }

        private void updatePaddlePosition(string direction)
        {
            if (isStarted)
            {
                if (direction == "left")
                {
                    if (!((double)paddle.getPaddle().GetValue(Canvas.LeftProperty) <= 0))
                    {
                        // move left
                        paddle.setX(paddle.getX() - increment);
                        Canvas.SetLeft(paddle.getPaddle(), paddle.getX());
                    }
                }
                else if (direction == "right")
                {
                    if (!((double)paddle.getPaddle().GetValue(Canvas.LeftProperty) >= (GameCanvas.ActualWidth - paddle.getPaddle().Width)))
                    {
                        // move right
                        paddle.setX(paddle.getX() + increment);
                        Canvas.SetLeft(paddle.getPaddle(), paddle.getX());
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
                        paddle.setX(paddle.getX() - increment);
                        Canvas.SetLeft(paddle.getPaddle(), paddle.getX());
                    }
                }
                else if (direction == "right")
                {
                    if (!((double)paddle.getPaddle().GetValue(Canvas.LeftProperty) >= (GameCanvas.ActualWidth - paddle.getPaddle().Width)))
                    {
                        // move right
                        ball.setX(ball.getX() + increment);
                        Canvas.SetLeft(ball.getBall(), ball.getX());
                        paddle.setX(paddle.getX() + increment);
                        Canvas.SetLeft(paddle.getPaddle(), paddle.getX());
                    }
                }
            }     
        }

        #endregion

        #region Game Mechanics

        private void impactEffect(Brick brick)
        {
            if (brick.GetType()== typeof(SpeedBrick)) {
                this.addPowerUp("speed");
            } else if (brick.GetType() == typeof(SlowBrick))
            {
                this.addPowerUp("slow");
            }

        }

        public void addPowerUp(String buff)
        {
            Ellipse buffSphere = new Ellipse();
            buffSphere.Height = 30;
            buffSphere.Width = 30;
            buffSphere.Opacity = 0.5;
            buffSphere.Tapped += BuffSphere_Tapped;
            buffSphere.Stroke = new SolidColorBrush(Colors.WhiteSmoke);


            switch (buff)
            {
                case "speed":
                    speedBuff++;
                    if (speedBuff == 1)
                    {
                        buffSphere.Name = "speedBuff";
                        buffSphere.Fill = new SolidColorBrush(Colors.CadetBlue);
                        this.addNewPowerUp(buffSphere, 120);
                    }
                    break;
                case "slow":
                    slowBuff++;
                    if (slowBuff == 1)
                    {
                        buffSphere.Name = "slowBuff";
                        buffSphere.Fill = new SolidColorBrush(Colors.DarkSalmon);
                        this.addNewPowerUp(buffSphere, 60);
                    }
                    break;
                default:
                    break;
            }
           

        }

        private void BuffSphere_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Ellipse buff = (Ellipse)sender;
            
            if (buff.Name.Equals("speedBuff"))
            {
                speedBuff--;
                if (speedBuff < 1)
                {
                    GameCanvas.Children.Remove(buff);
                }
                ball.speedUp();
            }
            if (buff.Name.Equals("slowBuff"))
            {
                slowBuff--;
                if (slowBuff < 1)
                {
                    GameCanvas.Children.Remove(buff);
                }
                ball.speedDown();
            }
        }

        private void addNewPowerUp(Ellipse powerUp, int height)
        {
            
            Canvas.SetLeft(powerUp, (GameCanvas.ActualWidth - 60));
            Canvas.SetTop(powerUp, (GameCanvas.ActualHeight - height/1.5));
            GameCanvas.Children.Add(powerUp);
        }

        private void calculateScore()
        {
            int speed = (int)ball.getSpeed();
            int scorePoint = speed - 5;
            if (scorePoint >= 0)
            {
                MainPage.scoreController.addScore(100+(50*(scorePoint+1)));
            }
            else
            {
                MainPage.scoreController.addScore(100 - (10 * (Math.Abs(scorePoint) + 1)));
            }
            tbxScore.Text = "Score: " + MainPage.scoreController.getScore().ToString();
            
            
        }
        


        #endregion

    }
}
