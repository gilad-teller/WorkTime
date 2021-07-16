using System;
using System.Collections.Generic;
using System.Text;
using WorkTimeCommon;
using WorkTimeDB;

namespace WorkTimeLogic.Models
{
    public class ReportDto
    {
        public Guid ReportId { get; set; }
        public double PayPeriodHours { get; set; }
        public double EstimatedPayPeriodHours { get; set; }
        public PeriodType PayPeriodType { get; set; }
        public PeriodType CalculationPeriodType { get; set; }

        public ReportDto(double payPeriodHours, double estimatedPayPeriodHours, PeriodType payPeriodType, PeriodType calculationPeriodType) 
        {
            if (payPeriodType != calculationPeriodType && payPeriodType != PeriodType.Daily)
            {
                throw new ArgumentException($"{nameof(payPeriodType)} must be the same as {nameof(calculationPeriodType)} or {PeriodType.Daily}", nameof(payPeriodType));
            }
            PayPeriodHours = payPeriodHours;
            EstimatedPayPeriodHours = estimatedPayPeriodHours;
            PayPeriodType = payPeriodType;
            CalculationPeriodType = calculationPeriodType;
        }

        public ReportDto(Report dbReport)
        {
            ReportId = dbReport.ReportId;
            PayPeriodHours = dbReport.PayPeriodHours;
            EstimatedPayPeriodHours = dbReport.EstimatedPayPeriodHours;
            PayPeriodType = dbReport.PayPeriodType;
            CalculationPeriodType = dbReport.CalculationPeriodType;
        }

        public override string ToString()
        {
            return $"{PayPeriodHours}/{PayPeriodType} => {CalculationPeriodType}";
        }
    }
}
