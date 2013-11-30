using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace PhotoProcess.Flickr
{
    public class FlickrImage
    {
        public Uri Image320 { get; set; }
        public Uri Image1024 { get; set; }

        public async static Task<List<FlickrImage>> GetFlickrImages(
            string flickrApiKey,
            string topic,
            double latitude = double.NaN,
            double longitude = double.NaN,
            double radius = double.NaN
            )
        {
            HttpClient client = new HttpClient();

            string baseUrl = getBaseUrl(flickrApiKey, topic, latitude, longitude, radius);

            string flickrResult = await client.GetStringAsync(baseUrl);

            FlickrData apiData =
                JsonConvert.DeserializeObject<FlickrData>(flickrResult);

            List<FlickrImage> images = new List<FlickrImage>();

            if (apiData.stat == "ok")
            {
                foreach (Photo data in apiData.photos.photo)
                {
                    FlickrImage img = new FlickrImage();

                    // To retrieve one photo, use this format:
                    //http://farm{farm-id}.staticflickr.com/{server-id}/{id}_{secret}{size}.jpg

                    string photoUrl = "http://farm{0}.staticflickr.com/{1}/{2}_{3}";

                    string baseFlickrUrl = string.Format(photoUrl,
                        data.farm,
                        data.server,
                        data.id,
                        data.secret);

                    img.Image320 = new Uri(baseFlickrUrl + "_n.jpg");
                    img.Image1024 = new Uri(baseFlickrUrl + "_b.jpg");

                    images.Add(img);
                }
            }
            return images;
        }

        private static string getBaseUrl(string flickrApiKey,
            string topic,
            double latitude = double.NaN,
            double longitude = double.NaN,
            double radius = double.NaN)
        {
            // About licenses:
            // http://www.flickr.com/services/api/flickr.photos.licenses.getInfo.html
            /*
             *  <license id="4" name="Attribution License" url="http://creativecommons.org/licenses/by/2.0/" />
                <license id="5" name="Attribution-ShareAlike License" url="http://creativecommons.org/licenses/by-sa/2.0/" />
                <license id="6" name="Attribution-NoDerivs License" url="http://creativecommons.org/licenses/by-nd/2.0/" />
                <license id="7" name="No known copyright restrictions" url="http://flickr.com/commons/usage/" />
             */
            string[] licenses = { "4", "5", "6", "7" };
            string license = String.Join(",", licenses);
            license = license.Replace(",", "%2C");

            if (!double.IsNaN(latitude))
                latitude = Math.Round(latitude, 5);

            if (!double.IsNaN(longitude))
                longitude = Math.Round(longitude, 5);

            // Search API
            // http://www.flickr.com/services/api/flickr.photos.search.html


            string url = "http://api.flickr.com/services/rest/" +
                "?method=flickr.photos.search" +
                "&license={0}" +
                "&api_key={1}" +
                "&lat={2}" +
                "&lon={3}" +
                "&topic={4}"+
                "&radius={5}" +
                "&format=json" +
                "&nojsoncallback=1";

            var baseUrl = string.Format(url,
                license,
                flickrApiKey,
                latitude,
                longitude,
                topic,
                radius);
            return baseUrl;
        }
    }
}
