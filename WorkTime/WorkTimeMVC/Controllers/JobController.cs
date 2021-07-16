using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkTimeCommon;
using WorkTimeLogic;
using WorkTimeLogic.Models;
using WorkTimeMVC.Models;

namespace WorkTimeMVC.Controllers
{
    public class JobController : Controller
    {
        private readonly ILogger<JobController> _logger;
        private readonly IJobService _jobService;
        private readonly IReportsService _reportService;
        private readonly IJobReportService _jobReportService;

        public JobController(ILogger<JobController> logger, IJobService jobService, IReportsService reportsService, IJobReportService jobReportService)
        {
            _logger = logger;
            _jobService = jobService;
            _reportService = reportsService;
            _jobReportService = jobReportService;
        }

        public async Task<IActionResult> Index(Guid? jobId = null)
        {
            JobDto job = null;
            IEnumerable<ReportDto> reports = null;
            if (jobId.HasValue)
            {
                job = await _jobService.GetJobById(jobId.Value);
                reports = await _reportService.GetReports();
            }
            JobModel model = new()
            {
                Job = job,
                Reports = reports
            };
            return View(model);
        }

        public async Task<IActionResult> Calendar(DateTime month, Guid jobId)
        {
            JobDto job = await _jobService.GetJobById(jobId);
            if (job == null)
            {
                throw new ArgumentException($"Couldn't find job {jobId}", nameof(jobId));
            }
            CalendarModel calendarModel = new(month, job);
            return View(calendarModel);
        }

        public async Task<IActionResult> SaveJob(Guid jobId, string name, IFormCollection formCollection)
        {
            JobDto job = new()
            {
                Name = name
            };
            foreach (DayOfWeek dow in Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>())
            {
                if (formCollection.ContainsKey($"dow-{dow}"))
                {
                    job.AddWeekendDay(dow);
                }
            }
            if (jobId == Guid.Empty)
            {
                jobId = await _jobService.TryAddJob(job);
            }
            else
            {
                job.JobId = jobId;
                await _jobService.TryUpdateJob(job);
            }
            return RedirectToAction("Index", new { jobId });
        }

        public async Task<IActionResult> SaveDay(DayType dayType, DateTime startTime, DateTime endTime, Guid shiftId, Guid jobId, DateTime date)
        {
            if (startTime != DateTime.MinValue && endTime != DateTime.MinValue && date != DateTime.MinValue)
            {
                DateTime startDateTime = new(date.Year, date.Month, date.Day, startTime.Hour, startTime.Minute, 0);
                DateTime endDateTime = new(date.Year, date.Month, date.Day, endTime.Hour, endTime.Minute, 0);
                if (shiftId == Guid.Empty)
                {
                    ShiftDto shiftDto = new(startDateTime, endDateTime);
                    await _jobService.TryAddShift(jobId, shiftDto);
                }
                else
                {
                    ShiftDto shiftDto = new(shiftId, startDateTime, endDateTime);
                    await _jobService.TryUpdateShift(shiftDto);
                }
            }
            if (dayType == DayType.OffDay)
            {
                await _jobService.TryAddOffDay(jobId, date);
            }
            if (dayType == DayType.HalfDay)
            {
                await _jobService.TryAddHalfDay(jobId, date);
            }
            return RedirectToAction("Calendar", new { month = date, jobId });
        }

        public async Task<IActionResult> JobReport(Guid jobId, Guid reportId)
        {
            JobReport jobReport = await _jobReportService.JobReport(jobId, reportId);
            JobReportModel model = new()
            {
                JobReport = jobReport
            };
            return View(model);
        }
    }
}
