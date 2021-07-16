using System;
using System.Collections.Generic;
using System.Text;

namespace WorkTimeLogic.Models
{
    public class ShiftDto
    {
        public Guid ShiftId { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public TimeSpan TotalTime => EndTime - StartTime;

        public ShiftDto(Guid shiftId, DateTime startTime, DateTime endTime) : this(startTime, endTime)
        {
            ShiftId = shiftId;
        }
        public ShiftDto(DateTime startTime, DateTime endTime)
        {
            if (startTime >= endTime) throw new ArgumentException("Start time must be before end time", nameof(startTime));
            StartTime = startTime;
            EndTime = endTime;
        }

        public override bool Equals(object obj)
        {
            if (obj is ShiftDto shift)
            {
                return shift.StartTime == StartTime && shift.EndTime == EndTime;
            }
            return false;
        }

        public override int GetHashCode() => 7 * StartTime.GetHashCode() + 13 * EndTime.GetHashCode();

        public bool IsIntersects(ShiftDto shift) => !(shift.StartTime > EndTime || shift.EndTime < StartTime);

        public int CompareTo(object obj)
        {
            if (obj is ShiftDto shift)
            {
                return StartTime.CompareTo(shift.StartTime);
            }
            throw new ArgumentException("Can't compare to non shift", nameof(obj));
        }

        public override string ToString()
        {
            return $"{StartTime:yyyy-MM-ddTHH:mm} - {EndTime:yyyy-MM-ddTHH:mm}";
        }
    }
}
