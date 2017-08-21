using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    public static class Configuration
    {
        public static string ServiceIoTHubConnectionString = "HostName=<<YourIoTHubName>>.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=<<YourSharedAccessKey>>";
        public static string DeviceIoTHubConnectionString = "HostName=<<YourIoTHubName>>.azure-devices.net;DeviceId=DeviceRunningWin10;SharedAccessKey=<<YourSharedAccessKey>>";
        public static string OwnerIoTHubConnectionString = "HostName=<<YourIoTHubName>>.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=<<YourSharedAccessKey>>";

        private static Dictionary<string, string> _deviceIotHubConnectionParameter = new Dictionary<string, string>(); 
        public static Dictionary<string,string> IoTHubConnectionParameter
        {
            get
            {
                if (_deviceIotHubConnectionParameter.Count < 1)
                {
                    foreach(var item in DeviceIoTHubConnectionString.Split(';').ToList<string>())
                    {
                        string key = item.Substring(0, item.IndexOf('='));
                        string value = item.Substring(item.IndexOf('=') + 1);
                        _deviceIotHubConnectionParameter.Add(key, value);
                    }
                }
                return _deviceIotHubConnectionParameter; 
            }
        }

        public static string DeviceIoTHubTargetUrl
        {
            get { return $"https://{IoTHubFullName}/devices/{DeviceId}/messages/events?api-version=2016-11-14"; }
        }
        public static string DeviceId
        {
            get { return IoTHubConnectionParameter.Where(item => item.Key == "DeviceId").Select(item => item.Value).FirstOrDefault(); }
        }

        public static string IoTHubFullName
        {
            get { return IoTHubConnectionParameter.Where(item => item.Key == "HostName").Select(item => item.Value).FirstOrDefault(); }
        }

        public static string DeviceSummetricKey
        {
            get { return IoTHubConnectionParameter.Where(item => item.Key == "SharedAccessKey").Select(item => item.Value).FirstOrDefault(); }
        }

    }
}
