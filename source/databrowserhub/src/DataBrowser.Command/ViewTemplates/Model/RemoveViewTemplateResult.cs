using System.Collections.Generic;
using DataBrowser.Domain.Dtos;

namespace DataBrowser.Command.ViewTemplates.Model
{
    public class RemoveViewTemplateResult
    {
        public bool Deleted { get; set; }
        public List<DashboardDto> Dashboards { get; set; }
        public bool NotFound { get; set; }
    }
}