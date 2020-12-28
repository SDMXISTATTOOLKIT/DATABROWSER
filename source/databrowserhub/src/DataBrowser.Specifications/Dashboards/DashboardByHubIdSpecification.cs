using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Specifications.Query;

namespace DataBrowser.Specifications.Dashboards
{
    public class DashboardByHubIdSpecification : BaseSpecification<Dashboard>
    {
        public DashboardByHubIdSpecification(int hubId)
            : base(b => b.HubFk != null && (hubId < 0 || b.HubFk == hubId))
        {
            AddInclude("Views");
            AddInclude("Title.TransatableItemValues");
            ApplyOrderBy(p => p.Weight);
        }
    }
}
