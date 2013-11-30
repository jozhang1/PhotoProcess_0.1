using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;


using PhotoProcess.PhotoEdit;
using PhotoProcess.AssistLib;

namespace PhotoProcess.PhotoEdit
{
    public partial class EditImageUserCtrl : UserControl
    {
        private Image imageRender;
        private StackPanel mainFuncuserCtrl;

        Stack<WriteableBitmap> processedBmpImageStack;
        private WriteableBitmap orginalBitmap;
            
        public EditImageUserCtrl()
        {
            InitializeComponent();
        }


        public void setImageGrid(StackPanel mainFuncuserCtrl, Image imageRender, 
            Stack<WriteableBitmap> processedBmpImageStack)
        {
            this.processedBmpImageStack = processedBmpImageStack;               
            this.imageRender = imageRender;
            this.mainFuncuserCtrl = mainFuncuserCtrl;
        }

        private void buttonRotate_Click(object sender, RoutedEventArgs e)
        {
            orginalBitmap = processedBmpImageStack.Peek(); 
            WriteableBitmap processedBitmap = EditImage.RotateBitmap(orginalBitmap);
            imageRender.Source = processedBitmap;
            processedBmpImageStack.Push( processedBitmap );
        }

        private void buttonGray_Click(object sender, RoutedEventArgs e)
        {
            orginalBitmap = processedBmpImageStack.Peek(); 
            WriteableBitmap processedBitmap = EditImage.ToGrayBitmap(orginalBitmap);
            imageRender.Source = processedBitmap;
            processedBmpImageStack.Push( processedBitmap );
        }

        private void buttonBackToMainFunc_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            mainFuncuserCtrl.Visibility = Visibility.Visible;
        }

    }
}
