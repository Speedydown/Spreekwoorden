using SpreekwoordenLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WRCHelperLibrary;

namespace Spreekwoorden
{
    public sealed partial class MainPage : Page
    {
        private SpreekwoordenWrapper SpreekwoordInstance = null;
        private PivotItem MyItemsPivotItem = null;

        public MainPage()
        {
            this.InitializeComponent();
            Task Loaddata = LoadData();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            await SpreekwoordInstance.Save();

            base.OnNavigatingFrom(e);
        }

        public async Task LoadData()
        {
            SpreekwoordInstance = await SpreekwoordenWrapper.GetInstance();

            MyItemsPivotItem = MyItems;

            if (SpreekwoordInstance.MyItems.Count == 0)
            {
                SpreekwoordenPivot.Items.Remove(MyItems);
            }

            this.DataContext = SpreekwoordInstance;
            //LoadingControl.SetLoadingStatus(false);

            //LoadingControl.DisplayLoadingError(false);
            //LoadingControl.SetLoadingStatus(true);

            await SpreekwoordInstance.GetRandomWoorden();

            //LoadingControl.SetLoadingStatus(false);

            if (SpreekwoordInstance.ChangeLockscreen)
            {
                //NotificationHandler.Run("SpreekwoordenBackgroundTaskW.BackgroundTask", "ImageService", (uint)SpreekwoordInstance.IntervalArray[SpreekwoordInstance.SelectedInterval]);
            }

            int ID = await Task.Run(() => Datahandler.GetRandomSpreekwoordAndSaveImageToFile());
            //await LockScreen.SetImageFileAsync(await ApplicationData.Current.LocalFolder.GetFileAsync("Tegeltje" + ID + ".jpg"));
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            SpreekwoordInstance.SpreekwoordFromSearchGridClick(e.ClickedItem as Spreekwoord);

            if (SpreekwoordInstance.MyItems.Count == 0)
            {
                SpreekwoordenPivot.Items.Remove(MyItems);
            }
            else if (!SpreekwoordenPivot.Items.Contains(MyItems))
            {
                SpreekwoordenPivot.Items.Add(MyItems);
            }
        }

        
    }
}
