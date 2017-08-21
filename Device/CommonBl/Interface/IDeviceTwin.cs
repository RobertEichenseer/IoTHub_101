using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Device.Dto;
using Device.CommonBl.Cloud2Device;
using Device.CommonBl.Events;
using Microsoft.Azure.Devices.Shared;

namespace Device.CommonBl.Interface
{
    public delegate void DeviceTwinDesiredPropertyEventHandler(object sender, DeviceTwinDesiredPropertyEventArgs e);

    internal interface IDeviceTwin
    {
        event DeviceTwinDesiredPropertyEventHandler OnDeviceTwinDesiredProperty;

        Task<Twin> CreateDeviceTwinAsync(string deviceTwinId, string deviceTwinContent);
        Task<Twin> ReadDeviceTwin(string deviceId);
        void StartListeningForPropertyChange();
        void StopListeningForPropertyChange();
        Task<bool> SetDesiredPropertyAsync(string deviceId, string deviceTwinDesiredProperty);
        
        Task<bool> SendReportedProperty(string deviceId, string deviceTwinReportedProperty);
    }
}
