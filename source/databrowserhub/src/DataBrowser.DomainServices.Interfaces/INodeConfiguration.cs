using System;
using System.Collections.Generic;

namespace DataBrowser.DomainServices.Interfaces
{
    public interface INodeConfiguration
    {
        int NodeId { get; set; }
        string Code { get; set; }
        string EndPoint { get; set; }
        string Type { get; set; }
        bool EnableHttpAuth { get; set; }
        string HttpAuthUsername { get; set; }
        string HttpAuthPassword { get; set; }
        bool EnableProxy { get; set; }
        bool UseProxySystem { get; set; }
        string ProxyAddress { get; set; }
        int ProxyPort { get; set; }
        string ProxyUsername { get; set; }
        string ProxyPassword { get; set; }
        Dictionary<string, object> Extras { get; set; }
        bool ShowDataflowUncategorized { get; set; }
        string CriteriaSelectionMode { get; set; }
        string LabelDimensionTerritorial { get; set; }
        string LabelDimensionTemporal { get; set; }
        List<string> CategorySchemaExcludes { get; set; }
        string EndPointFormatSupported { get; set; }
        string EmptyCellDefaultValue { get; set; }
        int MaxObservationsAfterCriteria { get; set; }
        bool ShowDataflowNotInProduction { get; set; }
    }
}
