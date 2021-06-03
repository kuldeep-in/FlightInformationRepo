using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlightInfo.Models
{
    public class RawDataModel
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Aircraft { get; set; }
        public string FlightTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTimeOffset STD { get; set; }

        //[DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public string ATD { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTimeOffset STA { get; set; }

        //[DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public string Status { get; set; }

        public string Airline { get; set; }

        public string FlightNumber { get; set; }
    }
}
