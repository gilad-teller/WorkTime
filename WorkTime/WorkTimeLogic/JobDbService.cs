using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTimeCommon;
using WorkTimeDB;
using WorkTimeLogic.Models;
using Microsoft.EntityFrameworkCore;

namespace WorkTimeLogic
{
    public interface IJobService
    {
        public Task<IEnumerable<JobDto>> GetJobs();
        public Task<JobDto> GetJobById(Guid jobId);
        public Task<Guid> TryAddJob(JobDto job);
        public Task<bool> TryUpdateJob(JobDto job);
        public Task<bool> TryAddShift(Guid jobId, ShiftDto shift);
        public Task<bool> TryUpdateShift(ShiftDto shift);
        public Task<bool> TryAddOffDay(Guid jobId, DateTime date);
        public Task<bool> TryAddHalfDay(Guid jobId, DateTime date);
    }

    public class JobDbService : IJobService
    {
        private readonly WorkTimeDbContext _context;
        private readonly ILogger<JobDbService> _logger;

        public JobDbService(WorkTimeDbContext context, ILogger<JobDbService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<JobDto>> GetJobs()
        {
            List<Job> dbJobs = await _context.Jobs.ToListAsync();
            return dbJobs.Select(x => new JobDto(x));
        }

        public async Task<JobDto> GetJobById(Guid jobId)
        {
            try
            {
                Job dbJob = await _context.Jobs.Include(x => x.Shifts).Include(y => y.OffDays).FirstOrDefaultAsync(z => z.JobId == jobId);
                if (dbJob == null)
                {
                    return null;
                }
                JobDto job = new JobDto(dbJob);
                return job;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get job by id {jobId}");
                return null;
            }
        }

        public async Task<Guid> TryAddJob(JobDto job)
        {
            try
            {
                Job dbJob = new Job()
                {
                    JobId = job.JobId,
                    Name = job.Name,
                    WeekendDays = string.Join(",", job.WeekendDays)
                };
                await _context.Jobs.AddAsync(dbJob);
                
                List<Shift> shiftsToAdd = new List<Shift>();
                foreach (ShiftDto shift in job.Shifts)
                {
                    Shift dbShift = new Shift()
                    {
                        JobId = dbJob.JobId,
                        StartTime = shift.StartTime,
                        EndTime = shift.EndTime
                    };
                    shiftsToAdd.Add(dbShift);
                }
                if (shiftsToAdd.Count > 0)
                {
                    await _context.Shifts.AddRangeAsync(shiftsToAdd);
                }

                List<OffDay> offDaysToAdd = new List<OffDay>();
                foreach (DateTime offDay in job.OffDays)
                {
                    OffDay dbOffDay = new OffDay()
                    {
                        Date = offDay.Date,
                        JobId = dbJob.JobId,
                        OffDayType = OffDayType.Full
                    };
                    offDaysToAdd.Add(dbOffDay);
                }
                foreach (DateTime halfDay in job.HalfDays)
                {
                    OffDay dbOffDay = new OffDay()
                    {
                        Date = halfDay.Date,
                        JobId = dbJob.JobId,
                        OffDayType = OffDayType.Half
                    };
                    offDaysToAdd.Add(dbOffDay);
                }
                if (offDaysToAdd.Count > 0)
                {
                    await _context.OffDays.AddRangeAsync(offDaysToAdd);
                }

                await _context.SaveChangesAsync();
                return dbJob.JobId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add job");
                return Guid.Empty;
            }
        }

        public async Task<bool> TryUpdateJob(JobDto job)
        {
            try
            {
                Job dbJob = await _context.Jobs.FirstOrDefaultAsync(x => x.JobId == job.JobId);
                if (dbJob == null)
                {
                    _logger.LogWarning($"Can't find job {job.JobId}");
                    return false;
                }
                dbJob.Name = job.Name;
                dbJob.WeekendDays = string.Join(",", job.WeekendDays);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add job");
                return false;
            }
        }

        public async Task<bool> TryAddShift(Guid jobId, ShiftDto shift)
        {
            try
            {
                Shift dbShift = new Shift()
                {
                    ShiftId = shift.ShiftId,
                    JobId = jobId,
                    StartTime = shift.StartTime,
                    EndTime = shift.EndTime
                };
                await _context.Shifts.AddAsync(dbShift);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add shift");
                return false;
            }
        }

        public async Task<bool> TryUpdateShift(ShiftDto shift)
        {
            try
            {
                Shift dbShift = await _context.Shifts.FirstOrDefaultAsync(x => x.ShiftId == shift.ShiftId);
                if (dbShift == null)
                {
                    _logger.LogWarning($"Can't find shift {shift.ShiftId}");
                    return false;
                }
                dbShift.StartTime = shift.StartTime;
                dbShift.EndTime = shift.EndTime;
                return await _context.SaveChangesAsync() == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update shift");
                return false;
            }
        }

        public async Task<bool> TryAddOffDay(Guid jobId, DateTime date)
        {
            try
            {
                await _context.OffDays.AddAsync(new OffDay()
                {
                    JobId = jobId,
                    Date = date.Date,
                    OffDayType = OffDayType.Full
                });
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add off day");
                return false;
            }
        }

        public async Task<bool> TryAddHalfDay(Guid jobId, DateTime date)
        {
            try
            {
                await _context.OffDays.AddAsync(new OffDay()
                {
                    JobId = jobId,
                    Date = date.Date,
                    OffDayType = OffDayType.Half
                });
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add half day");
                return false;
            }
        }
    }
}
