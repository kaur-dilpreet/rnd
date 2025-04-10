using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Domain.Models
{
    public class DateModel
    {
        public DateModel()
        {

        }

        public DateModel(DateTime? dateTime, Boolean showWithTimezoneOffset)
        {
            this.DateFormat = "MM/dd/yyyy hh:mm tt";
            this.DateTime = dateTime;
            this.ShowWithTimezoneOffset = showWithTimezoneOffset;
        }

        public DateModel(DateTime? dateTime, Boolean showWithTimezoneOffset, String dateFormat)
        {
            this.DateFormat = "MM/dd/yyyy hh:mm tt";
            this.DateFormat = dateFormat;
            this.DateTime = dateTime;
            this.ShowWithTimezoneOffset = showWithTimezoneOffset;
        }

        public String DateFormat { get; set; }
        public DateTime? DateTime { get; set; }
        public Boolean ShowWithTimezoneOffset { get; set; }
    }
}
