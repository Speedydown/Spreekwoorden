﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreekwoordenLogic
{
    public class Spreekwoord : INotifyPropertyChanged
    {
        private const string Server = "http://speedydown-001-site2.smarterasp.net/Spreekwoorden/Images/";

        public int ID { get; private set; }
        public string SpreekWoord { get; private set; }
        public string Betekenis { get; private set; }
        public string ImageURL
        {
            get
            {
                return Server + ID + ".jpg";
            }
        }
        public string SmallImageURL
        {
            get
            {
                return Server + "Small/" + ID + ".jpg";
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