using System;
using System.Collections.Generic;
using System.Text;

namespace EndPointConnector.Interfaces.Sdmx.Models
{
    public class DataflowDataRange
    {
        public string PeriodType { get; set; }
        public int RangePeriod { get; set; }
        public DateTime EndRange { get; set; }

        public DateTime CalcolateStartRangeFromPeriod()
        {
            switch (PeriodType.ToUpperInvariant())
            {
                case "A":
                    return EndRange.AddYears(-RangePeriod + 1);
                case "S":
                    return calcolateSemesterRange();
                case "Q":
                    return calcolateQuarterRange();
                case "M":
                    return EndRange.AddMonths(-RangePeriod + 1);
                case "D":
                    return EndRange.AddDays(-RangePeriod + 1);
            }



            return EndRange.AddYears(-RangePeriod);
        }

        private DateTime calcolateQuarterRange()
        {
            int endMount;
            if (EndRange.Month >= 1 &&
                EndRange.Month <= 3)
            {
                endMount = 3;
            }
            else if (EndRange.Month >= 4 &&
                EndRange.Month <= 6)
            {
                endMount = 6;
            }
            else if (EndRange.Month >= 7 &&
                EndRange.Month <= 9)
            {
                endMount = 9;
            }
            else //if (EndRange.Month >= 10 && EndRange.Month <= 12)
            {
                endMount = 12;
            }

            var endPeriod = new DateTime(EndRange.Year, endMount, 1).AddMonths(1).AddDays(-1);
            var startPeriod = new DateTime(endPeriod.Year, endPeriod.Month, 1).AddMonths(-2 + (-3 * (RangePeriod-1)));

            return startPeriod;
        }

        private DateTime calcolateSemesterRange()
        {
            int endMount;
            if (EndRange.Month >= 1 &&
                EndRange.Month <= 6)
            {
                endMount = 3;
            }
            else //if (EndRange.Month >= 7 && EndRange.Month <= 12)
            {
                endMount = 7;
            }

            var endPeriod = new DateTime(EndRange.Year, endMount, 1).AddMonths(1).AddDays(-1);
            var startPeriod = new DateTime(endPeriod.Year, endPeriod.Month, 1).AddMonths(-5 + (-6 * (RangePeriod - 1)));

            return startPeriod;
        }

    }
}
