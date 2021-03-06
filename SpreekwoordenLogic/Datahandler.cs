﻿using Newtonsoft.Json;
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
        private static Random Randomizer = new Random();

        public static async Task<IList<Spreekwoord>> GetSpreekwoordenBySearchQuery(string Query)
        {
            string Response = await HTTPGetUtil.GetDataAsStringFromURL("http://win10apps.nl/API/Spreekwoorden/GetSpreekwoorden/" + Query);

            try
            {
                return JsonConvert.DeserializeObject<List<Spreekwoord>>(Response);
            }
            catch
            {
                return new List<Spreekwoord>();
            }
        }

        public static async Task<IList<Spreekwoord>> GetRandomSpreekwoorden()
        {
            string URL = "http://win10apps.nl/API/Spreekwoorden/GetSpreekwoorden/?random=" + Randomizer.Next(0, 1000000);

            string Response = await HTTPGetUtil.GetDataAsStringFromURL(URL);

            try
            {
                return JsonConvert.DeserializeObject<List<Spreekwoord>>(Response);
            }
            catch
            {
                return new List<Spreekwoord>();
            }
        }

        private static async Task GetSpreekwoordenTile(int ID)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Tegeltje.jpg", CreationCollisionOption.ReplaceExisting);
            {
                string url = "http://win10apps.nl/API/Spreekwoorden/GetwallPaper/" + ID;
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

        public static async Task<StorageFile> GetSmallSpreekwoordenTile(int ID)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Tegeltje" + ID + ".jpg", CreationCollisionOption.ReplaceExisting);
            {
                string url = "http://win10apps.nl/API/Spreekwoorden/GetSmallSpreekwoordenTile/" + ID;
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

            return file;
        }

        public static async Task<int> GetRandomSpreekwoordAndSaveImageToFile()
        {
            SpreekwoordenWrapper spreekwoordInstance = await SpreekwoordenWrapper.GetInstance();

            List<Spreekwoord> spreekwoorden = new List<Spreekwoord>();

            if (!spreekwoordInstance.ChangeLockscreen)
            {
                return 0;
            }

            if (spreekwoordInstance.SourceIsRandom)
            {
                spreekwoorden.AddRange(await Datahandler.GetRandomSpreekwoorden());
            }

            if (spreekwoordInstance.SourceIsList)
            {
                spreekwoorden.AddRange(spreekwoordInstance.MyItems);
            }

            if (spreekwoorden.Count == 0)
            {
                return 0;
            }

            Random random = new Random();
            Spreekwoord s = spreekwoorden[random.Next(0, spreekwoorden.Count)];

            await GetSpreekwoordenTile(s.ID);

            return s.ID;
        }
    }
}
