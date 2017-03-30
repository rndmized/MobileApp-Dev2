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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Arkanoid
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserSelectionPage : Page
    {
        public UserSelectionPage()
        {
            this.InitializeComponent();
            this.Loaded += UserSelectionPage_Loaded;

        }

        private void UserSelectionPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.displayUsers();
            if(MainPage.user != null)
            {
                this.tblcurrentUser.Text = "Current profile: " + MainPage.user.username;
            }
            
            this.btnAddUser.Tapped += BtnAddUser_Tapped;
            this.btnStartGame.Tapped += BtnStartGame_Tapped;
        }

        private void BtnAddUser_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.showDialog();
        }


        private async void showDialog()
        {
            var dialog1 = new DialogControl();
            var result = await dialog1.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var text = dialog1.Text;
                MainPage.sqliteController.addUser(text);
                this.Frame.Navigate(typeof(UserSelectionPage));
            }

        }

        private void BtnStartGame_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (MainPage.user != null)
            {
                this.Frame.Navigate(typeof(GamePage));
            }

        }

        private void displayUsers()
        {
            List<User> users = MainPage.sqliteController.getUsers();

            foreach (var user in users)
            {
                StackPanel spUser = new StackPanel();

                TextBlock tblUsername = new TextBlock();
                tblUsername.Foreground = new SolidColorBrush(Colors.Green);
                tblUsername.Text = user.username;
                tblUsername.Name = user.id.ToString();
                tblUsername.Tapped += TblUsername_Tapped;

                TextBlock tblMaxScore = new TextBlock();
                tblMaxScore.Foreground = new SolidColorBrush(Colors.Green);
                tblMaxScore.Text = MainPage.sqliteController.getUserMaxScore(user.id).ToString();

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
                spUser.Children.Add(grdUsers);
                spUserListPanel.Children.Add(spUser);


            }
        }

        private void TblUsername_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TextBlock tbl = (TextBlock)sender;
            MainPage.user = new User(tbl.Text, int.Parse(tbl.Name));
            tblcurrentUser.Text = "Current Profile: " + tbl.Text;
           
        }
    }
}
