using System.Collections.Generic;

namespace DataBrowser.Query.Dashboards.ModelView
{
    public class DashboardMinimalInfoViewModel
    {
        public int DashboardId { get; set; }
        public Dictionary<string, string> Title { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}