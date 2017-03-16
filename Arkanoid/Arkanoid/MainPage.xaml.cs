using Arkanoid.Classes;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Arkanoid
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static ScoreController scoreController;

        public MainPage()
        {
            
            this.InitializeComponent();
            scoreController = new ScoreController();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            this.btnStart.Tapped += BtnStart_Tapped;
        }

        private void BtnStart_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(GamePage));
        }


    }
}
