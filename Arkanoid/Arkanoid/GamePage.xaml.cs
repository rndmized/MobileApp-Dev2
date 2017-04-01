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

        /*
         * This function sets up view elements such as user profile and score.
         */
        private void setupInfoGrid()
        {

            tbxLives = new TextBlock();
            tbxLives.Text = "User: " + MainPage.user.username;
            tbxLives.Foreground = new SolidColorBrush(Colors.Green);

            tbxScore = new TextBlock();
            tbxScore.Text = "Score: " + MainPage.scoreController.getScore().ToString();
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

        /*
         * Instantiate GameCanvas and add game elements into it: ball, paddle and bricks.
         * */
        private void setupGameField(int rows, int columns)
        {
            //Canvas size width 600 height 350
            //Settting up canvas
            GameCanvas = new Canvas();
            GameCanvas.Height = spCanvas.Height;
            GameCanvas.Width = spCanvas.Width;
            GameCanvas.Background = new SolidColorBrush(Colors.Black);
            spCanvas.Children.Add(GameCanvas);
            GameCanvas.Tapped += GameCanvas_Tapped;

            //Adding bricks 3.0
            _bricks = new List<Brick>();
            LevelBuilder lb = new LevelBuilder();
            _bricks = lb.getNewRandomLevelLayout(rows, columns, GameCanvas);

            foreach (Brick brick in _bricks)
            {
                GameCanvas.Children.Add(brick.getBrick());
            }

            //Adding Paddle 1.0
            paddle = new Paddle((int)(GameCanvas.Width / 2) - 50, (int)(GameCanvas.Height) - 6, 100, 6);
            Canvas.SetLeft(paddle.getPaddle(), paddle.getX());
            Canvas.SetTop(paddle.getPaddle(), paddle.getY());
            GameCanvas.Children.Add(paddle.getPaddle());

            //Adding Ball 1.0
            ball = new Ball(paddle.getX() + (paddle.getWidth() / 2), paddle.getY() - 11, 10, 10);
            Canvas.SetLeft(ball.getBall(), ball.getX());
            Canvas.SetTop(ball.getBall(), ball.getY());
            GameCanvas.Children.Add(ball.getBall());

        }
        //On canvas tapped start game
        private void GameCanvas_Tapped(object sender, TappedRoutedEventArgs e)
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
            //detect the key pressed 
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

                msgDialog.Content = "No accelerometer available! Use arrow keys instead. Click inside the sreen window to start.";
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
            //If no timer create new timer.
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = new TimeSpan(1000); // 100 ms
                _timer.Tick += _timer_Tick;
            }
        }

        private void _timer_Tick(object sender, object e)
        {
            // check if the game is running, if so update ball updateBallPosition
            if (isStarted)
            {
                updateBallPosition();
            }//if the game is not started and the winningCondition is true advane to new stage.
            else if (!isStarted && winningCondition)
            {
                stageOver();
            }
        }

        #endregion

        private void startGame()
        {
            //Decide whether the ball goes right or left at the start
            if ((ball.getX() > GameCanvas.Width / 2) && (!isStarted))
            {
                ball.setXVector(ball.getXVector() * -1);
            }
            //set started to true and start timer
            isStarted = true;
            _timer.Start();
        }

        #region Game over
        private void gameOver()
        {
            //stop game
            isStarted = false;
            //Save score 
            MainPage.sqliteController.saveScore(MainPage.user, MainPage.scoreController.getScore());

            //Display Game over and add buttons to view.
            GameCanvas.Children.Remove(ball.getBall());
            //Game Over text
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
            Canvas.SetLeft(btnRetry, (GameCanvas.Width / 2) + 25);
            Canvas.SetTop(btnRetry, (GameCanvas.Height / 2) + 60);
            GameCanvas.Children.Add(btnRetry);

        }

        private void btnRetry_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Clear Score
            MainPage.scoreController.resetScore();
            //reload page
            this.Frame.Navigate(typeof(GamePage));
        }

        private void BtnMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Navigate to main page
            this.Frame.Navigate(typeof(MainPage));
        }
        #endregion

        #region StageOver
        private void stageOver()
        {
            //Stop game, display Text and show button to go to next stage.
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
            //Reload page
            this.Frame.Navigate(typeof(GamePage));
        }
        #endregion

        #region GUI Updates
        //Checks ball position and updates it depending on whether collides or not
        private void updateBallPosition()
        {
            //Check if game is started
            if (isStarted)
            {
                //Check winning condition
                if (_bricks.Count <= 0)
                {
                    winningCondition = true;
                    isStarted = false;

                }
                else
                {
                    //Get ball's position values
                    float xPos = ball.getX();
                    float yPos = ball.getY();

                    //Check collision against borders
                    if (xPos > GameCanvas.Width - ball.getWidth())
                    {
                        Canvas.SetLeft(ball.getBall(), GameCanvas.ActualWidth - ball.getWidth());
                        ball.setXVector(ball.getXVector() * -1);
                    }
                    if (xPos < 0)
                    {
                        Canvas.SetLeft(ball.getBall(), 0);
                        ball.setXVector(ball.getXVector() * -1);
                    }
                    if (yPos < 0)
                    {
                        Canvas.SetTop(ball.getBall(), 0);
                        ball.setYVector(ball.getYVector() * -1);
                    }
                    //Check bottom border
                    if (yPos > GameCanvas.Height - ball.getHeight())
                    {
                        gameOver();
                    }
                    //If ball is close enough to top border
                    if (ball.getY() < 150)
                    {
                        //Check collision againt every brick
                        foreach (Brick brick in _bricks)
                        {
                            //If collision occurs
                            if (brick.collides(ball.getHitBox()))
                            {
                                //Break brick
                                brick.Break();
                                //Determine effect
                                this.impactEffect(brick);
                                //Add Score
                                this.calculateScore();
                                //Remove if fully broken
                                if (brick.isBrickBroken())
                                {
                                    _bricks.Remove(brick);
                                    GameCanvas.Children.Remove(brick.getBrick());
                                }
                                //Determine ball direction depending of it hitting the side (left/right o top/bottom) 
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
                    //If ball is near the bottom enough calculate collision against paddle
                    if (ball.getY() > 200)
                    {
                        if (paddle.collides(ball.getHitBox()))
                        {
                            ball.setY(paddle.getY() - 11);
                            ball.setYVector(ball.getYVector() * -1);
                        }
                    }

                    //Set Ball new psoition
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
            //Updates paddle postion from acceleromter reading
            updatePaddlePosition(reading);
        }

        private void updatePaddlePosition(AccelerometerReading reading)
        {

            // use the AccelerationY: if < -0.04 move right
            //                        if >0.04 move left
            //Check if is started if not, the ball will move along the paddle
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
        //Updates paddle position using strings
        private void updatePaddlePosition(string direction)
        {
            //Check if is started if not, the ball will move along the paddle
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

            }
            else
            {
                if (direction == "left")
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
            //Check brink type and add buff based on it
            if (brick.GetType() == typeof(SpeedBrick))
            {
                this.addPowerUp("speed");
            }
            else if (brick.GetType() == typeof(SlowBrick))
            {
                this.addPowerUp("slow");
            }

        }
        //Takes a string and adds a buff based on it
        public void addPowerUp(String buff)
        {
            //Visual representation of the buff
            Ellipse buffSphere = new Ellipse();
            buffSphere.Height = 40;
            buffSphere.Width = 40;
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
                        buffSphere.Fill = new SolidColorBrush(Colors.Yellow);
                        this.addNewPowerUp(buffSphere, 120);
                    }
                    break;
                case "slow":
                    slowBuff++;
                    if (slowBuff == 1)
                    {
                        buffSphere.Name = "slowBuff";
                        buffSphere.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                        this.addNewPowerUp(buffSphere, 60);
                    }
                    break;
                default:
                    break;
            }


        }

        //When buff tapped
        private void BuffSphere_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Ellipse buff = (Ellipse)sender;
            //Check buff if speed increase ball's speed and reduce buff amount
            if (buff.Name.Equals("speedBuff"))
            {
                speedBuff--;
                if (speedBuff < 1)
                {
                    GameCanvas.Children.Remove(buff);
                }
                ball.speedUp();
            }
            //if slow reduce ball's speed and reduce buff amount
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
        //Add buff to canvas
        private void addNewPowerUp(Ellipse powerUp, int height)
        {

            Canvas.SetLeft(powerUp, (GameCanvas.ActualWidth - 60));
            Canvas.SetTop(powerUp, (GameCanvas.ActualHeight - height / 1.25));
            GameCanvas.Children.Add(powerUp);
        }
        //Calculates score based on ball's speed and display it in view.
        private void calculateScore()
        {
            int speed = (int)ball.getSpeed();
            int scorePoint = speed - 5;
            if (scorePoint >= 0)
            {
                MainPage.scoreController.addScore(100 + (50 * (scorePoint + 1)));
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
