using System;
using System.Collections.Generic;
using System.Text;

namespace EndPointConnector.Models
{
    public class Dataset
    {
        public Dataflow Dataflow { get; set; }
        public Dsd Dsd { get; set; }
        public string CriteriaMode { get; set; }
        public List<string> LayoutRows { get; set; }
        public List<string> LayoutColumns { get; set; }
        public List<string> LayoutRowSections { get; set; }
        public List<string> LayoutChartPrimaryDim { get; set; }
        public List<string> LayoutChartSecondaryDim { get; set; }
        public List<string> LayoutChartFilter { get; set; }
        public int DecimalNumber { get; set; }
        public string DecimalSeparator { get; set; }
        public long? MaxCell { get; set; }
        public long? MaxObservation { get; set; }
        public List<string> GeoIds { get; set; }
        public List<FilterCriteria> DefaultCodeSelected { get; set; }
        public string EmptyCellPlaceHolder { get; set; }
        public List<string> TerritorialDimensions { get; set; }
        public string DefaultView { get; set; }
        public List<Criteria> NotDisplay { get; set; }
        public string LastUpdate { get; set; }
    }
}
