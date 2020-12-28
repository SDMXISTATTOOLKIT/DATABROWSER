using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Specifications.Query;

namespace DataBrowser.Specifications.Dashboards
{
    public class DashboardByUserIdSpecification : BaseSpecification<Dashboard>
    {
        public DashboardByUserIdSpecification(int userId, bool withViews = false, bool withNodes = false)
            : base(b => b.UserFk == userId)
        {
            if (withViews)
            {
                AddInclude("Views");
            }
            if (withNodes)
            {
                AddInclude("Nodes");
            }
            AddInclude("Title.TransatableItemValues");
            ApplyOrderBy(p => p.Weight);
        }
    }
}
