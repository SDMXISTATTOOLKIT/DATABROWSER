using System.Collections.Generic;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Serialization;

namespace WSHUB.Models.Request
{
    public class DashboardCreateModelView
    {
        public int DashboardId { get; set; }
        public Dictionary<string, string> Title { get; set; }
        public string DashboardConfig { get; set; }
        public List<int> ViewIds { get; set; }
        public List<int> NodeIds { get; set; }
        public int UserId { get; set; }
        public int? HubId { get; set; }
        public int Weight { get; set; }
        public string FilterLevels { get; set; }

        public DashboardDto ConvertToDto()
        {
            return new DashboardDto
            {
                DashboardId = DashboardId,
                Title = Title,
                DashboardConfig = string.IsNullOrWhiteSpace(DashboardConfig)
                    ? new DashboardDto.DashboardConfigItem[1][]
                    : DataBrowserJsonSerializer
                        .DeserializeObject<DashboardDto.DashboardConfigItem[][]>(DashboardConfig),
                ViewIds = ViewIds,
                NodeIds = NodeIds,
                UserId = UserId,
                HubId = HubId,
                Weight = Weight,
                FilterLevels = FilterLevels
            };
        }
    }
}