using Microsoft.Toolkit.Uwp.UI.Controls;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Device
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void mainMenu_ItemClick(object sender, ItemClickEventArgs e)
        {
            string name = ((HamburgerMenuImageItem)e.ClickedItem).Name;

            switch (name.ToUpper())
            {
                case "TELEMETRYINGEST": { mainFrame.Navigate(typeof(View.TelemetryIngest)); break; }
                case "CLOUD2DEVICE": { mainFrame.Navigate(typeof(View.Cloud2Device)); break; }
                case "DEVICETWIN": { mainFrame.Navigate(typeof(View.DeviceTwin)); break; }
            }
        }
    }
}
