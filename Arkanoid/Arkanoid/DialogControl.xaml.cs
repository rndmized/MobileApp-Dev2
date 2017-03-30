using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Template from - Reference: http://stackoverflow.com/questions/34538637/text-input-in-message-dialog-contentdialog

namespace Arkanoid
{

        public sealed partial class DialogControl : ContentDialog
        {
            public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
                "Text", typeof(string), typeof(DialogControl), new PropertyMetadata(default(string)));

            public DialogControl()
            {
                InitializeComponent();
            }

            public string Text
            {
                get { return (string)GetValue(TextProperty); }
                set { SetValue(TextProperty, value); }
            }

            private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
            {
                
            }

            private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
            {

            }
        }
    }
