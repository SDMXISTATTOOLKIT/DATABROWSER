using System;
using System.Collections.Generic;
using System.Linq;

namespace EndPointConnector.Interfaces.JsonStat
{
    public class JsonStatConverterConfigImpl : DefaultJsonStatConverterConfig
    {
        public JsonStatConverterConfigImpl() : base()
        {
            TerritorialDimensionIds = new List<string>();
            TemporalDimensionIds = new List<string>();
            OrderAnnotationId = null;
            NotDisplayedAnnotationId = null;
        }

        public bool ContainsTerritorialDimensionId(string id)
        {
            return TerritorialDimensionIds?.FirstOrDefault(x =>
                x != null && x.Equals(id, StringComparison.InvariantCultureIgnoreCase)) != null;
        }

        public bool ContainsTemporalDimensionId(string id)
        {
            return TemporalDimensionIds?.FirstOrDefault(x =>
                x != null && x.Equals(id, StringComparison.InvariantCultureIgnoreCase)) != null;
        }

        public bool IsValidOrderAnnotation(string annotation)
        {
            return OrderAnnotationId != null &&
                   OrderAnnotationId.Equals(annotation, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsValidNotDisplayedAnnotation(string annotation)
        {
            return NotDisplayedAnnotationId != null &&
                   NotDisplayedAnnotationId.Equals(annotation, StringComparison.InvariantCultureIgnoreCase);
        }


    }
}