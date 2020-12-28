using System.Collections.Generic;
using DataBrowser.AC.Responses.Services;

namespace DataBrowser.Command.Dashboards.Model
{
    public class RemoveDashboardResult
    {
        public bool Deleted { get; set; }
        public List<NodeMinimalInfoDto> Nodes { get; set; }
        public bool AssignToHub { get; set; }
        public bool NotFound { get; set; }
    }
}