using System.Collections.Generic;

namespace EndPointConnector.Models
{
    public class Dsd : MainObject
    {
        public List<Dimension> Dimensions { get; set; }
        public PrimaryMeasure PrimaryMeasure { get; set; }

        public Dictionary<string, List<FilterCriteria>> DefaultCodeSelected { get; set; }
        public Dictionary<string, List<string>> LayoutRows { get; set; }
        public Dictionary<string, List<string>> LayoutColumns { get; set; }
        public Dictionary<string, List<string>> LayoutRowSections { get; set; }
        public Dictionary<string, List<string>> LayoutChartPrimaryDim { get; set; }
        public Dictionary<string, List<string>> LayoutChartSecondaryDim { get; set; }
        public Dictionary<string, List<string>> LayoutChartFilter { get; set; }
        public Dictionary<string, List<Criteria>> NotDisplay { get; set; }
        public Dictionary<string, int?> MaxObservations { get; set; }
        public Dictionary<string, List<string>> GeoIds { get; set; }
    }
}