using System;

namespace FluentSharp.CoreLib
{
    public static class Extra_DateTime_ExtensionMethods
    {
        public static string  jsDate(this DateTime date)
        {
            return date.toJsDate();
        }
        public static string  toJsDate(this DateTime date)
        {
            if (date == default(DateTime))
                return String.Empty;
            var dateTime_1970       = new DateTime(1970, 1, 1);
            var dateTime_Universal  = date.ToUniversalTime();
            var timeSpan            = new TimeSpan(dateTime_Universal.Ticks - dateTime_1970.Ticks);
            return timeSpan.TotalMilliseconds.ToString("#");
        }
        public static DateTime fromJsDate(this string date_Milliseconds_After_1970)
        {
            if (date_Milliseconds_After_1970.valid() && date_Milliseconds_After_1970.isDouble())
            {
                var dateTime = new DateTime(1970, 1, 1);
                return dateTime.AddMilliseconds(date_Milliseconds_After_1970.toDouble());                
            }
            return default(DateTime);
        }
    }
}