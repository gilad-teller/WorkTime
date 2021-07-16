using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using WorkTimeCommon;

namespace WorkTimeDB
{
    public class WorkTimeDbContext : DbContext
    {
        public DbSet<Job> Jobs { get; set; }
        public DbSet<OffDay> OffDays { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Report> Reports { get; set; }

        public WorkTimeDbContext(DbContextOptions<WorkTimeDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>().ToTable("Jobs");
            modelBuilder.Entity<OffDay>().ToTable("OffDays");
            modelBuilder.Entity<Shift>().ToTable("Shifts");
            modelBuilder.Entity<Report>().ToTable("Reports");
        }
    }

    public class Job
    {
        public Guid JobId { get; set; }
        public string Name { get; set; }
        public string WeekendDays { get; set; }
        public ICollection<OffDay> OffDays { get; set; }
        public ICollection<Shift> Shifts { get; set; }
    }

    public class OffDay
    {
        public Guid OffDayId { get; set; }
        public DateTime Date { get; set; }
        public OffDayType OffDayType { get; set; }
        public Guid JobId { get; set; }
        public Job Job { get; set; }
    }

    public class Shift
    {
        public Guid ShiftId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Guid JobId { get; set; }
        public Job Job { get; set; }
    }

    public class Report
    {
        public Guid ReportId { get; set; }
        public double PayPeriodHours { get; set; }
        public double EstimatedPayPeriodHours { get; set; }
        public PeriodType PayPeriodType { get; set; }
        public PeriodType CalculationPeriodType { get; set; }
    }
}
