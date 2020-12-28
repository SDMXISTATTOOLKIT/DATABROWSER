using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Specifications.Query;
using Microsoft.EntityFrameworkCore.Internal;

namespace DataBrowser.Specifications.Dashboards
{
    public class DashboardByUserIdOrPublicSpecification : BaseSpecification<Dashboard>
    {
        public DashboardByUserIdOrPublicSpecification(int userId, bool withViews = false, bool withNodes = false)
            : base(b => b.UserFk == userId ||
                        (b.HubFk.HasValue && b.HubFk.Value > 0) ||
                        (b.Nodes != null && b.Nodes.Any()))
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
