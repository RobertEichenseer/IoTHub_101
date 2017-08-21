using Device.CommonBl.Interface;
using Device.ViewModel;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Device.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Cloud2Device : Page
    {
        Cloud2DeviceViewModel _viewModel = new Cloud2DeviceViewModel(); 

        public Cloud2Device()
        {
            this.InitializeComponent();
            grdMain.DataContext = _viewModel;
        }

        private void StartStopListeningForCommands_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.StartStopCheckForCloud2DeviceCommands(); 
        }

        private void StartStopListeningForMethods_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.StartStopListeningForMethods();
        }


        private void SendCloud2DeviceCommand_Click(object sender, RoutedEventArgs e)
        {
             _viewModel.SendCloud2DeviceCommand();
        }
        
        private async void ExecuteCloud2DeviceMethod_Click(object sender, RoutedEventArgs e)
        {
            string message = "Executing Cloud2Device Methods is currently not supported by the UWP part of the Device SDK!\n\n";
            message += "Please start console utillity <<Util_ExecuteCloud2DeviceMethod.exe>> in the .\\Utils folder of this package!"; 

            MessageDialog messageDialog = new MessageDialog(message, "Attention");
            await messageDialog.ShowAsync(); 

            //Not supported by UWP Device SDK
            //var c2D = new { DeviceId = deviceId, Severity = 0, Payload = "Payload" };
            //CloudToDeviceMethod c2DMethod = new CloudToDeviceMethod("ExecuteC2DMethod") { ResponseTimeout = TimeSpan.FromSeconds(15) };
            //c2DMethod.SetPayloadJson(JsonConvert.SerializeObject(c2D));

            //try
            //{
            //    CloudToDeviceMethodResult methodResult = await _serviceClient.InvokeDeviceMethodAsync(deviceId, c2DMethod);
            //}
            //catch (DeviceNotFoundException ex)
            //{
            //}
        }
    }
}
