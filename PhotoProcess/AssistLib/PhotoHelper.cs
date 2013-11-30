using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.IO;
using System.IO.IsolatedStorage;

using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using Coding4Fun.Toolkit.Controls;

using PhotoProcess.Flickr;

namespace PhotoProcess.AssistLib
{
    public class PhotoHelper
    {
        private const string BackgroundRoot = "Images/";
        private const string SelectedData = "FlickrImageData.json";
        private MediaLibrary photoLib = new MediaLibrary();
        
        /// <summary>
        /// Save to Photo library
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bitmap"></param>
        public static void SaveToPhotoLibrary(string fileName, WriteableBitmap bitmap)
        {
            //Create virtual store and file stream. Check for duplicate tempJPEG files.
            var isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (isoStore.FileExists(fileName))
            {
                isoStore.DeleteFile(fileName);
            }
            IsolatedStorageFileStream fileStream = isoStore.CreateFile(fileName);

            //Encode the WriteableBitmap into JPEG stream and place into isolated storage.
            Extensions.SaveJpeg(bitmap, fileStream, bitmap.PixelWidth, bitmap.PixelHeight, 0, 85);
            fileStream.Close();

            //Create a new file stream.
            fileStream = isoStore.OpenFile(fileName, FileMode.Open, FileAccess.Read);

            //Add the JPEG file to the photos library on the device.
            MediaLibrary library = new MediaLibrary();
            string photoName = formatImageName(fileName,library);
            Picture pic = library.SavePicture(photoName, fileStream);
            fileStream.Close();
        }

        private static string formatImageName(string originalImageName, MediaLibrary library)
        {
            string formatedName = originalImageName.EndsWith(".jpg") ? originalImageName : originalImageName + ".jpg";
            return formatedName;
        }

        
        /// <summary>
        /// clear the Isolated Storage
        /// </summary>
        public static void CleanStorage()
        {
            using (IsolatedStorageFile storageFolder =
                IsolatedStorageFile.GetUserStoreForApplication())
            {
                // can't call Delete Directory as some files may 
                // be in use for background or icons
                TryToDeleteAllFiles(storageFolder, BackgroundRoot);
            }
        }

        /// <summary>
        /// delete files in directory
        /// </summary>
        /// <param name="storageFolder"></param>
        /// <param name="directory"></param>
        private static void TryToDeleteAllFiles(IsolatedStorageFile storageFolder, string directory)
        {
            if (storageFolder.DirectoryExists(directory))
            {
                try
                {
                    string[] files = storageFolder.GetFileNames(directory);

                    foreach (string file in files)
                    {
                        storageFolder.DeleteFile(directory + file);
                    }
                }
                catch (Exception)
                {
                    // could be in use
                }
            }
        }


        /// <summary>
        /// save FlickrImage to phtot library 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task SaveToPhotoLibrary(List<FlickrImage> data)
        {
            // save to IsolatedStorage
            SaveToIsolatedStorage(data);
            // save from IsolatedStorage to PhotoLibrary
            await SaveFromIsolatedStorageToPhotoLib();
        }


        public static void SaveToIsolatedStorage(List<FlickrImage> data)
        {
            var stringData = JsonConvert.SerializeObject(data);

            using (var storageFolder = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = storageFolder.CreateFile(SelectedData))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(stringData);
                    }
                }
            }
        }

        private static async Task SaveFromIsolatedStorageToPhotoLib()
        {
            string fileData;

            using (IsolatedStorageFile storageFolder
                = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!storageFolder.FileExists(SelectedData))
                    return;

                using (IsolatedStorageFileStream stream
                    = storageFolder.OpenFile(SelectedData, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        fileData = reader.ReadToEnd();
                    }
                }
            }

            List<FlickrImage> images = JsonConvert.DeserializeObject<List<FlickrImage>>(fileData);

            if (images != null)
            {
                foreach (FlickrImage image in images)
                {
                    await downloadFlickrImage(image.Image1024);
                }
            }
        }

        private static async Task downloadFlickrImage(Uri uri)
        {
            string fileName = uri.Segments[uri.Segments.Length - 1];

            MediaLibrary photoLib = new MediaLibrary();
            HttpClient client = new HttpClient();
            byte[] flickrResult = await client.GetByteArrayAsync(uri);

            photoLib.SavePicture(fileName, flickrResult);
        }

        public static string GenFileName()
        {
            string fileName = DateTime.Now.ToString() + ".jpg";
            return fileName;
        }


    }
}
