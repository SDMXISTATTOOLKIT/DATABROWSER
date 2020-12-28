using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Dtos
{
    public class NodeDto
    {
        public int NodeId { get; set; }
        public bool Active { get; set; }
        public bool Default { get; set; }
        public string Agency { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Logo { get; set; }
        public string EndPoint { get; set; }
        public int Order { get; set; }
        public bool EnableHttpAuth { get; set; }
        public string AuthHttpUsername { get; set; }
        public string AuthHttpPassword { get; set; }
        public string AuthHttpDomain { get; set; }
        public bool EnableProxy { get; set; }
        public bool UseProxySystem { get; set; }
        public string ProxyAddress { get; set; }
        public int ProxyPort { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
        public string BackgroundMediaURL { get; set; }
        public string EmptyCellDefaultValue { get; set; }
        public string DefaultView { get; set; }
        public bool ShowDataflowUncategorized { get; set; }
        public bool ShowDataflowNotInProduction { get; set; }
        public string CriteriaSelectionMode { get; set; }
        public List<string> LabelDimensionTerritorials { get; set; }
        public List<string> LabelDimensionTemporals { get; set; }
        public List<string> CategorySchemaExcludes { get; set; }
        public int? DecimalNumber { get; set; }
        public int ShowCategoryLevels { get; set; }
        public string EndPointFormatSupported { get; set; }
        public string CatalogNavigationMode { get; set; }
        public int? TtlDataflow { get; set; }
        public int? TtlCatalog { get; set; }

        public Dictionary<string, string> Title { get; set; }
        public Dictionary<string, string> Slogan { get; set; }
        public Dictionary<string, string> Description { get; set; }
        public Dictionary<string, string> DecimalSeparator { get; set; }

        public List<ExtraDto> Extras { get; set; }

        public List<int> DashboardIds { get; set; }
    }
}
