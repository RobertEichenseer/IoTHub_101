
using Device.CommonBl;
using Device.CommonBl.Cloud2Device;
using Device.CommonBl.Interface;
using Device.CommonBl.TelemetryIngest;
using Device.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Device.CommonBl.Events;

namespace Device.ViewModel
{
    internal class Cloud2DeviceViewModel : BaseViewModel
    {
        ICloud2Device _bl;
        DispatcherTimer _checkCloud2DeviceCommand;
        DispatcherTimer _checkCloud2DeviceCommandFeedback;


        public Cloud2DeviceViewModel()
        {
            _bl = new Cloud2Device();
            _bl.OnCloud2DeviceCommand += _bl_OnCloud2DeviceCommand;
            _bl.OnCloud2DeviceMethod += _bl_OnCloud2DeviceMethod;
            _bl.OnCloud2DeviceCommandFeedback += _bl_OnCloud2DeviceCommandFeedback;

            _checkCloud2DeviceCommand = new DispatcherTimer();
            _checkCloud2DeviceCommand.Tick += _checkCloud2DeviceCommand_Tick;
            _checkCloud2DeviceCommand.Interval = TimeSpan.FromSeconds(2);
            _checkCloud2DeviceCommand.Stop();

            _checkCloud2DeviceCommandFeedback = new DispatcherTimer();
            _checkCloud2DeviceCommandFeedback.Tick += _checkCloud2DeviceCommandFeedback_Tick;
            _checkCloud2DeviceCommandFeedback.Interval = TimeSpan.FromSeconds(2);
            _checkCloud2DeviceCommand.Stop();

            App.CoreDispatcher = Window.Current.CoreWindow.Dispatcher;
        }
        
        private string _cloudStatusDisplay = "";
        public string CloudStatusDisplay
        {
            get { return _cloudStatusDisplay; }
            set { _cloudStatusDisplay = $"{DateTime.Now.ToString()} - {value}"; OnPropertyChanged(); }
        }    

        private bool _isCheckingC2DCommandFeedback = false;
        public bool IsCheckingC2DCommandFeedback
        {
            get { return _isCheckingC2DCommandFeedback; }
            set
            {
                _isCheckingC2DCommandFeedback = value;
                if (value)
                { 
                    _checkCloud2DeviceCommandFeedback.Start();
                    CloudStatusDisplay = "Listening for C2D Command Feedback started!";
                }
                else
                { 
                    _checkCloud2DeviceCommandFeedback.Stop();
                    CloudStatusDisplay = "Listening for C2D Command Feedback stopped!"; 
                }
                OnPropertyChanged();
            }
        }

        private string _cloud2DeviceFeedback = "";
        public string Cloud2DeviceFeedback
        {
            get { return _cloud2DeviceFeedback;  }
            set { _cloud2DeviceFeedback = value; OnPropertyChanged(); }
        }

        private string _cloud2DeviceCommandContent = "{'TargetDeviceId':'DeviceRunningWindows10','C2DCommand':'RefreshIntervall','Severity':0}";
        public string Cloud2DeviceCommandContent
        {
            get { return _cloud2DeviceCommandContent; }
            set { _cloud2DeviceCommandContent = value; OnPropertyChanged(); }
        }

        private bool _isListeningForCommands = false;
        public bool IsListeningForCommands
        {
            get { return _isListeningForCommands; }
            set { _isListeningForCommands = value; OnPropertyChanged(); StartStopListeningForCommandsCaption = ""; }
        }

        private bool _isListeningForMethods = false;
        public bool IsListeningForMethods
        {
            get { return _isListeningForMethods; }
            set { _isListeningForMethods = value; OnPropertyChanged(); StartStopListeningForMethodsCaption = ""; }
        }

        public string StartStopListeningForCommandsCaption
        {
            get { return IsListeningForCommands ? "Stop Listening" : "Start Listening"; }
            set { OnPropertyChanged(); }
        }

        public string StartStopListeningForMethodsCaption
        {
            get { return IsListeningForMethods ? "Stop Listening" : "Start Listening"; }
            set { OnPropertyChanged(); }
        }

