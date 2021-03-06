using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkTimeLogic.Models;

namespace WorkTimeMVC.Models
{
    public class HomeModel
    {
        public IEnumerable<JobDto> Jobs { get; set; }
        public IEnumerable<ReportDto> Reports { get; set; }
    }
}
