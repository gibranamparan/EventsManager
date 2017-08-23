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

        public TimeSpan totalTime
        {
            get
            {
                TimeSpan res = new TimeSpan();
                if (startDate != null && endDate != null)
                    res = endDate - startDate;
                return res;
            }
        }

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
            string res = "";
            if (startDate.Date == endDate.Date)
            { // 12/Jun 17 12:00-15:00
                res = string.Format("{0:dd/MMM/yy} {0:HH:mm}-{1:HH:mm}", 
                    this.startDate, this.endDate);
            }
            else if (startDate.Year == startDate.Year)
            { // 12/Jun 15:30-13/Jun 01:45 17
                res = string.Format("{0:dd/MMM HH:mm}-{1:dd/MMM HH:mm yyyy}",
                     this.startDate, this.endDate);
            }
            else
            {
                res = string.Format("{0:dd/MMM/yy} - {1:dd/MMM/yy}",
                     this.startDate, this.endDate);
            }
            return res;
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