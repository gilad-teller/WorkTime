using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkTimeLogic.Models;

namespace WorkTimeMVC.Models
{
    public class MonthModel
    {
        public IEnumerable<DayOfWeek> DaysOfWeek => Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
        public IEnumerable<IEnumerable<DayModel>> Dates { get; }

        public MonthModel(DateTime date, JobDto job)
        {
            DateTime startOfMonth = date.AddDays(-1 * date.Day);
            DateTime dayIteration = startOfMonth.AddDays(-1 * (int)startOfMonth.DayOfWeek);

            List<IEnumerable<DayModel>> weeks = new();
            for (int i = 0; i < 6; i++)
            {
                List<DayModel> week = new();
                for (int j = 0; j < 7; j++)
                {
                    DayModel day = new(job, dayIteration);
                    week.Add(day);
                    dayIteration = dayIteration.AddDays(1);
                }
                weeks.Add(week);
            }
            Dates = weeks;
        }
    }
}
