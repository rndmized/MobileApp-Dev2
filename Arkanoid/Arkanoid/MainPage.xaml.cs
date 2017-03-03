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
        public MainPage()
        {
            this.InitializeComponent();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            /*
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                //do something
                var statusBar = StatusBar.GetForCurrentView();
                await statusBar.HideAsync();
            }
            
            getScreenSize();
            */
            this.btnStart.Tapped += BtnStart_Tapped;
        }

        private void BtnStart_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(GamePage));
        }

        private void getScreenSize()
        {
            //Stack overflow http://stackoverflow.com/questions/31936154/get-screen-resolution-in-win10-uwp-app
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            tblScreenInfo.Text = (new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor)).ToString();
        }
    }
}
