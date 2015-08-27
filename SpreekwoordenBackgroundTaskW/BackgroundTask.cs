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
            
            SpreekwoordenWrapper spreekwoordInstance = await SpreekwoordenWrapper.GetInstance();

            List<Spreekwoord> spreekwoorden = new List<Spreekwoord>();

            if (spreekwoordInstance.SourceIsRandom)
            {
                spreekwoorden.AddRange(await Datahandler.GetRandomSpreekwoorden(false));
            }

            if (spreekwoordInstance.SourceIsList)
            {
                spreekwoorden.AddRange(spreekwoordInstance.MyItems);
            }

            if (spreekwoorden.Count == 0)
            {
                return;
            }

            Random random = new Random();
            Spreekwoord s = spreekwoorden[random.Next(0, spreekwoorden.Count)];

            await Datahandler.GetSpreekwoordenTile(s.ID);
            await LockScreen.SetImageFileAsync(await ApplicationData.Current.LocalFolder.GetFileAsync("Tegeltje" +  s.ID + ".jpg"));

            deferral.Complete();
        }
    }
}
