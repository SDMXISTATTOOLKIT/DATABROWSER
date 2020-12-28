using DataBrowser.Domain.Serialization;
using System.Collections.Generic;

namespace DataBrowser.Domain.Dtos
{
    public class DashboardDto
    {
        public int DashboardId { get; set; }
        public Dictionary<string, string> Title { get; set; }
        public DashboardConfigItem[][] DashboardConfig { get; set; }
        public List<int> ViewIds { get; set; }
        public List<int> NodeIds { get; set; }
        public int UserId { get; set; }
        public int? HubId { get; set; }
        public int Weight { get; set; }
        public string FilterLevels { get; set; }

        public string ConvertoDashboardConfigToText()
        {
            if (DashboardConfig == null ||
                DashboardConfig.Length == 0)
            {
                return "[]";
            }
            return DataBrowserJsonSerializer.SerializeObject(DashboardConfig);
        }

        public class DashboardConfigItem
        {
            public string Type { get; set; }
            public object Value { get; set; }
            public int WidthPercentage { get; set; }
            public bool ShowTitle { get; set; }
            public bool EnableFilters { get; set; }
            public string FilterDimension { get; set; }
            public string Extra { get; set; }
        }

        
    }
}
