
using Device.CommonBl;
using Device.CommonBl.Interface;
using Device.CommonBl.TelemetryIngest;
using Device.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;

namespace Device.ViewModel
{
    internal class TelemetryIngestViewModel : BaseViewModel
    {
        ITelemetryIngest _bl;
        ITelemetryIngest _blHttp; 
        DispatcherTimer _sendTelemetryTimer;
        DispatcherTimer _checkFileUploadAck;

        List<string> _telemetryList = new List<string>();


        public TelemetryIngestViewModel()
        {
            _bl = new TelemetryIngestSDK();
            _blHttp = new TelemetryIngestHttp(); 

            _sendTelemetryTimer = new DispatcherTimer();
            _sendTelemetryTimer.Tick += _sendTelemetryTimer_Tick;
            _sendTelemetryTimer.Interval = TimeSpan.FromSeconds(1);
            _sendTelemetryTimer.Stop();

            _checkFileUploadAck = new DispatcherTimer();
            _checkFileUploadAck.Tick += _checkFileUploadAck_Tick;
            _checkFileUploadAck.Interval = TimeSpan.FromSeconds(1);
            _checkFileUploadAck.Stop();
            
        }
        
        private StorageFile _batchUploadFile;
        public StorageFile BatchUploadFile
        {
            get { return _batchUploadFile; }
            set { _batchUploadFile = value; OnPropertyChanged(); BatchUploadFileName = ""; IsBatchUploadFileSelected = true; }
        }

        public string BatchUploadFileName
        {
            get { return _batchUploadFile != null ? _batchUploadFile.Name : ""; }
            set { OnPropertyChanged(); }
        }

        public bool IsBatchUploadFileSelected
        {
            get { return _batchUploadFile != null; }
            set { OnPropertyChanged(); }
        }
        
        private bool _isListeningForFileUploadAck = false;
        public bool IsListeningForFileUploadAck
        {
            get { return _isListeningForFileUploadAck; }
            set { _isListeningForFileUploadAck = value; OnPropertyChanged(); }
        }

        private bool _isHttpIngest = false;
        public bool IsHttpIngest
        {
            get { return _isHttpIngest; }
            set { _isHttpIngest = value; OnPropertyChanged(); }
        }

        private bool _isSendingTelemetry = false;
        public bool IsSendingTelemetry
        {
            get { return _isSendingTelemetry; }
            set { _isSendingTelemetry = value; OnPropertyChanged(); StartStopSendingTelemetryButtonCaption = "";  }
        }

        public string StartStopSendingTelemetryButtonCaption
        {
            get { return IsSendingTelemetry ? "Stop Sending Telemetry SDK" : "Start Sending Telemetry SDK"; }
            set { OnPropertyChanged(); }
        }


        private string _ioTHubConnectionString = Configuration.DeviceIoTHubConnectionString; 
        public string IoTHubConnectionString
        {
            get { return _ioTHubConnectionString; }
            set
            {
                _ioTHubConnectionString = value;
                DeviceId = getDeviceId(value);
                OnPropertyChanged();
            }
        }
        
        private string _deviceId = Configuration.DeviceId;
        public string DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; OnPropertyChanged(); }
        }

        private string _telemetryStatus = "";
        public string TelemetryStatus
        {
            get { return _telemetryStatus; }
            set { _telemetryStatus = value; OnPropertyChanged(); }
        }
        
        private string getDeviceId(string connectionString)
        {
            string deviceSection = (connectionString.ToUpper().Split(';')).ToList<string>().Where(item => item.Contains("DEVICEID")).FirstOrDefault();
            if (!String.IsNullOrEmpty(deviceSection))
                if (deviceSection.IndexOf('=') < deviceSection.Length)
                    return deviceSection.Substring(deviceSection.IndexOf('=')+1);

            return ""; 
        }

        internal void StartStopSendingTelemetrySDK()
        {
            IsSendingTelemetry = !IsSendingTelemetry;
            if (!IsSendingTelemetry)
            {
                TelemetryStatus = $"Telemetry Ingest Stopped";
                _sendTelemetryTimer.Stop();
            }
            else
            { 
                _sendTelemetryTimer.Start();
                TelemetryStatus = $"Start Telemetry Ingest!";
            }
        }
        
        private async void _sendTelemetryTimer_Tick(object sender, object e)
        {   
            TelemetryDto telemetry = _bl.GetTelemetryData();
            TelemetryStatus = ($"Trying to send: {JsonConvert.SerializeObject(telemetry)}");

            _telemetryList.Add(JsonConvert.SerializeObject(telemetry));
            
            if (IsHttpIngest)
                await _blHttp.SendTelemetryDataAsync(telemetry);
            else
                await _bl.SendTelemetryDataAsync(telemetry);
            TelemetryStatus = $"Sent: {JsonConvert.SerializeObject(telemetry)}";
        }

        internal async void LoadDefaultBatchUploadFile()
        {
            StorageFolder packageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            BatchUploadFile = await (await packageFolder.GetFolderAsync(@"Assets\DemoData")).GetFileAsync("DemoData.txt");
        }

        internal async void SelectUploadFile()
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.ViewMode = PickerViewMode.List;
            fileOpenPicker.FileTypeFilter.Add(".dat");
            fileOpenPicker.FileTypeFilter.Add(".xlsx");
            fileOpenPicker.FileTypeFilter.Add(".txt");
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            BatchUploadFile = await fileOpenPicker.PickSingleFileAsync(); 
        }

        internal async void BatchFileUpload()
        {
            if (IsListeningForFileUploadAck)
                _checkFileUploadAck.Start(); 
            else
                _checkFileUploadAck.Stop();

            TelemetryStatus = ($"Trying to upload: {BatchUploadFile.Name}");
            bool uploadSuccess = await _bl.BatchFileUploadAsync(BatchUploadFile);
            TelemetryStatus = uploadSuccess ? ($"{BatchUploadFile.Name} succesfully uploaded!") : ($"Error during upload of {BatchUploadFile.Name}");
        }
        private async void _checkFileUploadAck_Tick(object sender, object e)
        {
            _checkFileUploadAck.Stop();
            string fileUploadStatus = await _bl.CheckFileUploadAckAsync();
            if (fileUploadStatus != null)
            {
                TelemetryStatus = ($"{TelemetryStatus + '\n'}File upload reported from Backend: {fileUploadStatus}");
            }
            else
            { 
                _checkFileUploadAck.Start();
            }
        }
        internal async void IngestFileLineByLine()
        {
            using (Stream inputStream = (await BatchUploadFile.OpenReadAsync()).AsStream())
            {
                StreamReader streamReader = new StreamReader(inputStream);
                string telemetryLine = await streamReader.ReadLineAsync();
                while(!String.IsNullOrEmpty(telemetryLine))
                {
                    TelemetryStatus = ($"Trying to send: {telemetryLine}");
                    await _bl.SendTelemetryDataAsync(telemetryLine);
                    TelemetryStatus = $"Sent: {telemetryLine}";
                    telemetryLine = await streamReader.ReadLineAsync();
                }
            }
        }
    }
}
