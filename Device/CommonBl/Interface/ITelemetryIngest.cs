using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Device.Dto;
using Device.CommonBl.Events;
using Windows.Storage;

namespace Device.CommonBl.Interface
{
    internal interface ITelemetryIngest
    {
        TelemetryDto GetTelemetryData();
        Task<bool> SendTelemetryDataAsync(TelemetryDto telemetry);
        Task<bool> SendTelemetryDataAsync(string telemetryLine);
        Task<bool> BatchFileUploadAsync(StorageFile storageFile);
        Task<string> CheckFileUploadAckAsync();
    }
}
