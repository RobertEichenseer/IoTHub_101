
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Device.CommonBl.Interface;
using Microsoft.Azure.Devices.Client;
using Device.Dto;
using Windows.Storage;
using System.IO;
using Windows.Storage.Streams;


namespace Device.CommonBl
{
    
    public class TelemetryIngestSDK : ITelemetryIngest
    {
        DeviceClient _deviceClient;
        Microsoft.Azure.Devices.ServiceClient _serviceClient;

        public TelemetryIngestSDK()
        {
            _deviceClient = DeviceClient.CreateFromConnectionString(Configuration.DeviceIoTHubConnectionString, TransportType.Mqtt);
            _serviceClient = Microsoft.Azure.Devices.ServiceClient.CreateFromConnectionString(Configuration.ServiceIoTHubConnectionString, Microsoft.Azure.Devices.TransportType.Amqp);
        }

        public TelemetryDto GetTelemetryData()
        {
            Random randomNumber = new Random(DateTime.Now.Millisecond);
            
            return new TelemetryDto() {
                DeviceId = Configuration.DeviceId,
                Temperature = 20 + (randomNumber.NextDouble() * 10),
                Humidity = 40 + (randomNumber.NextDouble() * 10),
            };
        }

        public async Task<bool> SendTelemetryDataAsync(TelemetryDto telemetry)
        {
            Message telemetryMessage = new Message(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(telemetry)));
            telemetryMessage.Properties.Add("temperatureAlert", (telemetry.Temperature > 30) ? "true" : "false");

            try { 
                await _deviceClient.SendEventAsync(telemetryMessage);
            }
            catch(Exception ex)
            {
                return false; 
            }
            
            return true; 
        }

        public async Task<bool> SendTelemetryDataAsync(string telemetryLine)
        {
            Message telemetryMessage = new Message(Encoding.ASCII.GetBytes(telemetryLine));
            try
            {
                await _deviceClient.SendEventAsync(telemetryMessage);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> BatchFileUploadAsync(StorageFile storageFile)
        {
            try
            {
                using (var randomAccessStream = await storageFile.OpenReadAsync())
                { 
                    Stream stream = randomAccessStream.AsStreamForRead();
                    await _deviceClient.UploadToBlobAsync(storageFile.Name, stream);
                }
            }
            catch(Exception ex)
            { 
                return false;
            }

            return true; 
        }

        public async Task<string> CheckFileUploadAckAsync()
        {
            var notificationReceiver = _serviceClient.GetFileNotificationReceiver();

            var fileUploadNotification = await notificationReceiver.ReceiveAsync(TimeSpan.FromSeconds(5));
            if (fileUploadNotification != null)
            {
                await notificationReceiver.CompleteAsync(fileUploadNotification);
                return $"Blobname: {fileUploadNotification.BlobName} - DeviceId: {fileUploadNotification.DeviceId})";
            }

            return null; 
        }


    }
}
