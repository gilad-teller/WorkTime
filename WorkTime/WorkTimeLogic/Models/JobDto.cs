using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkTimeCommon;
using WorkTimeDB;

namespace WorkTimeLogic.Models
{
    public class JobDto
    {
        public Guid JobId { get; set; }
        public string Name { get; set; }
        public IEnumerable<DateTime> OffDays { get; }
        public IEnumerable<DateTime> HalfDays { get; }
        public IEnumerable<DayOfWeek> WeekendDays { get; }
        public IEnumerable<ShiftDto> Shifts { get; }

        public JobDto()
        {
            OffDays = new List<DateTime>();
            HalfDays = new List<DateTime>();
            WeekendDays = new List<DayOfWeek>();
            Shifts = new List<ShiftDto>();
        }

        public JobDto(Job dbJob)
        {
            JobId = dbJob.JobId;
            Name = dbJob.Name;
            WeekendDays = dbJob.WeekendDays.Split(",").Select(y => Enum.Parse<DayOfWeek>(y));
            OffDays = dbJob.OffDays != null ? dbJob.OffDays.Where(x => x.OffDayType == OffDayType.Full).Select(y => y.Date) : Enumerable.Empty<DateTime>();
            HalfDays = dbJob.OffDays != null ? dbJob.OffDays.Where(x => x.OffDayType == OffDayType.Half).Select(y => y.Date) : Enumerable.Empty<DateTime>();
            Shifts = dbJob.Shifts != null ? dbJob.Shifts.Select(x => new ShiftDto(x.ShiftId, x.StartTime, x.EndTime)) : Enumerable.Empty<ShiftDto>();
        }

        public void AddWeekendDay(DayOfWeek dayOfWeek)
        {
            if (!WeekendDays.Contains(dayOfWeek) && WeekendDays is List<DayOfWeek> lst)
            {
                lst.Add(dayOfWeek);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
