using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Dto
{

    public class TelemetryDto
    {
        public TelemetryDto()
        {
            TelemetryCreationTime = DateTime.Now; 
        }

        public DateTime TelemetryCreationTime { get; set; }
        public string DeviceId { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}
