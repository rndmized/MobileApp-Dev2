using Arkanoid.Classes;
using Arkanoid.Classes.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace Arkanoid
{

    public sealed partial class UserSelectionPage : Page
    {
        public UserSelectionPage()
        {
            this.InitializeComponent();
            //Create loaded event listener
            this.Loaded += UserSelectionPage_Loaded;

        }

        private void UserSelectionPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Call Funtion to display users in database.
            this.displayUsers();
            //Check if a user is selected
            if(MainPage.user != null)
            {
                //If a userr profile were to be selecte didsplay it in the view
                this.tblcurrentUser.Text = "Current profile: " + MainPage.user.username;
            }
            //On loading Page create event listeners for buttons
            this.btnAddUser.Tapped += BtnAddUser_Tapped;
            this.btnStartGame.Tapped += BtnStartGame_Tapped;
        }

        private void BtnAddUser_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Display Dialog to add new Profile
            this.showDialog();
        }


        /*
        showDialog will display a dialog containing a text field and two buttons to add a user profile to the database.
         */
        private async void showDialog()
        {
            //Local variables declaration
            var dialog1 = new DialogControl();
            var result = await dialog1.ShowAsync();
            //If Accept Button Clicked..
            if (result == ContentDialogResult.Primary)
            {
                //Get Text from dialog
                var text = dialog1.Text;
                //Add new user to database
                MainPage.sqliteController.addUser(text);
                //Reload User Selection Page
                this.Frame.Navigate(typeof(UserSelectionPage));
            }//end if

        }//end of showDialog

        private void BtnStartGame_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Check whether a profile is selected or not
            if (MainPage.user != null)
            {
                //If it is Navigate to Game Page
                this.Frame.Navigate(typeof(GamePage));
            }

        }

        /*
        displayUsers will get a list of users from database and their max scores and will add them to the view. It will also create an event listener to select a profile on Tapped.
         */
        private void displayUsers()
        {
            //Get list of Users from database
            List<User> users = MainPage.sqliteController.getUsers();
            //For each user...
            foreach (var user in users)
            {
                //Create suitable container.
                StackPanel spUser = new StackPanel();
                //Create TextBlock for username and set attributes to match style
                TextBlock tblUsername = new TextBlock();
                tblUsername.Foreground = new SolidColorBrush(Colors.Green);
                //Set value to username and name to id
                tblUsername.Text = user.username;
                tblUsername.Name = user.id.ToString();
                //Add tapped event
                tblUsername.Tapped += TblUsername_Tapped;

                //Create TextBlock for max score and set attributes to match style
                TextBlock tblMaxScore = new TextBlock();
                tblMaxScore.Foreground = new SolidColorBrush(Colors.Green);
                //Set value to max score for that user from db
                tblMaxScore.Text = MainPage.sqliteController.getUserMaxScore(user.id).ToString();

                //Create Grid to contain username and score and add TextBlock to it.
                Grid grdUsers = new Grid();
                grdUsers.ColumnDefinitions.Insert(0, new ColumnDefinition());
                grdUsers.ColumnDefinitions.Insert(1, new ColumnDefinition());
                tblUsername.SetValue(Grid.ColumnProperty, 0);
                tblUsername.HorizontalAlignment = HorizontalAlignment.Left;
                tblMaxScore.SetValue(Grid.ColumnProperty, 1);
                tblMaxScore.HorizontalAlignment = HorizontalAlignment.Right;
                grdUsers.Children.Add(tblUsername);
                grdUsers.Children.Add(tblMaxScore);
                grdUsers.Margin = new Thickness(5);
                //Add Grid to Stack Panel
                spUser.Children.Add(grdUsers);
                //Add StackPanel to Parent StackPanel in view
                spUserListPanel.Children.Add(spUser);


            }//end foreach
        }//end displayUsers()

        private void TblUsername_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Cast sender to TextBlock
            TextBlock tbl = (TextBlock)sender;
            //Set User
            MainPage.user = new User(tbl.Text, int.Parse(tbl.Name));
            //Display user selected
            tblcurrentUser.Text = "Current Profile: " + tbl.Text;
           
        }
    }
}
