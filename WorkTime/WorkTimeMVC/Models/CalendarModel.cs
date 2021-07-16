using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkTimeLogic.Models;

namespace WorkTimeMVC.Models
{
    public class CalendarModel
    {
        public CalendarModel(DateTime month, JobDto job)
        {
            Month = month;
            Job = job;
        }
        public DateTime Month { get; set; }
        public JobDto Job { get; set; }
    }
}
