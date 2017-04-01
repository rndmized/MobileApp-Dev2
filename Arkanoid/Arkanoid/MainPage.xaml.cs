using Arkanoid.Classes;
using Arkanoid.Classes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace Arkanoid
{

    public sealed partial class MainPage : Page
    {

#region Variable Declaration

        //ScoreController Class controlls score
        public static ScoreController scoreController;

        //SQLiteController Class controlls database access
        public static SQLiteController sqliteController;

        //User Class contains information about user (id and user name)
        public static User user;

#endregion
        public MainPage()
        {
            
            this.InitializeComponent();
            //Assing instances to Variables
            scoreController = new ScoreController();
            sqliteController = new SQLiteController();
            //Create listeners for Menu Buttons
            this.btnStart.Tapped += BtnStart_Tapped;
            this.btnScores.Tapped += BtnScores_Tapped;
            this.btnExit.Tapped += BtnExit_Tapped;
        }

        private void BtnExit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Exit Current Application
            Application.Current.Exit();
        }

        private void BtnScores_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Navigate to Scores Page
            this.Frame.Navigate(typeof(ScorePage));
        }

        private void BtnStart_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Navigate to User Selection Page 
            Frame.Navigate(typeof(UserSelectionPage));
        }



    }
}
