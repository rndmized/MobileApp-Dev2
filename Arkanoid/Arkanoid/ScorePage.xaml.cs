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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScorePage : Page
    {
        public ScorePage()
        {
            this.InitializeComponent();
            this.Loaded += ScorePage_Loaded;
            this.btnMenu.Tapped += BtnMenu_Tapped;
        }

        private void BtnMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void ScorePage_Loaded(object sender, RoutedEventArgs e)
        {
            List<User> users = MainPage.sqliteController.getUsers();
            foreach (User user in users)
            {
                List<Score> scores = MainPage.sqliteController.getScoresByUser(user.id);
                this.addUserScores(user,scores);
            }
        }

        private void addUserScores(User user, List<Score> scores)
        {
            Grid datatable = new Grid();
            datatable.ColumnDefinitions.Insert(0, new ColumnDefinition());
            datatable.ColumnDefinitions.Insert(1, new ColumnDefinition());
            TextBlock tblUser = new TextBlock();
            tblUser.Foreground = new SolidColorBrush(Colors.Green);
            tblUser.HorizontalAlignment = HorizontalAlignment.Left;
            tblUser.Text = user.username;
            tblUser.SetValue(Grid.ColumnProperty, 0);
            tblUser.SetValue(Grid.RowProperty, 0);
            for (int i = 0; i < scores.Count; i++)
            {
                datatable.RowDefinitions.Insert(i, new RowDefinition());
                TextBlock tblScore = new TextBlock();
                tblScore.Text = scores.ElementAt(i).score.ToString();
                tblScore.Foreground = new SolidColorBrush(Colors.Green);
                tblScore.Margin = new Thickness(100,0,0,0);
                tblScore.HorizontalAlignment = HorizontalAlignment.Right;
                tblScore.SetValue(Grid.ColumnProperty, 1);
                tblScore.SetValue(Grid.RowProperty, i);
                datatable.Children.Add(tblScore);
            }
            
            datatable.Children.Add(tblUser);
            spScoreBoard.Children.Add(datatable);
            
        }
    }
}
