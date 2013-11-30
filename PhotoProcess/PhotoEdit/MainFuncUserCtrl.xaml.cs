using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace PhotoProcess.PhotoEdit
{
    public partial class MainFuncUserCtrl : UserControl
    {
        private BitmapImage selectedBmpIamge;
        private Image imageDisplayedCtrl;
        private Grid grid;
        
        public MainFuncUserCtrl(Grid grid, Image imageDisplayedCtrl )
        {
            InitializeComponent();
            this.grid = grid;
            this.imageDisplayedCtrl = imageDisplayedCtrl;
        }

        private void buttonChooseImage_Click(object sender, RoutedEventArgs e)
        {
            photoChoose();
        }

        private void photoChoose()
        {
            PhotoChooserTask photoChooseTask = new PhotoChooserTask();
            photoChooseTask.Completed += new EventHandler<PhotoResult>(photoChooseTask_Completed);
            photoChooseTask.Show();
        }

        private void photoChooseTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                this.selectedBmpIamge = new BitmapImage();
                this.selectedBmpIamge.SetSource(e.ChosenPhoto);
                this.imageDisplayedCtrl.Source = selectedBmpIamge;
                this.enableButton();
            }
        }

        private void enableButton()
        {
            this.buttonFilters.IsEnabled = true;
            this.buttonFrame.IsEnabled = true;
            this.buttonEditImage.IsEnabled = true;
            //this.buttonCancel.IsEnabled = true;
        }



        private void buttonEditImage_Click(object sender, RoutedEventArgs e)
        {
        }

        private void buttonFilters_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonFrame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void imageChosenToEdit_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

    }
}
