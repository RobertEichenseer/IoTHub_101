# IoTHub - 101

Copy & Paste demo code to highlight Azure IoT Hub functionality. Demoed in a Win10 UWP application.


# Create IoT Hub / Create Device
  - Create an IoTHub in your Azure Portal: (https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-create-through-portal)
  - Copy from the portal the "Connection string - primary key" (<< Your IoT Hub -> "Shared Access policies" -> "iothubowner" -> "Connection string - primary key"
  - Download the Device Explorer tool from github (https://github.com/Azure/azure-iot-sdk-csharp/tree/master/tools/DeviceExplorer) 
  - Run DeviceExplorer on your computer (Windows)
  - Copy the "Connection string - primary key" from the Azure Portal to the "IoT Hub Connection String" text box and click "Update"
  - Go to the management tab, click "Create" and create a new device
  - Right click the created device and select "Copy connection string for selected device"
  - Go back to the Azure portal and copy the connection string for  "device" (<< Your IoT Hub -> "Shared Access policies" -> "device")
  
  
# Insert Connection Strings into demo code 
- Open the file "\IoTHub_101\Device\Configuration\Configuration.cs"
- Replace ServiceIotHubConnectionString with your "Device Connection String" from the Azure portal
```sh
  public static string ServiceIoTHubConnectionString = "HostName=<<YourIoTHubName>>.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=<<YourSharedAccessKey>>";
```
- Replace DeviceIotHubConnectionString with your "Device Connection String" created by the Device Explorer
```sh
  public static string ServiceIoTHubConnectionString = public static string DeviceIoTHubConnectionString = "HostName=<<YourIoTHubName>>.azure-devices.net;DeviceId=<<YourDeviceName>>;SharedAccessKey=<<YourSharedAccessKey>>";
```
- Replace OwnerIotHubConnectionString with your "IoT Hub Owner Connection String" from the Azure Portal
```sh
  public static string ServiceIoTHubConnectionString = public static string OwnerIoTHubConnectionString = "HostName=<<YourIoTHubName>>.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=<<YourSharedAccessKey>>";
```
- Open the file "\IoTHub_101\Util_ExecuteCloud2DeviceMethod\Program.cs"
- Replace ServiceIoTHubConnectionString with your "Device Connection String" from the Azure portal
```sh
  string iotHubConnectionString = "HostName=<<YourIoTHubName>>.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=<<YourSharedAccessKey>>";
```
- Replace DeviceId with your "Device Id" created by the Device Explorer
```sh
  string deviceId = "<<YourDeviceID>>";
```

**Copy & Paste Demo Code - Copy & Paste Demo Code - Don't use in production!**





