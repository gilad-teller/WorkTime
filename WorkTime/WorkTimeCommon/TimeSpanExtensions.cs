using System;
using System.Collections.Generic;
using System.Text;

namespace WorkTimeCommon
{
    public static class TimeSpanExtensions
    {
        public static string ToHoursAndMinutes(this TimeSpan ts)
        {
            int hours = (int)ts.TotalHours;
            int minutes = (int)((ts.TotalHours - hours) * 60);
            return $"{hours} Hours : {minutes} Minutes";
        }
    }
}
