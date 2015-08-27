using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace SpreekwoordenLogic
{
    public static class Datahandler
    {
        public static async Task<IList<Spreekwoord>> GetSpreekwoordenBySearchQuery(string Query)
        {
            string Response = await HTTPGetUtil.GetDataAsStringFromURL("http://speedydown-001-site2.smarterasp.net/api.ashx?Spreekwoord=SearchSpreekwoord=" + Query);

            return JsonConvert.DeserializeObject<List<Spreekwoord>>(Response);
        }

        public static async Task<IList<Spreekwoord>> GetRandomSpreekwoorden(bool RenderImages = true)
        {
            string URL = "http://speedydown-001-site2.smarterasp.net/api.ashx?Spreekwoord=GetRandomSpreekwoorden=" + RenderImages;

            string Response = await HTTPGetUtil.GetDataAsStringFromURL(URL);

            return JsonConvert.DeserializeObject<List<Spreekwoord>>(Response);
        }

        public static async Task GetSpreekwoordenTile(int ID)
        {
            string Response = await HTTPGetUtil.GetDataAsStringFromURL("http://speedydown-001-site2.smarterasp.net/api.ashx?Spreekwoord=GenerateSpreekwoord=" + ID);

            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Tegeltje" + ID + ".jpg", CreationCollisionOption.ReplaceExisting);
            {
                string url = "http://speedydown-001-site2.smarterasp.net/Spreekwoorden/Images/" + ID + ".jpg";
                HttpClient client = new HttpClient();

                byte[] responseBytes = await client.GetByteArrayAsync(url);

                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {

                    using (var outputStream = stream.GetOutputStreamAt(0))
                    {
                        DataWriter writer = new DataWriter(outputStream);
                        writer.WriteBytes(responseBytes);
                        await writer.StoreAsync();
                        await outputStream.FlushAsync();
                    }
                }
            }
        }
    }
}
