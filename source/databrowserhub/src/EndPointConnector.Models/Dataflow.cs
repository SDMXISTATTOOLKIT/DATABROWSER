using System.Collections.Generic;

namespace EndPointConnector.Models
{
    public class Dataflow : MainObject
    {
        public ArtefactRef DataStructureRef { get; set; }


        public Dictionary<string, string> CriteriaSelectionMode { get; set; }
        public Dictionary<string, List<FilterCriteria>> DefaultCodeSelected { get; set; }
        public Dictionary<string, List<string>> LayoutRows { get; set; }
        public Dictionary<string, List<string>> LayoutColumns { get; set; }
        public Dictionary<string, List<string>> LayoutRowSections { get; set; }
        public Dictionary<string, List<string>> LayoutFilter { get; set; }
        public Dictionary<string, List<string>> LayoutChartPrimaryDim { get; set; }
        public Dictionary<string, List<string>> LayoutChartSecondaryDim { get; set; }
        public Dictionary<string, List<string>> LayoutChartFilter { get; set; }
        public Dictionary<string, List<Criteria>> NotDisplay { get; set; }
        public Dictionary<string, string> MetadataUrl { get; set; }
        public Dictionary<string, string> VirtualEndPoint { get; set; }
        public Dictionary<string, List<string>> Keywords { get; set; }
        public Dictionary<string, int?> DecimalNumber { get; set; }
        public Dictionary<string, string> DataflowSource { get; set; }
        public Dictionary<string, string> DefaultView { get; set; }
        public Dictionary<string, string> DecimalSeparator { get; set; }
        public Dictionary<string, string> EmptyCellPlaceHolder { get; set; }
        public Dictionary<string, int?> MaxCell { get; set; }
        public Dictionary<string, int?> MaxObservations { get; set; }
        public Dictionary<string, List<string>> GeoIds { get; set; }
        public Dictionary<string, List<string>> AttachedDataFiles { get; set; }
        public Dictionary<string, List<string>> TerritorialDimensions { get; set; }
        public Dictionary<string, string> DefaultNote { get; set; }
        public Dictionary<string, string> DefaultPresentation { get; set; }
        public Dictionary<string, string> LastUpdate { get; set; }
        public Dictionary<string, string> DataflowCatalogType { get; set; }
        public Dictionary<string, bool> NonProductionDataflow { get; set; }
        public Dictionary<string, bool> HiddenFromCatalog { get; set; }

        //VirtualSection
        public DataflowType DataflowType { get; set; }
        public string VirtualEndPointSoapV20 { get; set; }
        public string VirtualEndPointSoapV21 { get; set; }
        public string VirtualEndPointRest { get; set; }
        public VirtualType VirtualType { get; set; }
        public string VirtualSource { get; set; }
        public DerivedDataflowdata DerivedDataflowData { get; set; }

        public class DerivedDataflowdata
        {
            public string NodeCode { get; set; }
            public string EndPointUrl { get; set; }
        }
    }

    public enum DataflowType
    {
        IsReal,
        IsVirtual,
        DerivedFromVirtual
    }

    public enum VirtualType
    {
        Node,
        SoapV21,
        SoapV20,
        Rest
    }
}