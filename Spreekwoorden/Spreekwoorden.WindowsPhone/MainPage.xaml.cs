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
using Windows.System;
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
            LoadingControl.SetLoadingStatus(false);

            LoadingControl.DisplayLoadingError(false);
            LoadingControl.SetLoadingStatus(true);

            await SpreekwoordInstance.GetRandomWoorden();

            LoadingControl.SetLoadingStatus(false);

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

        private async void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await Search();
            }
        }

        private async Task Search()
        {
            SearchTextbox.IsEnabled = false;

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

            SearchTextbox.IsEnabled = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpreekwoordInstance.ChangeLockscreen)
            {
                //NotificationHandler.Run("SpreekwoordenBackgroundTaskW.BackgroundTask", "ImageService", (uint)SpreekwoordInstance.IntervalArray[SpreekwoordInstance.SelectedInterval]);
            }
        }

        private async void PrivacyButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://wiezitwaarvandaag.nl/privacypolicy.aspx"));
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SpreekwoordenPivot.SelectedItem != SeachPivot)
            {
                SpreekwoordenPivot.SelectedItem = SeachPivot;
                await Task.Delay(150);
                SearchTextbox.Focus(FocusState.Programmatic);
            }
            else
            {
                if (SearchTextbox.Text.Length > 0)
                {
                    await Search();
                }
                else
                {
                    await Task.Delay(50);
                    SearchTextbox.Focus(FocusState.Programmatic);
                }
            }
        }

        private async void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (SpreekwoordenPivot.SelectedItem != SeachPivot)
            {
                SpreekwoordenPivot.SelectedItem = SeachPivot;
            }

            LoadingControl.DisplayLoadingError(false);
            LoadingControl.SetLoadingStatus(true);

            await SpreekwoordInstance.GetRandomWoorden();

            LoadingControl.SetLoadingStatus(false);
        }
    }
}
