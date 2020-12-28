using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace EndPointConnector.JsonStatParser.StructureUtils.Sorting.WeightGenerators.TimePeriod
{
    public class TimeDimensionWeightGenerator : WeightGenerator
    {

        public bool YearsAtTheEnd { get; } = true;

        private static readonly DateTime YearZeroTime = new DateTime(1000, 1, 1, 0, 0, 0, 0);

        private static readonly Regex YearRegex =
            new Regex(@"(^\d\d\d\d$)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex SemesterRegex =
            new Regex(@"^(\d\d\d\d)(-(S)([1-2]))$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex QuarterRegex =
            new Regex(@"^(\d\d\d\d)(-(Q)([1-4]))$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex MonthRegex = new Regex(@"^(\d\d\d\d)(-((1[0-2])|(0?[1-9])))$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly ILogger<TimeDimensionWeightGenerator> _logger;

        //TODO: verificare la presenza di pesi duplicati
        private readonly Dictionary<string, double?> _weights;

        public TimeDimensionWeightGenerator(ILoggerFactory loggerFactory, IEnumerable<string> bannedCodes) : base(
            bannedCodes)
        {
            _weights = new Dictionary<string, double?>();
            _logger = loggerFactory.CreateLogger<TimeDimensionWeightGenerator>();
        }


        protected static double DateToMilliseconds(int year, int month = 12, int day = 31, int hour = 23, int min = 59,
            int sec = 59, int millis = 999)
        {
            var date = new DateTime(year, month, day, hour, min, sec, millis);

            return DateToMilliseconds(date);
        }

        protected static double DateToMilliseconds(DateTime date)
        {
            var result = date.ToUniversalTime().Subtract(YearZeroTime).TotalMilliseconds;

            return result;
        }


        public override double? GenerateWeight(string code)
        {
            if (_weights.ContainsKey(code)) {
                return _weights[code];
            }

            if (IsCodeBanned(code)) {
                _weights[code] = null;

                return null;
            }

            try {
                _weights[code] = TimePeriodCodeToWeight(code);
            }
            catch (Exception e) {
                _weights[code] = _weights.Count;
                _logger.LogError(e, "An error occurred while parsing a date value string");
            }

            return _weights[code];
        }


        protected double? TimePeriodCodeToWeight(string code)
        {
            //2000
            var match = YearRegex.Match(code);

            if (match.Success) {
                return !YearsAtTheEnd
                    ? DateToMilliseconds(int.Parse(match.Groups[1].Value), 1, 1, 0, 0, 0, 0)
                    : DateToMilliseconds(int.Parse(match.Groups[1].Value));
            }

            //2000-S1
            match = SemesterRegex.Match(code);

            if (match.Success) {
                var semester = int.Parse(match.Groups[4].Value);
                var month = 12;
                var day = 31;

                if (semester != 1) {
                    return DateToMilliseconds(int.Parse(match.Groups[1].Value), month, day, 23, 59, 59, 998);
                }

                month = 6;
                day = 30;

                return DateToMilliseconds(int.Parse(match.Groups[1].Value), month, day, 23, 59, 59, 998);
            }

            //2000-Q2
            match = QuarterRegex.Match(code);

            if (match.Success) {
                var quarter = int.Parse(match.Groups[4].Value);
                var month = 12;
                var day = 31;

                switch (quarter) {
                    case 1:
                        month = 3;

                        break;
                    case 2:
                        month = 6;
                        day = 30;

                        break;
                    case 3:
                        month = 9;
                        day = 30;

                        break;
                    case 4:
                        month = 12;

                        break;
                }

                return DateToMilliseconds(int.Parse(match.Groups[1].Value), month, day, 23, 59, 59, 997);
            }

            //2000-06
            match = MonthRegex.Match(code);

            if (match.Success) {
                var month = int.Parse(match.Groups[3].Value);

                return DateToMilliseconds(int.Parse(match.Groups[1].Value), month, 1, 0, 0, 0);
            }

            // ISO8601 2020-02-27T18:00:00
            try {
                var d = DateTime.Parse(code, null, DateTimeStyles.RoundtripKind);

                return DateToMilliseconds(d);
            }
            catch (Exception e) {
                throw new Exception($"Cannot parse TimeDimensionValue = '{code}'. Unknown format.", e);
            }
        }

    }
}