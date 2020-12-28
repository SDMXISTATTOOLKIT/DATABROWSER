using DataBrowser.Domain.Entities;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Specifications.Query;
using System;

namespace DataBrowser.Specifications.Nodes
{
    public class NodeByCodeSpecification : BaseSpecification<Node>
    {
        public NodeByCodeSpecification(string code, ExtraInclude include)
            : base(b => b.Code.ToLower() == code.ToLower())
        {
            if (include == ExtraInclude.ExtraWithTransaltion)
            {
                AddInclude("Extras.TransatableItem.TransatableItemValues");
            }
            else if (include == ExtraInclude.Extra)
            {
                AddInclude("Extras");
            }
        }

        public enum ExtraInclude
        {
            Nothing, Extra, ExtraWithTransaltion
        }
    }
}
