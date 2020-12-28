using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Specifications.Query;
using System.Linq;

namespace DataBrowser.Specifications.Dashboards
{
    public class DashboardContainsViewIdSpecification : BaseSpecification<Dashboard>
    {
        public DashboardContainsViewIdSpecification(int viewId)
            : base(b => b.Views.Any(i=>i.ViewTemplateId == viewId))
        {
            AddInclude("Title.TransatableItemValues");
        }
    }
}
