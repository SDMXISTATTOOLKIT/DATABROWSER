using System.Collections.Generic;
using DataBrowser.Domain.Dtos;

namespace WSHUB.Models.Response
{
    public class NodeModelView
    {
        public int NodeId { get; set; }
        public bool Default { get; set; }
        public bool? Active { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string LogoURL { get; set; }
        public int Order { get; set; }
        public string BackgroundMediaURL { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slogan { get; set; }
        public string DecimalSeparator { get; set; }
        public int? DecimalNumber { get; set; }
        public string EmptyCellDefaultValue { get; set; }
        public string DefaultView { get; set; }
        public bool ShowDataflowUncategorized { get; set; }
        public string CriteriaSelectionMode { get; set; }
        public string CatalogNavigationMode { get; set; }
        public int ShowCategoryLevels { get; set; }
        public List<string> LabelDimensionTerritorials { get; set; }
        public List<string> LabelDimensionTemporals { get; set; }
        public List<string> CategorySchemaExcludes { get; set; }
        public string EndPointFormatSupported { get; set; }
        public List<ExtraModelView> Extras { get; set; }
        public List<Dashboard> Dashboards { get; set; }

        public void setHubValue(HubDto hubDto)
        {
            if (hubDto == null) return;
            DecimalSeparator = DecimalSeparator ?? hubDto.DecimalSeparator;
            DecimalNumber = DecimalNumber.HasValue ? DecimalNumber.Value : hubDto.DecimalNumber;
            EmptyCellDefaultValue = EmptyCellDefaultValue ?? hubDto.EmptyCellDefaultValue;
            DefaultView = DefaultView ?? hubDto.DefaultView;
        }

        public class Dashboard
        {
            public int Id { get; set; }
            public string Title { get; set; }
        }
    }
}