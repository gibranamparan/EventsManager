using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Jerry.GeneralTools
{
    public class TimePeriod
    {
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true,
            DataFormatString = "{0:yyyy-MM-dd}")]
        [DisplayName("Desde")]
        public DateTime startDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true,
            DataFormatString = "{0:yyyy-MM-dd}")]
        [DisplayName("Hasta")]
        public DateTime endDate { get; set; }

        public TimePeriod() { }
        public TimePeriod(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }

        public bool isValid()
        {
            return this.startDate <= this.endDate;
        }

        public override string ToString()
        {
            return string.Format("{0:MMMM/dd/yyyy} - {1:MMMM/dd/yyyy}",
                     this.startDate, this.endDate);
        }

        public string ToString(string dateFormat)
        {
            return string.Format("{0:" + dateFormat + "} - {1:" + dateFormat + "}",
                     this.startDate, this.endDate);
        }
        public bool hasInside(DateTime date)
        {
            return date >= this.startDate && date <= this.endDate;
        }
        public bool hasInside(TimePeriod other)
        {
            return other.startDate >= this.startDate && other.startDate <= this.endDate
                && other.endDate >= this.startDate && other.endDate <= this.endDate;
        }

        public bool hasPartInside(TimePeriod other)
        {
            return other.startDate >= this.startDate && other.startDate <= this.endDate
                || other.endDate >= this.startDate && other.endDate <= this.endDate;
        }

        public bool Equals(TimePeriod other)
        {
            return this.startDate == other.startDate && this.endDate == other.endDate;
        }
    }
}