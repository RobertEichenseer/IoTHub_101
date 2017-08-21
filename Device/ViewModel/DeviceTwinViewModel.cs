using Device.CommonBl.DeviceTwin;
using Device.CommonBl.Events;
using Device.CommonBl.Interface;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Device.ViewModel
{
    public class DeviceTwinViewModel : BaseViewModel
    {

        IDeviceTwin _bl = new DeviceTwin(); 

        public DeviceTwinViewModel()
        {
            _bl.OnDeviceTwinDesiredProperty += _bl_OnDeviceTwinDesiredProperty;

            var deviceTwin = new {
                tags = new
                {
                    Location = new
                    {
                        Region = "GER",
                        City = "Munich",
                    },
                    Type = new
                    {
                        OS = "Win10",
                        Version = "IoTCore"
                    }
                }
            };
            DeviceTwinContent = JsonConvert.SerializeObject(deviceTwin);

            var desiredProperty = new
            {
                properties = new
                {
                    desired = new
                    {
                        MeasurementFrequency = "100",
                    }
                }
            };
            DeviceTwinDesiredProperty = JsonConvert.SerializeObject(desiredProperty);
            
            TwinCollection reportedProperties = new TwinCollection();
            TwinCollection frequency = new TwinCollection();
            frequency["intervall"] = "100";
            reportedProperties["frequency"] = frequency;
            DeviceTwinReportedProperty = JsonConvert.SerializeObject(reportedProperties);
        }

        string _deviceTwinReportedProperty = "";
        public string DeviceTwinReportedProperty
        {
            get { return _deviceTwinReportedProperty; }
            set { _deviceTwinReportedProperty = value; OnPropertyChanged(); }
        }
        
        bool _isListeningForPropertyChange = false;
        public bool IsListeningForPropertyChange
        {
            get { return _isListeningForPropertyChange; }
            set { _isListeningForPropertyChange = value; OnPropertyChanged(); StartStopListeningPropertyChangeContent = ""; }
        }
        
        public string StartStopListeningPropertyChangeContent
        {
            get { return IsListeningForPropertyChange ? "Stop Listening" : "Start Listening"; }
            set { OnPropertyChanged(); }
        }

        private string _cloudStatusDisplay = "";
        public string CloudStatusDisplay
        {
            get { return _cloudStatusDisplay; }
            set { _cloudStatusDisplay = value; OnPropertyChanged(); }
        }

        private string _deviceStatusDisplay = "";
        public string DeviceStatusDisplay
        {
            get { return _deviceStatusDisplay; }
            set { _deviceStatusDisplay = value; OnPropertyChanged(); }
        }
        
        private string _deviceTwinContent = "";
        public string DeviceTwinContent
        {
            get { return _deviceTwinContent;  }
            set { _deviceTwinContent = value; OnPropertyChanged(); }
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

        private string _deviceTwinDesiredProperty = ""; 
        public string DeviceTwinDesiredProperty
        {
            get { return _deviceTwinDesiredProperty; }
            set { _deviceTwinDesiredProperty = value; OnPropertyChanged(); }
        }

        private string getDeviceId(string connectionString)
        {
            string deviceSection = (connectionString.ToUpper().Split(';')).ToList<string>().Where(item => item.Contains("DEVICEID")).FirstOrDefault();
            if (!String.IsNullOrEmpty(deviceSection))
                if (deviceSection.IndexOf('=') < deviceSection.Length)
                    return deviceSection.Substring(deviceSection.IndexOf('=') + 1);

            return "";
        }

        internal async void CreateDeviceTwin()
        {
            Twin twin = await _bl.CreateDeviceTwinAsync(DeviceId, DeviceTwinContent);
            CloudStatusDisplay = $"DeviceTwin created: {JsonConvert.SerializeObject(twin)}"; 
        }

        internal async void ReadDeviceTwin(bool showInServerResults = false)
        {
            if (showInServerResults)
                CloudStatusDisplay = JsonConvert.SerializeObject(await _bl.ReadDeviceTwin(DeviceId));
            else
                DeviceStatusDisplay = JsonConvert.SerializeObject(await _bl.ReadDeviceTwin(DeviceId)); 
        }

        internal void StartStopLIsteningForPropertyChange()
        {
            IsListeningForPropertyChange = !IsListeningForPropertyChange;
            if (IsListeningForPropertyChange)
            {
                DeviceStatusDisplay = $"Start Listening for Property Changes";
                _bl.StartListeningForPropertyChange();
            }
            else
            {
                DeviceStatusDisplay = $"Stop Listening for Property Changes";
                _bl.StopListeningForPropertyChange();
            }
        }

        internal async void SetDesiredProperty()
        {
            await _bl.SetDesiredPropertyAsync(DeviceId, DeviceTwinDesiredProperty);
            CloudStatusDisplay = $"Desired Property Set: {DeviceTwinDesiredProperty}";
        }


        private async void _bl_OnDeviceTwinDesiredProperty(object sender, DeviceTwinDesiredPropertyEventArgs e)
        {
            string result = "";
            foreach (var property in e.DesiredProperties)
            {
                result = string.Concat(result, JsonConvert.SerializeObject(property), '\n');
            }
            await App.CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                DeviceStatusDisplay = result;
            });
        }

        internal async void SendReportedProperty()
        {
            await _bl.SendReportedProperty(DeviceId, DeviceTwinReportedProperty);
            DeviceStatusDisplay = $"Reported Properties send: {DeviceTwinReportedProperty}";
        }


    }
}
