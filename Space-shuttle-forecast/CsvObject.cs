using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_shuttle_forecast
{
    public class CsvObject
    {
        public int Day { get; set; }

        public int Temperature { get; set; }

        public int Wind { get; set; }

        public int Humidity { get; set; }

        public int Precipitation { get; set; }

        public string? Lightning { get; set; }

        public string? Clouds { get; set; }

    }
}
