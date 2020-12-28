using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace EndPointConnector.JsonStatParser.Model.JsonStat.Extensions
{
    public class AttributesIndex
    {

        //attributes references [attr1-posx, attr2-posy, attr3-posz,...]
        [JsonProperty("dataSet", NullValueHandling = NullValueHandling.Ignore)]
        protected List<int?> DataSet { get; set; }

        //observation index [obs-index1,obs-index2,obs-index3, obs-index4] -> attributes references [attr1-posx, attr2-posy, attr3-posz,...]
        // OR 
        //observation index [ null     ,obs-index2,obs-index3,  null     ] -> attributes references [attr1-posx, attr2-posy, attr3-posz,...]
        //where null means "any value"
        [JsonProperty("series", NullValueHandling = NullValueHandling.Ignore)]
        protected List<SeriesIndexEntry> Series { get; set; }

        //observation position -> attributes references [attr1-posx, attr2-posy, attr3-posz,...]
        [JsonProperty("observation", NullValueHandling = NullValueHandling.Ignore)]
        protected Dictionary<int, int?[]> Observation { get; set; }

        public AttributesIndex()
        {
            Observation = new Dictionary<int, int?[]>();
            DataSet = new List<int?>();
            Series = new List<SeriesIndexEntry>();
        }

        public void AddObservationAttributeIndex(int observationIndex, int?[] attributepositions)
        {
            if (attributepositions == null || attributepositions.Length == 0) {
                return;
            }

            Observation[observationIndex] = attributepositions;
        }

        public void SetDataSetAttributeIndex(List<int?> attributepositions)
        {
            if (attributepositions == null || attributepositions.Count == 0) {
                return;
            }

            DataSet = attributepositions;
        }


        public void RemoveSeriesDimensionReference(int dimensionPosition)
        {
            foreach (var seriesAttr in Series) seriesAttr.ShrinkCoordinates(dimensionPosition);
        }

        public void AddSeriesAttributeIndex(int?[] index, List<int?> attributeposition)
        {
            if (index == null || attributeposition == null) {
                return;
            }

            //search for duplicates
            var existingValueIndex = Series.FindIndex(v => v.Coordinates.SequenceEqual(index));

            // no duplicate found
            if (existingValueIndex < 0) {
                Series.Add(new SeriesIndexEntry(index, attributeposition));
            }
            else {
                // set attribute index if null. 
                // existing attribute index = [null,null,0,1,null]
                // given attribute index = [7,null,null,null,null]
                // existing will become [7,null,0,1,null]
                var existingVal = Series[existingValueIndex];

                if (existingVal.Attributes.Count != attributeposition.Count) {
                    return;
                }

                for (var i = 0; i < existingVal.Attributes.Count; i++)
                    if (existingVal.Attributes[i] == null && attributeposition[i] != null) {
                        existingVal.Attributes[i] = attributeposition[i];
                    }
            }
        }

        public int?[] GetObservationAttributeIndex(int position)
        {
            return Observation.TryGetValue(position, out var result) ? result : null;
        }


        public List<int?> GetDatasetAttributeIndex()
        {
            return DataSet;
        }


        public List<SeriesIndexEntry> GetSeriesAttributeIndex(int?[] index)
        {
            var result = new List<SeriesIndexEntry>();

            foreach (var entry in Series) {
                var otherIndex = entry.Coordinates;
                var otherIndexMatches = true;

                if (index.Length < otherIndex.Length) {
                    continue;
                }

                if (otherIndex.Where((t, i) => t != null && t != index[i]).Any()) {
                    otherIndexMatches = false;
                }

                if (otherIndexMatches) {
                    result.Add(entry);
                }
            }

            return result.Count > 0 ? result : null;
        }

    }
}