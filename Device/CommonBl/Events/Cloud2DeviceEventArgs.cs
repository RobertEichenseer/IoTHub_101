using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.CommonBl.Events
{
    public class Cloud2DeviceCommandEventArgs : EventArgs
    {
        public Cloud2DeviceCommandEventArgs(string payload)
        {
            Payload = payload; 
        }
        public string Payload { get; set; }

    }

    public class Cloud2DeviceMethodEventArgs : EventArgs
    {
        public Cloud2DeviceMethodEventArgs(string payload)
        {
            Payload = payload;
        }
        public string Payload { get; set; }
    }

    public class Cloud2DeviceCommandFeedbackEventArgs : EventArgs
    {
        public Cloud2DeviceCommandFeedbackEventArgs(string payload)
        {
            Payload = payload;
        }
        public string Payload { get; set; }
    }
}
