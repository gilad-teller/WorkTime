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
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IReportsService _reportsService;

        public ReportController(ILogger<ReportController> logger, IReportsService reportsService)
        {
            _logger = logger;
            _reportsService = reportsService;
        }

        public async Task<IActionResult> Index(Guid? reportId = null)
        {
            ReportDto report = null;
            if (reportId.HasValue)
            {
                report = await _reportsService.GetReportById(reportId.Value);
            }
            ReportModel model = new()
            {
                Report = report
            };
            return View(model);
        }

        public async Task<IActionResult> SaveReport(Guid reportId, double payPeriodHours, double estimatedPayPeriodHours, PeriodType payPeriodType, PeriodType calculationPeriodType)
        {
            ReportDto report = new(payPeriodHours, estimatedPayPeriodHours, payPeriodType, calculationPeriodType);
            if (reportId == Guid.Empty)
            {
                reportId = await _reportsService.AddReport(report);
                _logger.LogDebug($"New report: {reportId}");
            }
            else
            {
                report.ReportId = reportId;
                await _reportsService.UpdateReport(report);
                _logger.LogDebug($"Updated report: {reportId}");
            }
            return RedirectToAction("Index", new { reportId });
        }
    }
}
