using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.System.UserProfile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PhotoProcess.Flickr
{
    class FlickrImageHelper
    {
        private const string IconRoot = "Shared/ShellContent/";
        private const string BackgroundRoot = "Images/";
        public const string SelectedData = "LockScreenData.json";
        MediaLibrary photoLib = new MediaLibrary();

        public static void CleanStorage()
        {
            using (IsolatedStorageFile storageFolder =
                IsolatedStorageFile.GetUserStoreForApplication())
            {
                // can't call Delete Directory as some files may 
                // be in use for background or icons
                TryToDeleteAllFiles(storageFolder, BackgroundRoot);
                TryToDeleteAllFiles(storageFolder, IconRoot);
            }
        }

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

        public static async Task SaveFromIsolatedStorageToPhotoLib()
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

            List<FlickrImage> images
                = JsonConvert.DeserializeObject<List<FlickrImage>>(fileData);

            if (images != null)
            {
                foreach (FlickrImage image in images) 
                {
                    await SaveImage(image.Image1024);
                }
            }
        }

        public static async Task SaveImage(Uri uri)
        {
            string fileName = uri.Segments[uri.Segments.Length - 1];

            //string imageName = BackgroundRoot + fileName;
            //string iconName = IconRoot + fileName;

            MediaLibrary photoLib = new MediaLibrary();
            HttpClient client = new HttpClient();
            byte[] flickrResult = await client.GetByteArrayAsync(uri);
            
            photoLib.SavePicture(fileName, flickrResult);

            //using (IsolatedStorageFile storageFolder
            //    = IsolatedStorageFile.GetUserStoreForApplication())
            //{
            //    if (!storageFolder.DirectoryExists(BackgroundRoot))
            //        storageFolder.CreateDirectory(BackgroundRoot);

            //    if (!storageFolder.FileExists(imageName))
            //    {
            //        using (IsolatedStorageFileStream stream
            //            = storageFolder.CreateFile(imageName))
            //        {
            //            HttpClient client = new HttpClient();

            //            byte[] flickrResult = await client.GetByteArrayAsync(uri);
            //            photoLib.SavePicture("test.jpg", flickrResult);

            //            await stream.WriteAsync(flickrResult, 0, flickrResult.Length);
            //        }

            //        //storageFolder.CopyFile(imageName, iconName);
            //    }
            //}

            //await SetLockScreen(fileName);
        }
        

        private static async Task SetLockScreen(string fileName)
        {
            // This article describes how to programmatically 
            // take control of the lockscreen background:
            // http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj206968(v=vs.105).aspx

            // Right-click WMAppManifest.xml, select Open With ...
            // Choose 'XML (Text) Editor with Encoding'
            // Choose AutoDetect when asked about which Encoding

            // IMPORTANT: Add this under </Tokens> section:
            //
            /*
                <Extensions>
            		<Extension ExtensionName="LockScreen_Background" 
                     ConsumerID="{111DFF24-AA15-4A96-8006-2BFF8122084F}" 
                     TaskID="_default" />
            	</Extensions>
            */
            // That will make our app a Lock Screen Background "provider".

            // Lockscreen Design Guidelines
            // http://msdn.microsoft.com/en-us/library/windowsphone/design/jj662927(v=vs.105).aspx

            bool hasAccessForLockScreen
                = LockScreenManager.IsProvidedByCurrentApplication;

            if (!hasAccessForLockScreen)
            {
                // If you're not the provider, this call will prompt the user for permission.
                // Calling RequestAccessAsync from a background agent is not allowed.                    
                var accessRequested = await LockScreenManager.RequestAccessAsync();

                // Only do further work if the access was granted.
                hasAccessForLockScreen = (accessRequested == LockScreenRequestResult.Granted);
            }

            if (hasAccessForLockScreen)
            {
                Uri imgUri = new Uri(
                    "ms-appdata:///local/" + BackgroundRoot + fileName,
                    UriKind.Absolute);
                LockScreen.SetImageUri(imgUri);
            }

            var mainTile = ShellTile.ActiveTiles.FirstOrDefault();

            if (null != mainTile)
            {
                Uri iconUri = new Uri("isostore:///" + IconRoot + fileName, UriKind.Absolute);
                var imgs = new List<Uri>();
                imgs.Add(iconUri);

                CycleTileData tileData = new CycleTileData();
                tileData.CycleImages = imgs;
                //tileData.IconImage = imgUri;

                mainTile.Update(tileData);
            }
        }


    }
}
