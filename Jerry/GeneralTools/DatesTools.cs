using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web;
using System.Globalization;

namespace Jerry.GeneralTools
{
    public class DatesTools
    {
        public static class DatesToText
        {
            public static string ConvertToMonth(DateTime date, string culture)
            {
                return new DateTime(date.Year, date.Month, date.Day).ToString("MMMM", CultureInfo.CreateSpecificCulture(culture));                
            }
        }
    }
}