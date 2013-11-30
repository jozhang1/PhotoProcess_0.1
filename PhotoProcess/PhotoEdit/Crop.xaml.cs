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

using PhotoProcess.AssistLib;

namespace PhotoProcess.PhotoEdit
{
    public partial class Crop : PhoneApplicationPage
    {

        //Variables for the application bar buttons
        ApplicationBarIconButton buttonCrop;
        ApplicationBarIconButton buttonAccept;
        ApplicationBarIconButton buttonReject;
        ApplicationBarIconButton buttonHelp;

        private Random rand;

        //Variables for the crop feature
        Point p1, p2;
        bool cropping = false;

        public WriteableBitmap CroppedImage 
        { 
            get{ return this.croppedImage; }
            set { this.croppedImage = value; } 
        }

        // Image after being cropped
        private WriteableBitmap croppedImage;
        // Originally captured Image
        private WriteableBitmap selcectedImage;

        public Crop(WriteableBitmap selcectedImage)
        {
            InitializeComponent();
            this.selcectedImage = selcectedImage;

            rand = new Random((int)DateTime.Now.Ticks);

            textStatus.Text = "Select the cropping region with your finger." + " Once completed, tap the crop button to crop the image.";

            //Sets the source to the Image control on the crop page to the WriteableBitmap object created previously.
            DisplayedImageElement.Source = selcectedImage;

            //Create event handlers for cropping selection on the picture.
            DisplayedImageElement.MouseLeftButtonDown += new MouseButtonEventHandler(CropImage_MouseLeftButtonDown);
            DisplayedImageElement.MouseLeftButtonUp += new MouseButtonEventHandler(CropImage_MouseLeftButtonUp);
            DisplayedImageElement.MouseMove += new MouseEventHandler(CropImage_MouseMove);

            //Used for rendering the cropping rectangle on the image.
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);

            //Creating an application bar and then setting visibility and menu properties.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            //This code creates the application bar icon buttons.
            buttonCrop = new ApplicationBarIconButton(new Uri("/Assets/edit.png", UriKind.Relative));
            buttonAccept = new ApplicationBarIconButton(new Uri("/Assets/check.rest.png", UriKind.Relative));
            buttonReject = new ApplicationBarIconButton(new Uri("/Assets/cancel.rest.png", UriKind.Relative));
            buttonHelp = new ApplicationBarIconButton(new Uri("/Assets/questionmark.png", UriKind.Relative));

            //Labels for the application bar buttons.
            buttonCrop.Text = "Crop";
            buttonAccept.Text = "Accept";
            buttonReject.Text = "Reject";
            buttonHelp.Text = "Help";

            //This code adds buttons to application bar.
            ApplicationBar.Buttons.Add(buttonCrop);
            ApplicationBar.Buttons.Add(buttonAccept);
            ApplicationBar.Buttons.Add(buttonReject);
            ApplicationBar.Buttons.Add(buttonHelp);

            //This code will create event handlers for buttons.
            buttonCrop.Click += new EventHandler(buttonCrop_Click);
            buttonAccept.Click += new EventHandler(buttonAccept_Click);
            buttonReject.Click += new EventHandler(buttonReject_Click);
            buttonHelp.Click += new EventHandler(buttonHelp_Click);

            //Disable buttons so user cannot click until appropriate time.
            buttonCrop.IsEnabled = false;
            buttonAccept.IsEnabled = false;
            buttonReject.IsEnabled = false;

            //Begin storyboard for rectangle color effect.
            Rectangle.Begin();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

        }


        private void buttonHelp_Click(object sender, EventArgs e)
        {
            HelpPopup help = new HelpPopup();
            string helpText = "Use your finger on the image to define a cropping region." + 
                " Once the region is selected, as seen with a rectangle, tap the crop button to crop the image." + 
                " You may choose to save this image in the media library by tapping the check button on the application bar, or reject the cropping and return to the original image with the cancel button (X).";
            help.Show(helpText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (cropping)
            {
                rectCropRange.SetValue(Canvas.LeftProperty, (p1.X < p2.X) ? p1.X : p2.X);
                rectCropRange.SetValue(Canvas.TopProperty, (p1.Y < p2.Y) ? p1.Y : p2.Y);
                rectCropRange.Width = (int)Math.Abs(p2.X - p1.X);
                rectCropRange.Height = (int)Math.Abs(p2.Y - p1.Y);
            }
        }


        /// <summary>
        /// Click event handler for mouse move.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CropImage_MouseMove(object sender, MouseEventArgs e)
        {
            p2 = e.GetPosition(DisplayedImageElement);
        }

        /// <summary>
        /// Click event handler for mouse left button up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CropImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            p2 = e.GetPosition(DisplayedImageElement);
            cropping = false;
        }

