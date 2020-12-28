using DataBrowser.Domain.Entities;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Specifications.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Specifications.Nodes
{
    public class NodeByIdWithDataSpecification : BaseSpecification<Node>
    {

        public NodeByIdWithDataSpecification(int nodeId = -1, bool excludeTitle = false,
                                                    bool excludeSlogan = false, bool excludeDescription = false,
                                                    bool excludeExtras = false, bool excludeDecimalSeparator = false)
            : base(b => nodeId == -1 || b.NodeId == nodeId)
        {
            if (!excludeExtras)
            {
                AddInclude("Extras.TransatableItem.TransatableItemValues");
            }
            if (!excludeTitle)
            {
                AddInclude("Title.TransatableItemValues");
            }
            if (!excludeSlogan)
            {
                AddInclude("Slogan.TransatableItemValues");
            }
            if (!excludeDescription)
            {
                AddInclude("Description.TransatableItemValues");
            }
            if (!excludeDecimalSeparator)
            {
                AddInclude("DecimalSeparator.TransatableItemValues");
            }
            if (nodeId == -1)
            {
                ApplyOrderBy(i => i.Order);
            }

        }

    }
}
