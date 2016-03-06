using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SpreekwoordenLogic
{
    public class Spreekwoord : INotifyPropertyChanged
    {
        private const string Server = "http://win10apps.nl/API/Spreekwoorden/";

        public int ID { get; private set; }
        public string SpreekWoord { get; private set; }
        public string Betekenis { get; private set; }
        public string ImageURL
        {
            get
            {
                return Server + "GetwallPaper/" + ID;
            }
        }
        public string SmallImageURL
        {
            get
            {
                return "http://win10apps.nl/spreekwoorden/images/small/" + ID + ".jpg";
            }
        }

        public bool IsInList
        {
            get
            {
                if (SpreekwoordenWrapper.instance == null)
                {
                    return false;
                }

                foreach (Spreekwoord sw in SpreekwoordenWrapper.instance.MyItems)
                {
                    if (sw.ID == this.ID)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public int Width
        {
            get
            {
                try
                {
                    if (Window.Current.Bounds.Width > Window.Current.Bounds.Height)
                    {
                        return ((int)Window.Current.Bounds.Height / 2) - 10;
                    }
                    else
                    {
                        return ((int)Window.Current.Bounds.Width / 2) - 10;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        public Spreekwoord(int ID, string SpreekWoord, string Betekenis)
        {
            this.ID = ID;
            this.SpreekWoord = SpreekWoord;
            this.Betekenis = Betekenis;
        }

        public void Notify()
        {
            RaisePropertyChanged("IsInList");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
