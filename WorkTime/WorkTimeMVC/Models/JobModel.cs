using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkTimeLogic.Models;

namespace WorkTimeMVC.Models
{
    public class JobModel
    {
        public JobDto Job { get; set; }
        public IEnumerable<ReportDto> Reports { get; set; }
    }
}
