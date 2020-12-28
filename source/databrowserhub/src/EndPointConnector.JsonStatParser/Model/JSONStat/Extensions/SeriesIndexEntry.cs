using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.JsonStat.Extensions
{
    public class SeriesIndexEntry
    {

        [JsonProperty("coordinates", NullValueHandling = NullValueHandling.Ignore)]
        public int?[] Coordinates { get; protected set; }

        [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)]
        public List<int?> Attributes { get; set; }

        public SeriesIndexEntry(int?[] coordinates, List<int?> attributePositions)
        {
            Coordinates = coordinates ??
                          throw new Exception("Error. Series index entry must have not null coordinates.");
            Attributes = attributePositions;
        }

        public void ShrinkCoordinates(int dimensionPositionToRemove)
        {
            if (dimensionPositionToRemove >= Coordinates.Length) {
                throw new Exception(
                    $"Error. Cannot shrink series attribute coordinates. Size: {Coordinates.Length} IndexToRemove {dimensionPositionToRemove}");
            }

            var tmpList = Coordinates.ToList();
            tmpList.RemoveAt(dimensionPositionToRemove);
            Coordinates = tmpList.ToArray();
        }

    }
}