        private string _cloud2DeviceMethodReturnValue = @"//Rückgabewert der C2D Method {'ReturnCode': 200, 'Status' : '200', 'Detail': 'C2D Method Executed'}";
        public string Cloud2DeviceMethodReturnValue
        {
            get { return _cloud2DeviceMethodReturnValue;  }
            set
            {
                _cloud2DeviceMethodReturnValue = value;
                OnPropertyChanged();
                _bl.SetCloud2DeviceMethodReturnValue(value);
            }
        }

        private string _deviceIoTHubConnectionString = Configuration.DeviceIoTHubConnectionString; 
        public string DeviceIoTHubConnectionString
        {
            get { return _deviceIoTHubConnectionString; }
            set
            {
                _deviceIoTHubConnectionString = value;
                DeviceId = getDeviceId(value);
                OnPropertyChanged();
            }
        }

        private string _serviceIoTHubConnectionString = Configuration.ServiceIoTHubConnectionString;
        public string ServiceIoTHubConnectionString
        {
            get { return _serviceIoTHubConnectionString; }
            set
            {
                _serviceIoTHubConnectionString = value;
                OnPropertyChanged();
            }
        }

        private string _deviceId = Configuration.DeviceId;
        public string DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; OnPropertyChanged(); }
        }

        private string _deviceStatusDisplay = "";
        public string DeviceStatusDisplay
        {
            get { return _deviceStatusDisplay; }
            set { _deviceStatusDisplay = $"{DateTime.Now.ToString()} - {value}"; OnPropertyChanged(); }
        }

        private string getDeviceId(string connectionString)
        {
            string deviceSection = (connectionString.ToUpper().Split(';')).ToList<string>().Where(item => item.Contains("DEVICEID")).FirstOrDefault();
            if (!String.IsNullOrEmpty(deviceSection))
                if (deviceSection.IndexOf('=') < deviceSection.Length)
                    return deviceSection.Substring(deviceSection.IndexOf('=')+1);

            return ""; 
        }

        internal void StartStopCheckForCloud2DeviceCommands()
        {
            IsListeningForCommands = !IsListeningForCommands; 
            if (!IsListeningForCommands)
            {
                DeviceStatusDisplay = $"Check for C2D commands stopped";
                _checkCloud2DeviceCommand.Stop();
            }
            else
            {
                _checkCloud2DeviceCommand.Start();
                DeviceStatusDisplay = $"Check for C2D commands started";
            }
        }
        
        private async void _checkCloud2DeviceCommand_Tick(object sender, object e)
        {
            _checkCloud2DeviceCommand.Stop(); 
            await _bl.ListenForCommandsAsync(); 

            _checkCloud2DeviceCommand.Start();
        }

        private async void _checkCloud2DeviceCommandFeedback_Tick(object sender, object e)
        {
            _checkCloud2DeviceCommandFeedback.Stop();
            await _bl.ListenForCloud2DeviceCommandFeedback();

            _checkCloud2DeviceCommandFeedback.Start();
        }


        private async void _bl_OnCloud2DeviceCommand(object sender, CommonBl.Events.Cloud2DeviceCommandEventArgs e)
        {
            await App.CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal,() => {
                DeviceStatusDisplay = e.Payload; 
            });
        }

        internal async void StartStopListeningForMethods()
        {
            IsListeningForMethods = !IsListeningForMethods;
            DeviceStatusDisplay = IsListeningForMethods ? "Started Listening for Cloud To Device Methods" : "Stopped Listening for Cloud To Device Methods";
            await _bl.StartStopListeningForMethodsAsync(IsListeningForMethods);
        }
        private async void _bl_OnCloud2DeviceMethod(object sender, Cloud2DeviceMethodEventArgs e)
        {
            await App.CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                DeviceStatusDisplay = e.Payload;
            });
        }
        
        internal async void SendCloud2DeviceCommand()
        {
            await _bl.SendCloud2DeviceCommand(Configuration.DeviceId, Cloud2DeviceCommandContent); 
        }

        private async void _bl_OnCloud2DeviceCommandFeedback(object sender, Cloud2DeviceCommandFeedbackEventArgs e)
        {
            await App.CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                CloudStatusDisplay = e.Payload;
            });
        }
    }
}
