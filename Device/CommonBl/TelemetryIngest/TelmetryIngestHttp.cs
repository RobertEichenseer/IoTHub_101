using Device.CommonBl.Interface;
using Device.Dto;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using PCLCrypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;

namespace Device.CommonBl.TelemetryIngest
{
    public class TelemetryIngestHttp : ITelemetryIngest
    {
        DeviceClient _deviceClient;

        public TelemetryIngestHttp()
        {
            
        }

        public TelemetryDto GetTelemetryData()
        {
            Random randomNumber = new Random(DateTime.Now.Millisecond);

            return new TelemetryDto()
            {
                DeviceId = Configuration.DeviceId,
                Temperature = 20 + (randomNumber.NextDouble() * 10),
                Humidity = 40 + (randomNumber.NextDouble() * 10),
            };
        }

        public async Task<bool> SendTelemetryDataAsync(TelemetryDto telemetry)
        {
            string sASToken = CreateServiceBusSASToken(Configuration.DeviceId, Configuration.DeviceSummetricKey, Configuration.IoTHubFullName);

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("Authorization", sASToken);
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(telemetry));

                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(Configuration.DeviceIoTHubTargetUrl, httpContent);
                if (httpResponseMessage.IsSuccessStatusCode)
                    return true;
                else
                    return false;
            }
        }


        private string CreateServiceBusSASToken(string deviceId, string deviceSymmetricKey, string ioTHubUri)
        {
            int expirySeconds = (int)DateTime.UtcNow.AddMinutes(20).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            string stringToSign = $"{WebUtility.UrlEncode(ioTHubUri)}%2Fdevices%2F{deviceId}\n{expirySeconds.ToString()}";
        
            string signature = Sign(stringToSign, deviceSymmetricKey); 
            return String.Format("SharedAccessSignature sr={0}%2Fdevices%2F{1}&sig={2}&se={3}", WebUtility.UrlEncode(ioTHubUri), deviceId, WebUtility.UrlEncode(signature), expirySeconds);
        }


        static string Sign(string stringToSign, string deviceSymmetricKey)
        {
            IMacAlgorithmProvider algorithm = WinRTCrypto.MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha256);
            PCLCrypto.CryptographicHash hash = algorithm.CreateHash(Convert.FromBase64String(deviceSymmetricKey));
            hash.Append(Encoding.UTF8.GetBytes(stringToSign));
            byte[] mac = hash.GetValueAndReset();

            return Convert.ToBase64String(mac);
        }

        public Task<bool> BatchFileUploadAsync(StorageFile storageFile)
        {
            //Just available in SDK implementation
            return Task.FromResult<bool>(false);
        }


        public Task<bool> BatchFileUploadAsync(StorageFile storageFile, bool listenForFileUploadAck)
        {
            //Just available in SDK implementation
            return Task.FromResult<bool>(false);
        }

        public Task<string> CheckFileUploadAckAsync()
        {
            //Just available in SDK implementation
            return Task.FromResult<string>("");
        }

        public Task<bool> SendTelemetryDataAsync(string telemetryLine)
        {
            //Just available in SDK implementation
            return Task.FromResult<bool>(false);
        }
    }
}
