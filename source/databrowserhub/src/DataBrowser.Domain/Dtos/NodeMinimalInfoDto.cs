using DataBrowser.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.AC.Responses.Services
{
    public class NodeMinimalInfoDto
    {
        public int NodeId { get; set; }
        public bool Default { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Logo { get; set; }
        public int Order { get; set; }
        public bool? Active { get; set; }
        public string BackgroundMediaURL { get; set; }
        public int ShowCategoryLevels { get; set; }
        public string CatalogNavigationMode { get; set; }
        public Dictionary<string, string> Title { get; set; }
        public Dictionary<string, string> Slogan { get; set; }
        public Dictionary<string, string> Description { get; set; }
        public List<ExtraDto> Extras { get; set; }
        public List<NodeDashboard> Dashboards { get; set; }
        public class NodeDashboard
        {
            public int Id { get; set; }
            public Dictionary<string, string> Titles { get; set; }
        }
    }
}
