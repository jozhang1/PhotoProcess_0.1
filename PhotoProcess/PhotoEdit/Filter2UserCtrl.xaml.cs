using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Nokia.Graphics.Imaging;
namespace PhotoProcess.PhotoEdit
{
    public partial class Filter2UserCtrl : UserControl
    {

        private Image imageRender;
        private StackPanel mainFuncuserCtrl;

        Stack<WriteableBitmap> processedBmpImageStack;
        private WriteableBitmap orginalBitmap;

        private byte[] imageData;

        ////////////////
        public List<IFilter> _components;
        ////////////////
        /// <summary>

        /// </summary>
        public Filter2UserCtrl()
        {
            InitializeComponent();
        }

        public void setImageGrid(StackPanel mainFuncuserCtrl, Image imageRender, byte[] imageData,
            Stack<WriteableBitmap> processedBmpImageStack, List<IFilter> components)
        {
            this.processedBmpImageStack = processedBmpImageStack;
            this.imageRender = imageRender;
            this.imageData = imageData;
            this.mainFuncuserCtrl = mainFuncuserCtrl;
            this._components = components;
        }


        private void buttonBackToMainFunc_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            mainFuncuserCtrl.Visibility = Visibility.Visible;
        }

        private void buttonBoost_Click(object sender, RoutedEventArgs e)
        {
            _components.Add(new ColorBoostFilter(3.0f));
            apply();
        }

        private void buttonTint_Click(object sender, RoutedEventArgs e)
        {
            _components.Add(new TemperatureAndTintFilter(0.8, -0.4));
            apply();
        }

        private void buttonLevel_Click(object sender, RoutedEventArgs e)
        {
            _components.Add(new LevelsFilter(0.7f, 0.2f, 0.4f));
            apply();
        }



        private void apply()
        {
            this.orginalBitmap = processedBmpImageStack.Peek();
            WriteableBitmap processedBmpImage = new WriteableBitmap(orginalBitmap.PixelWidth, orginalBitmap.PixelHeight);
            Filter filter = new Filter();
            filter.applyFilter(imageData, processedBmpImage, _components);
            imageRender.Source = processedBmpImage;
            processedBmpImageStack.Push(processedBmpImage);
        }


    }
}
