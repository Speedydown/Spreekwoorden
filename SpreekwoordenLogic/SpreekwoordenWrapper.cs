using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace SpreekwoordenLogic
{
    public class SpreekwoordenWrapper : INotifyPropertyChanged
    {
        private const string FileName = "Wrapper.json";
        private static StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        public static SpreekwoordenWrapper instance = null;
        public static async Task<SpreekwoordenWrapper> GetInstance()
        {
            await Load();

            return instance;
        }

        public bool Searching = false;

        private bool _ChangeLockscreen;
        public bool ChangeLockscreen
        {
            get { return _ChangeLockscreen; }
            set
            {
                _ChangeLockscreen = value;
                Notify();
            }
        }

        private int _SelectedInterval = 1;
        public int SelectedInterval
        {
            get
            {
                return _SelectedInterval;
            }
            set
            {
                _SelectedInterval = value;
                RaisePropertyChanged("SelectedInterval");
            }
        }

        private int[] _IntervalArray = new int[] { 15, 30, 45, 60, 120 };
        [JsonIgnore]
        public int[] IntervalArray
        {
            get
            {
                return _IntervalArray;
            }
        }

        private bool _SourceIsRandom;
        public bool SourceIsRandom
        {
            get { return _SourceIsRandom; }
            set
            {
                _SourceIsRandom = value;
                Notify();
            }
        }

        private bool _SourceIsList;
        public bool SourceIsList
        {
            get { return _SourceIsList; }
            set
            {
                _SourceIsList = value;
                Notify();
            }
        }

        public ObservableCollection<Spreekwoord> SearchResult { get; set; }
        public ObservableCollection<Spreekwoord> MyItems { get; set; }
        public Visibility MySpreekwoordenTextVisibility
        {
            get
            {
                return MyItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private SpreekwoordenWrapper()
        {
            this.SearchResult = new ObservableCollection<Spreekwoord>();
            this.MyItems = new ObservableCollection<Spreekwoord>();
            this.ChangeLockscreen = true;
            this.SourceIsRandom = true;
            this.SourceIsList = false;
        }

        [JsonConstructor]
        public SpreekwoordenWrapper(
            bool SourceIsList,
            bool SourceIsRandom,
            bool ChangeLockscreen,
            ObservableCollection<Spreekwoord> SearchResult,
            ObservableCollection<Spreekwoord> MyItems
            )
        {
            this.ChangeLockscreen = ChangeLockscreen;
            this.SourceIsList = SourceIsList;
            this.SourceIsRandom = SourceIsRandom;
            this.SearchResult = SearchResult;
            this.MyItems = MyItems;
            this.Searching = false;
        }

        public async Task Search(String Query)
        {
            if (!this.Searching && Query.Length > 2)
            {
                Debug.WriteLine(Query);
                this.Searching = true;
                this.SearchResult.Clear();
                IList<Spreekwoord> Spreekwoorden = await Datahandler.GetSpreekwoordenBySearchQuery(Query);

                try
                {
                    foreach (Spreekwoord sw in Spreekwoorden)
                    {
                        this.SearchResult.Add(sw);
                    }
                }
                catch
                {

                }

                this.Searching = false;
            }
        }

        public async Task GetRandomWoorden()
        {
            this.SearchResult.Clear();
            IList<Spreekwoord> Spreekwoorden = await Datahandler.GetRandomSpreekwoorden();

            try
            {
                foreach (Spreekwoord sw in Spreekwoorden)
                {
                    this.SearchResult.Add(sw);
                }
            }
            catch
            {

            }

        }

        public void SpreekwoordFromYourItemsClick(Spreekwoord spreekwoord)
        {
            MyItems.Remove(spreekwoord);

            foreach (Spreekwoord sw in SearchResult)
            {
                if (sw.ID == spreekwoord.ID)
                {
                    MyItems.Remove(sw);
                    sw.Notify();
                    return;
                }
            }

            spreekwoord.Notify();
            Notify();
        }

        public void SpreekwoordFromSearchGridClick(Spreekwoord spreekwoord)
        {
            if (!spreekwoord.IsInList)
            {
                foreach (Spreekwoord sw in MyItems)
                {
                    if (sw.ID == spreekwoord.ID)
                    {
                        return;
                    }
                }

                MyItems.Add(spreekwoord);
                spreekwoord.Notify();
            }
            else
            {
                MyItems.Remove(spreekwoord);
                spreekwoord.Notify();

                foreach (Spreekwoord sw in MyItems)
                {
                    if (sw.ID == spreekwoord.ID)
                    {
                        MyItems.Remove(sw);
                        return;
                    }
                }
            }

            Notify();
        }

        public void Notify()
        {
            RaisePropertyChanged("MySpreekwoordenTextVisibility");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private static async Task Load()
        {
            if (instance == null)
            {
                try
                {
                    StorageFile sFile = await localFolder.GetFileAsync(FileName);
                    string FileAsText = await FileIO.ReadTextAsync(sFile);
                    instance = JsonConvert.DeserializeObject<SpreekwoordenWrapper>(FileAsText);
                    instance.Searching = false;
                }
                catch (Exception)
                {

                }

                if (instance == null)
                {
                    instance = new SpreekwoordenWrapper();
                }
            }
        }

        public async Task Save()
        {
            StorageFile file = await localFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);

            if (file != null)
            {
                string JsonString = JsonConvert.SerializeObject(instance);

                if (JsonString == string.Empty)
                {
                    Debug.WriteLine("Could not save!");

                    return;
                }

                await FileIO.WriteTextAsync(file, JsonString);
                Debug.WriteLine("Saved");
            }
        }

    }
}
