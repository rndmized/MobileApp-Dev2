using Arkanoid.Classes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Arkanoid
{
    public sealed partial class ScorePage : Page
    {
        public ScorePage()
        {
            this.InitializeComponent();
            //Create Load and tapped events
            this.Loaded += ScorePage_Loaded;
            this.btnMenu.Tapped += BtnMenu_Tapped;
        }

        private void BtnMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Navigate to Menu Page
            this.Frame.Navigate(typeof(MainPage));
        }

        /* On ScorePage loaded Get list of users from database, and then, for each user in database get its scores.
        Finally add user and score to view 
        */
        private void ScorePage_Loaded(object sender, RoutedEventArgs e)
        {
            //Getting Users from DB through sqliteController
            List<User> users = MainPage.sqliteController.getUsers();
            //For each user in the list...
            foreach (User user in users)
            {
                //Get score list for a given user from DB through sqliteController
                List<Score> scores = MainPage.sqliteController.getScoresByUser(user.id);
                //Call function to add the user and scores to view
                this.addUserScores(user,scores);
            }//end foreach
        }//end ScorePage_Loaded

        /* 
        Given a user and a list of scores, create a grid table containing user name and scores and add them to the view.
        */
        private void addUserScores(User user, List<Score> scores)
        {
            //Create new Grid and add Columns(2)
            Grid datatable = new Grid();
            datatable.ColumnDefinitions.Insert(0, new ColumnDefinition());
            datatable.ColumnDefinitions.Insert(1, new ColumnDefinition());
            //Create User TextBlock and set its attributes to match style
            TextBlock tblUser = new TextBlock();
            tblUser.Foreground = new SolidColorBrush(Colors.Green);
            tblUser.HorizontalAlignment = HorizontalAlignment.Left;
            //Get User username
            tblUser.Text = user.username;
            //Set TextBlock position in Grid
            tblUser.SetValue(Grid.ColumnProperty, 0);
            tblUser.SetValue(Grid.RowProperty, 0);
            //For each score
            for (int i = 0; i < scores.Count; i++)
            {
                //Add a new row to Table/Grid
                datatable.RowDefinitions.Insert(i, new RowDefinition());
                //Create Score TextBlock and set its attributes to match style
                TextBlock tblScore = new TextBlock();
                //Set TextBlock value to score of current element
                tblScore.Text = scores.ElementAt(i).score.ToString();
                tblScore.Foreground = new SolidColorBrush(Colors.Green);
                tblScore.Margin = new Thickness(100,0,0,0);
                tblScore.HorizontalAlignment = HorizontalAlignment.Right;
                //Set TextBlock position in Grid
                tblScore.SetValue(Grid.ColumnProperty, 1);
                tblScore.SetValue(Grid.RowProperty, i);
                //Add Score TextBlock to Grid
                datatable.Children.Add(tblScore);
            }//end for
            //Add User TextBlock to Grid
            datatable.Children.Add(tblUser);
            //Add Grid to Stack Panel
            spScoreBoard.Children.Add(datatable);
            
        }//end addUserScores() function
    }//end of ScorePage
}//end of Namespace
