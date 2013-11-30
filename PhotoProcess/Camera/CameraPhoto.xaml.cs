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

using Coding4Fun.Toolkit.Controls;

using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Phone;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Controls.Primitives;

using PhotoProcess.AssistLib;

namespace PhotoProcess.Camera
{
    public partial class CameraPhoto : PhoneApplicationPage
    {
        //This is a variable for the help popup.
        Popup help = new Popup();

        //The application bar buttons that are used.
        ApplicationBarIconButton buttonCamera;
        ApplicationBarIconButton buttonSave;
        ApplicationBarIconButton buttonHelp;
        ApplicationBarIconButton appBarBackButton;

        //The camera chooser used to capture a picture.
        CameraCaptureTask cameraTask;

        //Create a WritebleBitmap object to store captured photo
        private WriteableBitmap capturedImage;

        //Constructor
        public CameraPhoto()
        {
            InitializeComponent();

            SupportedOrientations = SupportedPageOrientation.Portrait | SupportedPageOrientation.Landscape;

            //Creates an application bar and then sets visibility and menu properties.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = true;

            //This code creates the application bar icon buttons.
            buttonCamera = new ApplicationBarIconButton(new Uri("/Assets/camera.png", UriKind.Relative));
            buttonSave = new ApplicationBarIconButton(new Uri("/Assets/save.png", UriKind.Relative));
            buttonHelp = new ApplicationBarIconButton(new Uri("/Assets/questionmark.png", UriKind.Relative));
            appBarBackButton = new ApplicationBarIconButton(new Uri("/Assets/back.png", UriKind.Relative));

            //Labels for the application bar buttons.
            buttonCamera.Text = "Camera";
            buttonSave.Text = "Save";
            buttonHelp.Text = "Help";

            //This code will create event handlers for buttons.
            buttonCamera.Click += new EventHandler(buttonCamera_Click);
            buttonHelp.Click += new EventHandler(buttonHelp_Click);
            buttonSave.Click += new EventHandler(buttonSave_Click);

            //This code adds buttons to application bar.
            ApplicationBar.Buttons.Add(buttonCamera);
            ApplicationBar.Buttons.Add(buttonSave);
            ApplicationBar.Buttons.Add(buttonHelp);
            
            appBarBackButton.Text = "Back To Main";
            appBarBackButton.Click += new EventHandler(BackToMainPage_Click);
            ApplicationBar.Buttons.Add(appBarBackButton);

            //Disable crop button until photo is taken.
            buttonSave.IsEnabled = false;

            //Create new instance of CameraCaptureClass
            cameraTask = new CameraCaptureTask();

            //Create new event handler for capturing a photo
            cameraTask.Completed += new EventHandler<PhotoResult>(cameraTask_Completed);
        }

        
        private void BackToMainPage_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Click event handler for the help button.
        ///This will create a popup/message box for help and add content to the popup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void buttonHelp_Click(object sender, EventArgs e)
        {
            HelpPopup help = new HelpPopup();
            string helpText = "Tap the camera button image on the application bar to take a photo." + " Once the photo is taken and returned to this page, tap the crop button on the application bar to crop the image.";
            help.Show(helpText);
        }

        /// <summary>
        /// Click event handler for the camera button.
        /// Opens the camera on the phone.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void buttonCamera_Click(object sender, EventArgs e)
        {
            textStatus.Text = "";

            //Show the camera.
            cameraTask.Show();

            //Set progress bar to visible to show time between user snapshot and decoding
            //of image into a writeable bitmap object.
            progressBar.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void buttonSave_Click(object sender, EventArgs e)
        {
            InputPrompt fileName = new InputPrompt();
            fileName.Title = "Photo Name";
            fileName.Message = "What should we call the Photo?";
            fileName.Completed += FileNameCompleted;
            fileName.Show();
        }

        private void FileNameCompleted(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            
            if (e.PopUpResult == PopUpResult.Ok)
            {
                // Get fileName from user input
                string fileName = e.Result;
                fileName = fileName.EndsWith(".jpg") ? fileName : fileName + ".jpg";
                PhotoHelper.SaveToPhotoLibrary(fileName, capturedImage);
            }
        }

        /// <summary>
        /// Event handler for retrieving the JPEG photo stream.
        /// Also to for decoding JPEG stream into a writeable bitmap and displaying.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cameraTask_Completed(object sender, PhotoResult e)
        {

            if (e.TaskResult == TaskResult.OK && e.ChosenPhoto != null)
            {

                //Take JPEG stream and decode into a WriteableBitmap object
                capturedImage = PictureDecoder.DecodeJpeg(e.ChosenPhoto);

                //Collapse visibility on the progress bar once writeable bitmap is visible.
                progressBar.Visibility = Visibility.Collapsed;


                //Populate image control with WriteableBitmap object.
                imageCamera.Source = capturedImage;

                //Once writeable bitmap has been rendered, the crop button
                //is enabled.
                buttonSave.IsEnabled = true;

                textStatus.Text = "Tap the save button to proceed";
            }
            else
            {
                textStatus.Text = "You decided not to take a picture.";
            }
        }
    }
}
