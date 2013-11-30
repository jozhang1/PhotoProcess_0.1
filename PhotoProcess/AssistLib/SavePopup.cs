using System;
using System.Windows.Media.Imaging;

using Coding4Fun.Toolkit.Controls;

namespace PhotoProcess.AssistLib
{
    public class SavePopup
    {
        private WriteableBitmap toSaveBitmap;

        public SavePopup(WriteableBitmap toSaveBitmap)
        {
            this.toSaveBitmap = toSaveBitmap;
        }


        public void Show()
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
                PhotoHelper.SaveToPhotoLibrary(fileName, this.toSaveBitmap);
            }
        }
    }
}
