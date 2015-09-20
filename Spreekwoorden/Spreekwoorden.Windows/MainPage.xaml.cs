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
using Windows.Storage.Streams;
using Windows.System;
using Windows.System.UserProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WRCHelperLibrary;

namespace Spreekwoorden
{
    public sealed partial class MainPage : Page
    {
        private SpreekwoordenWrapper SpreekwoordInstance = null;

        public MainPage()
        {
            this.InitializeComponent();
            Task Loaddata = LoadData();
        }

        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            await SpreekwoordInstance.Save();

            base.OnNavigatingFrom(e);
        }

        public async Task LoadData()
        {
            SpreekwoordInstance = await SpreekwoordenWrapper.GetInstance();

            this.DataContext = SpreekwoordInstance;
            LoadingControl.SetLoadingStatus(false);

            LoadingControl.DisplayLoadingError(false);
            LoadingControl.SetLoadingStatus(true);

            await SpreekwoordInstance.GetRandomWoorden();

            LoadingControl.SetLoadingStatus(false);

            if (SpreekwoordInstance.ChangeLockscreen)
            {
                NotificationHandler.Run("SpreekwoordenBackgroundTaskW.BackgroundTask", "ImageService", (uint)SpreekwoordInstance.IntervalArray[SpreekwoordInstance.SelectedInterval]);
            }

            int ID = await Task.Run(() => Datahandler.GetRandomSpreekwoordAndSaveImageToFile());
            await LockScreen.SetImageFileAsync(await ApplicationData.Current.LocalFolder.GetFileAsync("Tegeltje" + ID + ".jpg"));
        }

        private async void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                await Search();
            }
        }

        public async Task Search()
        {
            LoadingControl.DisplayLoadingError(false);
            LoadingControl.SetLoadingStatus(true);

            await SpreekwoordInstance.Search(SearchTextbox.Text);

            if (!SpreekwoordInstance.Searching)
            {
                if (SpreekwoordInstance.SearchResult.Count == 0)
                {
                    LoadingControl.DisplayLoadingError(true);
                }

                LoadingControl.SetLoadingStatus(false);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Search();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            SpreekwoordInstance.SpreekwoordFromSearchGridClick(e.ClickedItem as Spreekwoord);
        }

        private void GridviewYourItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            SpreekwoordInstance.SpreekwoordFromYourItemsClick(e.ClickedItem as Spreekwoord);
        }

        private async void RandomSpreekwoordenButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingControl.DisplayLoadingError(false);
            LoadingControl.SetLoadingStatus(true);

            await SpreekwoordInstance.GetRandomWoorden();

            LoadingControl.SetLoadingStatus(false);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpreekwoordInstance.ChangeLockscreen)
            {
                NotificationHandler.Run("SpreekwoordenBackgroundTaskW.BackgroundTask", "ImageService", (uint)SpreekwoordInstance.IntervalArray[SpreekwoordInstance.SelectedInterval]);
            }
        }
    }
}
