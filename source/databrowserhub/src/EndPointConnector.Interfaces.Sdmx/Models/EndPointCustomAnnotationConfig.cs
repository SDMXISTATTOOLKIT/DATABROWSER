using Org.Sdmxsource.Sdmx.Api.Constants;

namespace EndPointConnector.Interfaces.Sdmx.Models
{
    public class EndPointCustomAnnotationConfig
    {
        public string ORDER_CATEGORY { get; set; }
        public string ORDER_CODELIST { get; set; }
        public string DEFAULT { get; set; }
        public string LAYOUT_ROW { get; set; }
        public string LAYOUT_COLUMN { get; set; }
        public string LAYOUT_ROW_SECTION { get; set; }
        public string NOT_DISPLAYED { get; set; }
        public string METADATA_URL { get; set; }
        public string DATAFLOW_SOURCE { get; set; }
        public string CRITERIA_SELECTION { get; set; }
        public string VIRTUAL_DATAFLOW_NODE { get; set; }
        public string KEYWORDS { get; set; }
        public string DEFAULT_VIEW { get; set; }
        public string GEO_ID { get; set; }
        public string DATAFLOW_NOTES { get; set; }
        public string LAYOUT_FILTER { get; set; }
        public string LAYOUT_NUMBER_OF_DECIMALS { get; set; }
        public string LAYOUT_DECIMAL_SEPARATOR { get; set; }
        public string LAYOUT_EMPTY_CELL_PLACEHOLDER { get; set; }
        public string LAYOUT_MAX_TABLE_CELLS { get; set; }
        public string ATTACHED_DATA_FILES { get; set; }
        public string LAYOUT_DEFAULT_PRESENTATION { get; set; }
        public string LAST_UPDATE { get; set; }
        public string DATAFLOW_CATALOG_TYPE { get; set; }
        public string LAYOUT_CHART_PRIMARY_DIM { get; set; }
        public string LAYOUT_CHART_SECONDARY_DIM { get; set; }
        public string LAYOUT_CHART_FILTER { get; set; }


        public string GetOrderValueForType(SdmxStructureEnumType sdmxStructureType)
        {
            if (sdmxStructureType == SdmxStructureEnumType.CodeList)
                return ORDER_CODELIST;
            if (sdmxStructureType == SdmxStructureEnumType.Category) return ORDER_CATEGORY;
            return ORDER_CODELIST;
        }
    }
}