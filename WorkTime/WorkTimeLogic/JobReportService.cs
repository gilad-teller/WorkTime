using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTimeCommon;
using WorkTimeLogic.Models;

namespace WorkTimeLogic
{
    public interface IJobReportService
    {
        public Task<JobReport> JobReport(Guid jobId, Guid reportId);
    }
    public class JobReportService : IJobReportService
    {
        private readonly IJobService _jobService;
        private readonly IReportsService _reportsService;

        public JobReportService(IJobService jobService, IReportsService reportsService)
        {
            _jobService = jobService;
            _reportsService = reportsService;
        }

        public async Task<JobReport> JobReport(Guid jobId, Guid reportId)
        {
            JobDto job = await _jobService.GetJobById(jobId);
            ReportDto report = await _reportsService.GetReportById(reportId);
            Tuple<DateTime, DateTime> period = CalcPeriod(report.CalculationPeriodType);
            JobReport jobReport = new JobReport()
            {
                JobName = job.Name,
                ReportName = report.ToString()
            };
            
            IEnumerable<ShiftDto> shiftsInPeriod = job.Shifts.Where(x => x.StartTime >= period.Item1 && x.EndTime < period.Item2);
            foreach (ShiftDto s in shiftsInPeriod)
            {
                jobReport.TotalShifts += s.TotalTime;
            }

            if (report.CalculationPeriodType == report.PayPeriodType)
            {
                jobReport.TotalTimeNeeded = TimeSpan.FromHours(report.PayPeriodHours);
            }
            else if (report.PayPeriodType == PeriodType.Daily)
            {
                for (DateTime i = period.Item1; i < period.Item2; i = i.AddDays(1))
                {
                    if (job.WeekendDays.Contains(i.DayOfWeek))
                    {
                        continue;
                    }
                    if (job.OffDays.Contains(i))
                    {
                        continue;
                    }
                    if (job.HalfDays.Contains(i))
                    {
                        jobReport.TotalTimeNeeded += TimeSpan.FromHours(report.PayPeriodHours / 2);
                        continue;
                    }
                    jobReport.TotalTimeNeeded += TimeSpan.FromHours(report.PayPeriodHours);
                }
            }

            List<DateTime> fullDaysRemainingInPeriod = new List<DateTime>();
            List<DateTime> halfDaysRemainingInPeriod = new List<DateTime>();
            for (DateTime i = DateTime.Today; i < period.Item2; i = i.AddDays(1))
            {
                if (job.WeekendDays.Contains(i.DayOfWeek))
                {
                    continue;
                }
                if (job.OffDays.Contains(i))
                {
                    continue;
                }
                if (job.HalfDays.Contains(i))
                {
                    halfDaysRemainingInPeriod.Add(i);
                    continue;
                }
                fullDaysRemainingInPeriod.Add(i);
            }
            jobReport.FullDaysRemaining = fullDaysRemainingInPeriod.Count;
            jobReport.HalfDaysRemaining = halfDaysRemainingInPeriod.Count;

            jobReport.EstimatedTimeAtEndOfPeriod = jobReport.TotalShifts + (TimeSpan.FromHours(report.EstimatedPayPeriodHours) * fullDaysRemainingInPeriod.Count) + (TimeSpan.FromHours(report.EstimatedPayPeriodHours) * halfDaysRemainingInPeriod.Count / 2);

            return jobReport;
        }

        private static Tuple<DateTime, DateTime> CalcPeriod(PeriodType periodType)
        {
            DateTime today = DateTime.Today;
            switch (periodType)
            {
                case PeriodType.Daily:
                    return new Tuple<DateTime, DateTime>(today, today.AddDays(1));
                case PeriodType.Weekly:
                    DateTime startOfWeek = today.AddDays(-1 * (int)today.DayOfWeek);
                    return new Tuple<DateTime, DateTime>(startOfWeek, startOfWeek.AddDays(7));
                case PeriodType.Monthly:
                    DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
                    return new Tuple<DateTime, DateTime>(startOfMonth, startOfMonth.AddMonths(1));
                case PeriodType.Quarterly:
                    int quarter = (today.Month - 1) / 3;
                    int quarterMonth = (quarter * 3) + 1;
                    DateTime startOfQuarter = new DateTime(today.Year, quarterMonth, 1);
                    return new Tuple<DateTime, DateTime>(startOfQuarter, startOfQuarter.AddMonths(3));
                case PeriodType.Annually:
                    DateTime startOfYear = new DateTime(today.Year, 1, 1);
                    return new Tuple<DateTime, DateTime>(startOfYear, startOfYear.AddYears(1));
            }
            return null;
        }
    }
}
