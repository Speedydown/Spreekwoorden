using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Spreekwoorden
{
    public sealed partial class PrivacyPolicy : SettingsFlyout
    {
        public PrivacyPolicy()
        {
            this.InitializeComponent();
            this.privacyPolicyHyperLink.NavigateUri = new Uri("http://wiezitwaarvandaag.nl/privacypolicy.aspx");
        }
    }
}
