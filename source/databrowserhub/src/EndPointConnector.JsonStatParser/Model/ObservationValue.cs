using System;

namespace EndPointConnector.JsonStatParser.Model
{
    public struct ObservationValue
    {

        public double? Double;

        public string String;

        public static implicit operator ObservationValue(double doubleVal)
        {
            return new ObservationValue {Double = doubleVal};
        }

        public static implicit operator ObservationValue(double? doubleVal)
        {
            return new ObservationValue {Double = doubleVal};
        }

        public static implicit operator ObservationValue(string stringVal)
        {
            return new ObservationValue {String = stringVal};
        }


        public double? GetAsNullableDouble()
        {
            if (IsNull) {
                return null;
            }

            // numeric value
            if (Double.HasValue) {
                return Double.Value;
            }

            // string value (parse needed)
            if (!string.IsNullOrEmpty(String) && double.TryParse(String, out var parsedDouble)) {
                return parsedDouble;
            }

            return null;
        }

        public int? GetAsNullableInt()
        {
            if (IsNull) {
                return null;
            }

            var doubleValue = GetAsNullableDouble();

            int? result = null;

            if (doubleValue != null) {
                result = Convert.ToInt32(doubleValue.Value);
            }

            return result;
        }

        public bool IsNull => Double == null && String == null;

        public override string ToString()
        {
            if (String != null) {
                return String;
            }

            return "" + GetAsNullableDouble();
        }

    }
}