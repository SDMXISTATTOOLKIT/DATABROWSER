using System.Collections.Generic;

namespace EndPointConnector.Models.Dto
{
    public class NodeCatalogDto
    {
        public List<CategoryGroupDto> CategoryGroups;
        public Dictionary<string, DatasetDto> DatasetMap;
        public List<DatasetDto> DatasetUncategorized;
    }

    public class CategoryGroupDto
    {
        public string Id { get; set; }
        public Dictionary<string, string> Labels { get; set; }
        public Dictionary<string, string> Descriptions { get; set; }
        public List<ExtraValueDto> Extras { get; set; }
        public List<CategoryDto> Categories { get; set; }
    }

    public class CategoryDto
    {
        public string Id { get; set; }
        public Dictionary<string, string> Labels { get; set; }
        public Dictionary<string, string> Descriptions { get; set; }
        public List<CategoryDto> ChildrenCategories { get; set; }
        public List<ExtraValueDto> Extras { get; set; }
        public HashSet<string> DatasetIdentifiers { get; set; }
    }

    public class ExtraValueDto
    {
        public string Key { get; set; }
        public Dictionary<string, string> Values { get; set; }
        public string Type { get; set; }
        public bool IsPublic { get; set; }
    }

    public class DatasetDto
    {
        public string Identifier { get; set; }
        public Dictionary<string, string> Titles { get; set; }
        public Dictionary<string, string> Descriptions { get; set; }
        public Dictionary<string, string> Source { get; set; }
        public Dictionary<string, string> MetadataUrl { get; set; }
        public Dictionary<string, List<string>> Keywords { get; set; }
        public Dictionary<string, List<string>> AttachedDataFiles { get; set; }
        public Dictionary<string, string> DefaultNote { get; set; }
        public Dictionary<string, List<string>> LayoutFilter { get; set; }
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
    }
}