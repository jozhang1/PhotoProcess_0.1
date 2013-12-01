using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Navigation;

using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

using Nokia.Graphics.Imaging;

using PhotoProcess.AssistLib;


namespace PhotoProcess.PhotoEdit
{
    public partial class PhotoEdit : PhoneApplicationPage
    {
        private WriteableBitmap selectedBmpImage;
        private WriteableBitmap processedBmpImage;
        private Stack<WriteableBitmap> processBmpImageStack;

        private ApplicationBarIconButton appBarBackButton;
        private ApplicationBarIconButton appBarSaveButton;
        private ApplicationBarIconButton appBarCancelButton;
        private ApplicationBarIconButton appBarClearAllButton;

        private byte[] imageData;

        private EditImageUserCtrl editImageUserCtrl;
        private FilterUserCtrl filterUserCtrl;
        private Filter2UserCtrl filter2UserCtrl;

        private List<IFilter> components;
        public PhotoEdit()
        {
            InitializeComponent();
            InitialUserCtrl();
            BuildLocalizedApplicationBar();
            processBmpImageStack = new Stack<WriteableBitmap>();
            components = new List<IFilter>();
        }

        private void InitialUserCtrl()
        {
            editImageUserCtrl = new EditImageUserCtrl();
            setUserCtrlInPanel(editImageUserCtrl);

            filterUserCtrl = new FilterUserCtrl();
            filter2UserCtrl = new Filter2UserCtrl();
            setUserCtrlInPanel(filterUserCtrl);
            setUserCtrlInPanel(filter2UserCtrl);
        }

        private void setUserCtrlInPanel(UserControl userCtrl)
        {
            Grid.SetRow(userCtrl, 2);
            ContentPanel.Children.Add(userCtrl);
            userCtrl.Visibility = Visibility.Collapsed;
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
                imageData = new byte[e.ChosenPhoto.Length];

                e.ChosenPhoto.Read(imageData, 0, imageData.Length);

                MemoryStream ms = new MemoryStream(imageData);
                this.selectedBmpIamge = PictureDecoder.DecodeJpeg(ms);
                this.imageChosenToEdit.Source = selectedBmpIamge;
                this.processBmpImageStack.Clear();
                this.processBmpImageStack.Push(selectedBmpIamge);
            }
        }

        private void buttonEditImage_Click(object sender, RoutedEventArgs e)
        {
            if (this.processBmpImageStack.Count == 0 )
            {
                MessageBox.Show("Please choose a piture first!");
            }
            else
            {
                this.clearComp();
                this.editImageUserCtrl.Visibility = Visibility.Visible;
                WriteableBitmap originalBmpImg = processBmpImageStack.Peek();
                this.processedBmpImage = new WriteableBitmap(originalBmpImg.PixelWidth, originalBmpImg.PixelHeight);
                this.editImageUserCtrl.setImageGrid(this.mainFuncCtrl, this.imageChosenToEdit, this.processBmpImageStack);
                enableSaveCancelAppBar();
            }
        }

        private void buttonFilters_Click(object sender, RoutedEventArgs e)
        {
            if (this.processBmpImageStack.Count == 0)
            {
                MessageBox.Show("Please choose a piture first!");
            }
            else
            {
                this.clearComp();
                this.filterUserCtrl.Visibility = Visibility.Visible;
                WriteableBitmap originalBmpImg = processBmpImageStack.Peek();
                this.processedBmpImage = new WriteableBitmap(originalBmpImg.PixelWidth, originalBmpImg.PixelHeight);
                this.filterUserCtrl.setImageGrid(this.mainFuncCtrl, this.imageChosenToEdit, this.imageData, this.processBmpImageStack,this.components);
                enableSaveCancelAppBar();
            }
        }

        private void buttonFilters2_Click(object sender, RoutedEventArgs e)
        {
            if (this.processBmpImageStack.Count == 0)
            {
                MessageBox.Show("Please choose a piture first!");
            }
            else
            {
                this.clearComp();
                this.filter2UserCtrl.Visibility = Visibility.Visible;
                WriteableBitmap originalBmpImg = processBmpImageStack.Peek();
                this.processedBmpImage = new WriteableBitmap(originalBmpImg.PixelWidth, originalBmpImg.PixelHeight);
                this.filter2UserCtrl.setImageGrid(this.mainFuncCtrl, this.imageChosenToEdit, this.imageData, this.processBmpImageStack, this.components);
                enableSaveCancelAppBar();
            }
        }
        private void buttonFrame_Click(object sender, RoutedEventArgs e)
        {
            if (this.imageChosenToEdit.Source == null)
            {
                MessageBox.Show("Please choose a piture first!");
            }
            else
            {
            }
        }

        private void enableSaveCancelAppBar()
        {
            appBarCancelButton.IsEnabled = true;
            appBarSaveButton.IsEnabled = true;
            appBarClearAllButton.IsEnabled = true;
        }

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            appBarBackButton = new ApplicationBarIconButton(
                    new Uri("/Assets/back.png", UriKind.Relative));
            appBarBackButton.Text = "Back To Main";
            appBarBackButton.Click += appBarBackButton_Click;
            ApplicationBar.Buttons.Add(appBarBackButton);

            appBarCancelButton = new ApplicationBarIconButton(
                    new Uri("/Assets/cancel.png", UriKind.Relative));
            appBarCancelButton.Text = "Cancel";
            appBarCancelButton.IsEnabled = false;
            appBarCancelButton.Click += appBarButtonCancel_Click;
            ApplicationBar.Buttons.Add(appBarCancelButton);

            appBarClearAllButton = new ApplicationBarIconButton(
                    new Uri("/Assets/refresh.png", UriKind.Relative));
            appBarClearAllButton.Text = "Clear All";
            appBarClearAllButton.IsEnabled = false;
            appBarClearAllButton.Click += appBarButtonClearAll_Click;
            ApplicationBar.Buttons.Add(appBarClearAllButton);

            appBarSaveButton = new ApplicationBarIconButton(
                    new Uri("/Assets/check.png", UriKind.Relative));
            appBarSaveButton.Text = "Accept";
            appBarSaveButton.IsEnabled = false;
            appBarSaveButton.Click += appBarSaveButton_Click;
            ApplicationBar.Buttons.Add(appBarSaveButton);
        }

        private void appBarButtonClearAll_Click(object sender, EventArgs e)
        {
            while (this.processBmpImageStack != null && this.processBmpImageStack.Count != 1)
            {
                this.processBmpImageStack.Pop();
            }
            if (this.processBmpImageStack != null) 
                this.imageChosenToEdit.Source = this.processBmpImageStack.Peek();
            components.Clear();
        }

        private void appBarSaveButton_Click(object sender, EventArgs e)
        {
            WriteableBitmap finalProcessedBmpImage = this.processBmpImageStack.Peek();
            SavePopup savePopup = new SavePopup(finalProcessedBmpImage);
            savePopup.Show();
        }

        private void appBarButtonCancel_Click(object sender, EventArgs e)
        {
            if (this.processBmpImageStack != null && this.processBmpImageStack.Count != 1)
            {
                this.processBmpImageStack.Pop();
            }
            if (this.processBmpImageStack != null) 
                this.imageChosenToEdit.Source = this.processBmpImageStack.Peek();
            if (components.Count != 0)
                components.RemoveAt(components.Count - 1);
        }
         
        private void appBarBackButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void clearComp()
        {
            this.mainFuncCtrl.Visibility = Visibility.Collapsed;
            this.editImageUserCtrl.Visibility = Visibility.Collapsed;
        }

       

    }

}
