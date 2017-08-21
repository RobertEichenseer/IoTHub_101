using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Device.Dto;
using Device.CommonBl.Cloud2Device;
using Device.CommonBl.Events;

namespace Device.CommonBl.Interface
{
    public delegate void Cloud2DeviceCommandEventHandler(object sender, Cloud2DeviceCommandEventArgs e);
    public delegate void Cloud2DeviceMethodEventHandler(object sender, Cloud2DeviceMethodEventArgs e);
    public delegate void Cloud2DeviceCommandFeedbackEventHandler(object sender, Cloud2DeviceCommandFeedbackEventArgs e);

    internal interface ICloud2Device
    {
        event Cloud2DeviceCommandEventHandler OnCloud2DeviceCommand;
        event Cloud2DeviceMethodEventHandler OnCloud2DeviceMethod;
        event Cloud2DeviceCommandFeedbackEventHandler OnCloud2DeviceCommandFeedback;

        Task<bool> ListenForCommandsAsync();
        Task<bool> StartStopListeningForMethodsAsync(bool startListening);
        Task SendCloud2DeviceCommand(string deviceId, string cloud2DeviceCommandContent);
        Task ListenForCloud2DeviceCommandFeedback();
        void SetCloud2DeviceMethodReturnValue(string value);
    }
}
