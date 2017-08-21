using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.CommonBl.Events
{
    public class DeviceTwinDesiredPropertyEventArgs : EventArgs
    {
        public TwinCollection DesiredProperties { get; set; }
        public DeviceTwinDesiredPropertyEventArgs(TwinCollection twinCollection)
        {
            DesiredProperties = twinCollection; 
        }
    }
}
