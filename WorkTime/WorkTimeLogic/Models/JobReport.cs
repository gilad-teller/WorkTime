using System;
using System.Collections.Generic;
using System.Text;

namespace WorkTimeLogic.Models
{
    public class JobReport
    {
        public string JobName { get; set; }
        public string ReportName { get; set; }
        public TimeSpan TotalShifts { get; set; }
        public TimeSpan TotalTimeNeeded { get; set; }
        public int FullDaysRemaining { get; set; }
        public int HalfDaysRemaining { get; set; }
        public TimeSpan EstimatedTimeAtEndOfPeriod { get; set; }
        public TimeSpan TotalTimeRemaining => TotalTimeNeeded - TotalShifts;
        public TimeSpan TimePerDayNeeded
        {
            get
            {
                double daysRemaining = FullDaysRemaining + (HalfDaysRemaining / 2);
                if (daysRemaining == 0)
                {
                    return TimeSpan.Zero;
                }
                return TotalTimeRemaining.Divide(daysRemaining);
            }
        }
        public TimeSpan TimePerDayNeededNoHalfDays => FullDaysRemaining > 0 ? TotalTimeRemaining.Divide(FullDaysRemaining) : TimeSpan.Zero;

        public JobReport()
        {
            TotalShifts = TimeSpan.Zero;
            TotalTimeNeeded = TimeSpan.Zero;
        }
    }
}
