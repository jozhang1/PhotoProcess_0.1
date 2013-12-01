using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Nokia.Graphics.Imaging;
using Windows.Foundation;


namespace PhotoProcess.PhotoEdit
{
    public partial class FilterUserCtrl : UserControl
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
        public FilterUserCtrl()
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

        private void buttonMagic_Click(object sender, RoutedEventArgs e)
        {
            _components.Add(new MagicPenFilter());
            apply();
        }

        private void buttonAntique_Click(object sender, RoutedEventArgs e)
        {
            _components.Add(new AntiqueFilter());
            apply();
        }

        private void buttonSketch_Click(object sender, RoutedEventArgs e)
        {
            _components.Add(new SketchFilter());
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
