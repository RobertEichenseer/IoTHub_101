using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util_ExecuteCloud2DeviceMethod
{
    class Program
    {
        static void Main(string[] args)
        {
            string iotHubConnectionString = "HostName=<<YourIoTHubName>>.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=<<YourSharedAccessKey>>";
            string deviceId = "<<YourDeviceID>>";
            ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(iotHubConnectionString);

            var c2DMethodDto = new { TargetDeviceId = deviceId, C2DMethod = "RefreshIntervall", Severity = 1};
            CloudToDeviceMethod c2DMethod = new CloudToDeviceMethod("ExecuteC2DMethod") { ResponseTimeout = TimeSpan.FromSeconds(15) };
            c2DMethod.SetPayloadJson(JsonConvert.SerializeObject(c2DMethodDto));

            bool loop = true; 
            while (loop)
            { 
                try
                {
                    Console.WriteLine($"Executing Method ...");
                    Task<CloudToDeviceMethodResult>  task = serviceClient.InvokeDeviceMethodAsync(deviceId, c2DMethod);
                    task.Wait();
                    CloudToDeviceMethodResult methodResult = task.Result; 
                    Console.WriteLine($"Method Result: {methodResult.Status} - Payload: {methodResult.GetPayloadAsJson()}");
                }
                catch (DeviceNotFoundException ex)
                {
                    Console.WriteLine($"Device: {deviceId} isn't online/listening!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occured:{ex.Message}");
                }

                Console.WriteLine("Please press 'r' or 'R' to re-execute Message or any other key to end!");
                string userInput = Console.ReadLine();
                if (userInput.ToUpper() != "R")
                    loop = false; 
            }
        }
    }
}
