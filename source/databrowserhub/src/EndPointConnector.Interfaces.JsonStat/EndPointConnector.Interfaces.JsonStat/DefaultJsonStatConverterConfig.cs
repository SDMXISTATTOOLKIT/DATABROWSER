using System;
using System.Collections.Generic;
using System.Linq;

namespace EndPointConnector.Interfaces.JsonStat
{
    public class DefaultJsonStatConverterConfig : IFromSDMXToJsonStatConverterConfig
    {
        public List<string> TerritorialDimensionIds { get; set; }
        public List<string> TemporalDimensionIds { get; set; }
        public string OrderAnnotationId { get; set; }
        public string NotDisplayedAnnotationId { get; set; }
        public string GeoAnnotationId { get; set; }

        public DefaultJsonStatConverterConfig()
        {
            TerritorialDimensionIds = new List<string> ();
            TemporalDimensionIds = new List<string> {"TIME_PERIOD"};
            OrderAnnotationId = "ORDER";
            NotDisplayedAnnotationId = "NOT_DISPLAYED";
        }


        public static IFromSDMXToJsonStatConverterConfig GetNew()
        {
            return new DefaultJsonStatConverterConfig();
        }

        public bool ContainsTerritorialDimensionId(string id)
        {
            return TerritorialDimensionIds?.FirstOrDefault(x =>
                x.Equals(id, StringComparison.InvariantCultureIgnoreCase)) != null;
        }

        public bool ContainsTemporalDimensionId(string id)
        {
            return TemporalDimensionIds?.FirstOrDefault(x =>
                x.Equals(id, StringComparison.InvariantCultureIgnoreCase)) != null;
        }

        public bool IsValidOrderAnnotation(string annotation)
        {
            return OrderAnnotationId.Equals(annotation, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsValidNotDisplayedAnnotation(string annotation)
        {
            return NotDisplayedAnnotationId.Equals(annotation, StringComparison.InvariantCultureIgnoreCase);
        }


        public IFromSDMXToJsonStatConverterConfig Clone()
        {
            IFromSDMXToJsonStatConverterConfig clone = new DefaultJsonStatConverterConfig();
            clone.Merge(this);
            return clone;
        }


        public void Merge(IFromSDMXToJsonStatConverterConfig otherConfig)
        {
            TerritorialDimensionIds ??= new List<string>();
            foreach (var terrDimId in otherConfig.TerritorialDimensionIds)
            {
                if (!TerritorialDimensionIds.Contains(terrDimId))
                    TerritorialDimensionIds.Add(terrDimId);
            }

            TemporalDimensionIds ??= new List<string>();
            foreach (var temporalDimId in otherConfig.TemporalDimensionIds)
            {
                if (!TemporalDimensionIds.Contains(temporalDimId))
                {
                    TemporalDimensionIds.Add(temporalDimId);
                }
            }

            if (OrderAnnotationId == null && otherConfig.OrderAnnotationId != null)
            {
                OrderAnnotationId = otherConfig.OrderAnnotationId;
            }

            if (NotDisplayedAnnotationId == null && otherConfig.NotDisplayedAnnotationId != null)
            {
                NotDisplayedAnnotationId = otherConfig.NotDisplayedAnnotationId;
            }
        }

        public void AddTemporalDimensionId(string timeIdLabel)
        {
            if (string.IsNullOrEmpty(timeIdLabel))
            {
                return;
            }

            if (TemporalDimensionIds == null)
            {
                TemporalDimensionIds = new List<string>();
            }

            if (!TemporalDimensionIds.Contains(timeIdLabel))
            {
                TemporalDimensionIds.Add(timeIdLabel);
            }
        }

        public void AddTerritorialDimensionId(string terrLabel)
        {
            if (string.IsNullOrEmpty(terrLabel))
            {
                return;
            }

            TerritorialDimensionIds ??= new List<string>();

            if (!TerritorialDimensionIds.Contains(terrLabel))
            {
                TerritorialDimensionIds.Add(terrLabel);
            }
        }

    }
}