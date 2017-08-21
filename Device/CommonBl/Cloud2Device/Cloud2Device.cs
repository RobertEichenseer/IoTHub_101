using Device.CommonBl.Events;
using Device.CommonBl.Interface;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Device.CommonBl.Cloud2Device
{
    
    public class Cloud2Device : ICloud2Device
    {
        public event Cloud2DeviceCommandEventHandler OnCloud2DeviceCommand;
        public event Cloud2DeviceMethodEventHandler OnCloud2DeviceMethod;
        public event Cloud2DeviceCommandFeedbackEventHandler OnCloud2DeviceCommandFeedback;

        private DeviceClient _deviceClient;
        private ServiceClient _serviceClient;

        private string _returnPayloadCloud2DeviceMethod = "{'success' : true}";
        private int _returnCodeCloud2DeviceMethod = 200; 


        public Cloud2Device()
        {
            _deviceClient = DeviceClient.CreateFromConnectionString(Configuration.DeviceIoTHubConnectionString, Microsoft.Azure.Devices.Client.TransportType.Mqtt);
            _serviceClient = ServiceClient.CreateFromConnectionString(Configuration.ServiceIoTHubConnectionString, Microsoft.Azure.Devices.TransportType.Amqp);
        }

        public async Task<bool> ListenForCommandsAsync()
        {
            Microsoft.Azure.Devices.Client.Message c2DMessage = await _deviceClient.ReceiveAsync(TimeSpan.FromSeconds(1));
            if (c2DMessage == null)
                return false;

            string payload = Encoding.ASCII.GetString(c2DMessage.GetBytes());
            await _deviceClient.CompleteAsync(c2DMessage);
            
            OnCloud2DeviceCommand?.Invoke(this, new Cloud2DeviceCommandEventArgs(payload));
            
            return true; 
        }

        public async Task<bool> StartStopListeningForMethodsAsync(bool startListening)
        {
            if (startListening)
                await _deviceClient.SetMethodHandlerAsync("ExecuteC2DMethod", OnExecuteC2DMethod, null);
            else
                await _deviceClient.SetMethodHandlerAsync("ExecuteC2DMethod", null, null);
            
            return true; 
        }

        private Task<MethodResponse> OnExecuteC2DMethod(MethodRequest methodRequest, object userContext)
        {
            OnCloud2DeviceMethod?.Invoke(this, new Cloud2DeviceMethodEventArgs(methodRequest.DataAsJson));

            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(_returnPayloadCloud2DeviceMethod), 200));
        }

        public async Task SendCloud2DeviceCommand(string deviceId, string cloud2DeviceCommandContent)
        {
            Microsoft.Azure.Devices.Message c2DMessage = new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes(cloud2DeviceCommandContent));
            c2DMessage.Ack = Microsoft.Azure.Devices.DeliveryAcknowledgement.Full;
            c2DMessage.ExpiryTimeUtc = DateTime.Now.AddSeconds(3);

            await _serviceClient.SendAsync(deviceId, c2DMessage);
        }

        public async Task ListenForCloud2DeviceCommandFeedback()
        {
            FeedbackReceiver<FeedbackBatch> feedbackReceiver = _serviceClient.GetFeedbackReceiver();
            FeedbackBatch feedbackBatch = await feedbackReceiver.ReceiveAsync(TimeSpan.FromSeconds(5));

            if (feedbackBatch == null)
                return;

            OnCloud2DeviceCommandFeedback?.Invoke(this, new Cloud2DeviceCommandFeedbackEventArgs($"{String.Join("\n", feedbackBatch.Records.Select(item => item.StatusCode).ToArray())}"));
            
            await feedbackReceiver.CompleteAsync(feedbackBatch);

        }

        public void SetCloud2DeviceMethodReturnValue(string value)
        {
            _returnPayloadCloud2DeviceMethod = value; 
            if (value.ToUpper().Contains("RETURNCODE"))
            {
                try
                {
                    string returnValue = value.Replace(" ", "").ToUpper();
                    int startIndex = returnValue.IndexOf("'RETURNCODE':") + "'RETURNCODE':".Length;
                    int length = returnValue.IndexOf(",", startIndex +1) - (startIndex);
                    var temp = returnValue.Substring(startIndex + 2, length);
                }
                finally { 
                    _returnCodeCloud2DeviceMethod = 100;
                }
            }
        }
    }
}
