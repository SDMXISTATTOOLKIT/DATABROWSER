using System.Collections.Generic;
using DataBrowser.Interfaces.ModelViews;

namespace DataBrowser.Query.Dashboards.ModelView
{
    public class DashboardViewModel
    {
        public int DashboardId { get; set; }
        public Dictionary<string, string> Title { get; set; }
        public string DashboardConfig { get; set; }
        public List<int> ViewIds { get; set; }
        public List<int> NodeIds { get; set; }
        public int? HubId { get; set; }
        public int Weight { get; set; }
        public Dictionary<int, ViewTemplateViewModel> Views { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FilterLevels { get; set; }
    }
}