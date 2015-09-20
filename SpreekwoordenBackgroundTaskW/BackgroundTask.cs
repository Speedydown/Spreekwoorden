using SpreekwoordenLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.System.UserProfile;
using Windows.UI.Xaml.Media.Imaging;

namespace SpreekwoordenBackgroundTaskW
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            int ID = await Datahandler.GetRandomSpreekwoordAndSaveImageToFile();

            if (ID != 0)
            {
                await LockScreen.SetImageFileAsync(await ApplicationData.Current.LocalFolder.GetFileAsync("Tegeltje" + ID + ".jpg"));
            }

            deferral.Complete();
        }
    }
}
