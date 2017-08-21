using Device.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Device.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TelemetryIngest : Page
    {
        TelemetryIngestViewModel _viewModel = new TelemetryIngestViewModel(); 

        public TelemetryIngest()
        {
            this.InitializeComponent();
            this.grdMain.DataContext = _viewModel;
        }

        private void ClearTelemetryStatus_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.TelemetryStatus = "";
        }

        private void StartStopSendingTelemetrySDK_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.StartStopSendingTelemetrySDK();    
        }

        private void SelectUploadFile_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectUploadFile(); 
        }

        private void UploadFile_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.BatchFileUpload();
        }

        private void IngestFileLineByLine_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.IngestFileLineByLine();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.LoadDefaultBatchUploadFile();
        }
    }
}
