using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using Microsoft.Phone;
using Microsoft.Phone.Shell;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Controls.Primitives;
using Microsoft.Xna.Framework.Media;


namespace PhotoProcess.AssistLib
{
    public class HelpPopup
    {
        Popup helpPopup;
        public HelpPopup()
        {
            helpPopup = new Popup();
        }

        public void Show( string helpText )
        {
            StackPanel panelHelp = new StackPanel();
            panelHelp.Background = new SolidColorBrush(Colors.Black);
            panelHelp.Width = 400;
            panelHelp.Height = 550;

            //Create a white border
            Border border = new Border();
            border.BorderBrush = new SolidColorBrush(Colors.White);
            border.BorderThickness = new Thickness(7.0);

            //Create a close button to exit popup
            Button close = new Button();
            close.Content = "Close";
            close.Margin = new Thickness(5.0);
            close.Click += new RoutedEventHandler(close_Click);

            //Create helper text
            TextBlock textblockHelp = new TextBlock();
            textblockHelp.FontSize = 24;
            textblockHelp.Foreground = new SolidColorBrush(Colors.White);
            textblockHelp.TextWrapping = TextWrapping.Wrap;
            textblockHelp.Text = helpText; 
            textblockHelp.Margin = new Thickness(5.0);

            //Add controls to stack panel
            panelHelp.Children.Add(textblockHelp);
            panelHelp.Children.Add(close);
            border.Child = panelHelp;

            // Set the Child property of Popup to the border 
            // that contains a stackpanel, textblock and button.
            helpPopup.Child = border;

            // Set where the popup will show up on the screen.   
            helpPopup.VerticalOffset = 150;
            helpPopup.HorizontalOffset = 40;

            // Open the popup.
            helpPopup.IsOpen = true;
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            helpPopup.IsOpen = false;
        }
    }
}
