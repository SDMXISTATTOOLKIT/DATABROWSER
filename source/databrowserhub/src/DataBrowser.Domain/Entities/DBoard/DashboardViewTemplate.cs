using DataBrowser.Domain.Entities.SeedWork;

namespace DataBrowser.Domain.Entities.DBoard
{
    public class DashboardViewTemplate : Entity
    {
        public int DashboardId { get; set; }
        public int ViewTemplateId { get; set; }
        public int Weight { get; set; }

        // navigation properties
        public virtual Dashboard Dashboard { get; set; }
    }
}
