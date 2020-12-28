using System.Collections.Generic;

namespace EndPointConnector.JsonStatParser.Model.JsonStat
{
    public class JsonStatUtils
    {

        public static int? RowMajorOrderWithnullables(List<int> size, int?[] coords)
        {
            // row-major position algorithm
            var numberOfDimensions = size.Count;
            var valuePosition = 0;
            var multiplier = 1;

            for (var i = 0; i < numberOfDimensions; i++) {
                if (size[numberOfDimensions - 1 - i] == 0 || coords[numberOfDimensions - 1 - i].HasValue == false) {
                    continue; // il valore viene ignorato
                }

                multiplier *= i > 0 ? size[numberOfDimensions - i] : 1;
                valuePosition += multiplier * coords[numberOfDimensions - 1 - i].Value;
            }

            return valuePosition;
        }

        public static int RowMajorOrder(List<int> size, int[] coords)
        {
            if (coords == null) {
                return -1;
            }

            // row-major position algorithm
            var numberOfDimensions = size.Count;
            var valuePosition = 0;
            var multiplier = 1;

            for (var i = 0; i < numberOfDimensions; i++) {
                multiplier *= i > 0 ? size[numberOfDimensions - i] : 1;
                valuePosition += multiplier * coords[numberOfDimensions - 1 - i];
            }

            return valuePosition;
        }

    }
}