        /// <summary>
        /// Click event handler for mouse left button down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void CropImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            buttonCrop.IsEnabled = true;
            p1 = e.GetPosition(DisplayedImageElement);
            p2 = p1;
            rectCropRange.Visibility = Visibility.Visible;
            cropping = true;
        }

        /// <summary>
        /// Click event handler for the reject button on the application bar.
        /// This will allow you to reject the cropped image and set back to the original image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void buttonReject_Click(object sender, EventArgs e)
        {
            //Sets the cropped image back to the original image. For users that want to revert changes.
            DisplayedImageElement.Source = selcectedImage;

            //Buttons are disabled and user cannot proceed to use the below until they crop an image again.
            buttonCrop.IsEnabled = false;
            buttonAccept.IsEnabled = false;
            buttonReject.IsEnabled = false;

            //Instructional Text
            textStatus.Text = "Select the cropping region with your finger." + " Once completed, tap the crop button to crop the image.";
        }

        /// <summary>
        /// Click event handler for the accept button on the application bar.
        /// Saves cropped image to isolated storage, then into
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            //Make progress bar visible for the event handler as there may be posible latency.
            progressBar.Visibility = Visibility.Visible;

            //Create filename for JPEG in isolated storage
            string croppedImageName = "croppedImage" + rand.Next(0,100000) +".jpg";

            //Create virtual store and file stream. Check for duplicate tempJPEG files.
            PhotoHelper.SaveToPhotoLibrary(croppedImageName, croppedImage);

            progressBar.Visibility = Visibility.Collapsed;
            textStatus.Text = "Picture saved to photos library on the device.";
        }

        /// <summary>
        /// Click event handler for the crop button on the application bar.
        /// This code creates the new cropped writeable bitmap object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCrop_Click(object sender, EventArgs e)
        {
            // Get the size of the source image captured by the camera
            double originalImageWidth = selcectedImage.PixelWidth;
            double originalImageHeight = selcectedImage.PixelHeight;


            // Get the size of the image when it is displayed on the phone
            double displayedWidth = DisplayedImageElement.ActualWidth;
            double displayedHeight = DisplayedImageElement.ActualHeight;

            // Calculate the ratio of the original image to the displayed image
            double widthRatio = originalImageWidth / displayedWidth;
            double heightRatio = originalImageHeight / displayedHeight;

            // Create a new WriteableBitmap. The size of the bitmap is the size of the cropping rectangle
            // drawn by the user, multiplied by the image size ratio.
            croppedImage = new WriteableBitmap((int)(widthRatio * Math.Abs(p2.X - p1.X)), (int)(heightRatio * Math.Abs(p2.Y - p1.Y)));


            // Calculate the offset of the cropped image. This is the distance, in pixels, to the top left corner
            // of the cropping rectangle, multiplied by the image size ratio.
            int xoffset = (int)(((p1.X < p2.X) ? p1.X : p2.X) * widthRatio);
            int yoffset = (int)(((p1.Y < p2.Y) ? p1.Y : p2.X) * heightRatio);

            // Copy the pixels from the targeted region of the source image into the target image, 
            // using the calculated offset
            for (int i = 0; i < CroppedImage.Pixels.Length; i++)
            {
                int x = (int)((i % CroppedImage.PixelWidth) + xoffset);
                int y = (int)((i / CroppedImage.PixelWidth) + yoffset);
                croppedImage.Pixels[i] = selcectedImage.Pixels[y * selcectedImage.PixelWidth + x];
            }

            // Set the source of the image control to the new cropped bitmap
            DisplayedImageElement.Source = croppedImage;
            rectCropRange.Visibility = Visibility.Collapsed;


            //Enable  accept and reject buttons to save or discard current cropped image.
            //Disable crop button until a new cropping region is selected.
            buttonAccept.IsEnabled = true;
            buttonReject.IsEnabled = true;
            buttonCrop.IsEnabled = false;

            //Instructional text
            textStatus.Text = "Continue to crop image, accept, or reject.";
        }
    }
}
