using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Specifications.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Specifications.Nodes
{
    public class NodePositionHighSpecification : BaseSpecification<Node>
    {
        public NodePositionHighSpecification()
            : base(b => true)
        {
            ApplyOrderByDescending(i => i.Order);
            ApplyPaging(0, 1);
        }
    }
}
