using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Specifications.Query;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

namespace DataBrowser.Specifications.Dashboards
{
    public class DashboardByNodeIdSpecification : BaseSpecification<Dashboard>
    {

        public DashboardByNodeIdSpecification(int nodeId, bool includeView)
            : base(b => b.Nodes.Any(i => i.NodeId == nodeId))
        {
            if (includeView)
            {
                AddInclude("Views");
            }
            AddInclude("Nodes");
            AddInclude("Title.TransatableItemValues");
            ApplyOrderBy(x => x.Nodes.FirstOrDefault(n => n.NodeId == nodeId).Weight);
        }
    }
}
