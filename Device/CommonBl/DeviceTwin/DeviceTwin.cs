using Device.CommonBl.Events;
using Device.CommonBl.Interface;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Device.CommonBl.DeviceTwin
{
    
    public class DeviceTwin : IDeviceTwin
    {
        private DeviceClient _deviceClient;
        private ServiceClient _serviceClient;

        private RegistryManager _registryManager;

        public event DeviceTwinDesiredPropertyEventHandler OnDeviceTwinDesiredProperty;

        public DeviceTwin()
        {
            _deviceClient = DeviceClient.CreateFromConnectionString(Configuration.DeviceIoTHubConnectionString, Microsoft.Azure.Devices.Client.TransportType.Mqtt);
            _serviceClient = ServiceClient.CreateFromConnectionString(Configuration.ServiceIoTHubConnectionString, Microsoft.Azure.Devices.TransportType.Amqp);

            _registryManager = RegistryManager.CreateFromConnectionString(Configuration.ServiceIoTHubConnectionString);
        }

        public async Task<Twin> CreateDeviceTwinAsync(string deviceId, string deviceTwinContent)
        {
            
            Twin twin = await _registryManager.GetTwinAsync(deviceId);
            try
            {
                await _deviceClient.UpdateReportedPropertiesAsync(JsonConvert.DeserializeObject<TwinCollection>("{\"frequency\":{\"intervall\":\"\"}}"));
                await _registryManager.UpdateTwinAsync(deviceId, "{\"properties\":{\"desired\":{\"MeasurementFrequency\":\"\"}}}", twin.ETag);
                twin = await _registryManager.GetTwinAsync(deviceId);
            }
            catch (Exception ex)
            {
                return new Twin(); 
            }

            return twin;
        }

        public async Task<Twin> ReadDeviceTwin(string deviceId)
        {
            return await _deviceClient.GetTwinAsync();
        }
        
        public async void StartListeningForPropertyChange()
        {
            await _deviceClient.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyUpdateAsync, null);
            
        }
        public void StopListeningForPropertyChange()
        {
            //Porperty Update Method can't be removed in current SDK; New DeviceClient is created instead.
            _deviceClient = DeviceClient.CreateFromConnectionString(Configuration.DeviceIoTHubConnectionString, Microsoft.Azure.Devices.Client.TransportType.Mqtt);
        }

        private async Task<bool> OnDesiredPropertyUpdateAsync(TwinCollection desiredProperties, object userContext)
        {
            TwinCollection twinCollection = await Task.Run(() => desiredProperties); 
            OnDeviceTwinDesiredProperty?.Invoke(this, new DeviceTwinDesiredPropertyEventArgs(twinCollection));
            return true;
        }


        public async Task<bool> SetDesiredPropertyAsync(string deviceId, string deviceTwinDesiredProperty)
        {
            //Multiple Devices can be retrieved based on tags if required

            //string deviceQuery = $"SELECT * FROM devices WHERE tags.location.region = '{location}'";
            //IQuery query = _registryManager.CreateQuery(deviceQuery, 10);
            //IEnumerable<Twin> deviceTwins = await query.GetNextAsTwinAsync();

            Twin twin = await _registryManager.GetTwinAsync(deviceId);
            await _registryManager.UpdateTwinAsync(deviceId, deviceTwinDesiredProperty, twin.ETag);

            return true; 
        }

        public async Task<bool> SendReportedProperty(string deviceId, string deviceTwinReportedProperty)
        {
            try
            { 
                Twin twin = await _registryManager.GetTwinAsync(deviceId);
                await _deviceClient.UpdateReportedPropertiesAsync(JsonConvert.DeserializeObject<TwinCollection>(deviceTwinReportedProperty));
            }
            catch(Exception ex)
            {
                return false; 
            }

            return true; 
        }
    }
}

