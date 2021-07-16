using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkTimeLogic.Models;

namespace WorkTimeMVC.Models
{
    public class DayModel
    {
        public Guid JobId { get; }
        public DateTime Date { get; }
        public DayType DayType { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public Guid ShiftId { get; }
        public string DayTypeClass { get; }
        public string DayTypeDescription { get; set; }
        public static IDictionary<DayType, string> DayTypeDescriptions { get; }
        public static IEnumerable<DayType> DayTypesForSelection { get; }
        private static IDictionary<DayType, string> DayTypeClasses { get; }

        static DayModel()
        {
            DayTypeClasses = new Dictionary<DayType, string>
            {
                { DayType.Weekend, "card text-white bg-secondary mb-3" },
                { DayType.WorkDay, "card bg-light mb-3" },
                { DayType.HalfDay, "card text-white bg-secondary mb-3" },
                { DayType.OffDay, "card text-white bg-secondary mb-3" }
            };
            DayTypeDescriptions = new Dictionary<DayType, string>
            {
                { DayType.Weekend, "Weekend" },
                { DayType.WorkDay, "Work Day" },
                { DayType.HalfDay, "Half Day" },
                { DayType.OffDay, "Off Day" }
            };
            DayTypesForSelection = new List<DayType> { DayType.WorkDay, DayType.HalfDay, DayType.OffDay };
        }

        public DayModel(JobDto job, DateTime date)
        {
            JobId = job?.JobId ?? Guid.Empty;
            Date = date.Date;
            if (job?.WeekendDays != null && job.WeekendDays.Contains(Date.DayOfWeek))
            {
                DayType = DayType.Weekend;
            }
            else if (job?.OffDays != null && job.OffDays.Contains(Date))
            {
                DayType = DayType.OffDay;
            }
            else if (job?.HalfDays != null && job.HalfDays.Contains(Date))
            {
                DayType = DayType.HalfDay;
            }
            else
            {
                DayType = DayType.WorkDay;
            }
            ShiftDto shift = job?.Shifts?.FirstOrDefault(x => x.StartTime.Date == Date);
            if (shift != null)
            {
                StartTime = shift.StartTime;
                EndTime = shift.EndTime;
                ShiftId = shift.ShiftId;
            }
            else
            {
                StartTime = DateTime.Now;
                EndTime = DateTime.Now;
            }
            DayTypeClass = DayTypeClasses[DayType];
            DayTypeDescription = DayTypeDescriptions[DayType];
        }
    }

    public enum DayType 
    { 
        WorkDay,
        Weekend,
        OffDay,
        HalfDay
    }
}
