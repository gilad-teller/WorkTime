using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTimeDB;
using WorkTimeLogic.Models;

namespace WorkTimeLogic
{
    public interface IReportsService
    {
        public Task<IEnumerable<ReportDto>> GetReports();
        public Task<ReportDto> GetReportById(Guid repoortId);
        public Task<Guid> AddReport(ReportDto report);
        public Task<bool> UpdateReport(ReportDto report);
    }

    public class ReportsDbService : IReportsService
    {
        private readonly WorkTimeDbContext _context;
        private readonly ILogger<ReportsDbService> _logger;

        public ReportsDbService(WorkTimeDbContext context, ILogger<ReportsDbService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ReportDto>> GetReports()
        {
            List<Report> dbReports = await _context.Reports.ToListAsync();
            return dbReports.Select(x => new ReportDto(x));
        }

        public async Task<ReportDto> GetReportById(Guid repoortId)
        {
            try
            {
                Report dbReport = await _context.Reports.FirstOrDefaultAsync(x => x.ReportId == repoortId);
                if (dbReport == null)
                {
                    _logger.LogWarning($"Can't find report {repoortId}");
                    return null;
                }
                return new ReportDto(dbReport);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get report");
                return null;
            }
        }

        public async Task<Guid> AddReport(ReportDto report)
        {
            try
            {
                Report dbReport = new Report()
                {
                    PayPeriodHours = report.PayPeriodHours,
                    EstimatedPayPeriodHours = report.EstimatedPayPeriodHours,
                    PayPeriodType = report.PayPeriodType,
                    CalculationPeriodType = report.CalculationPeriodType
                };
                await _context.Reports.AddAsync(dbReport);
                await _context.SaveChangesAsync();
                return dbReport.ReportId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add report");
                return Guid.Empty;
            }
        }

        public async Task<bool> UpdateReport(ReportDto report)
        {
            try
            {
                Report dbReport = await _context.Reports.FirstOrDefaultAsync(x => x.ReportId == report.ReportId);
                if (dbReport == null)
                {
                    _logger.LogWarning($"Can't find report {report.ReportId}");
                    return false;
                }
                dbReport.PayPeriodHours = report.PayPeriodHours;
                dbReport.EstimatedPayPeriodHours = report.EstimatedPayPeriodHours;
                dbReport.PayPeriodType = report.PayPeriodType;
                dbReport.CalculationPeriodType = report.CalculationPeriodType;
                return await _context.SaveChangesAsync() == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update report");
                return false;
            }
        }
    }
}
