using DataBrowser.Domain.Entities;
using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Specifications.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Specifications.Hubs
{
    public class HubByIdWithAllDataSpecification : BaseSpecification<Hub>
    {

        public HubByIdWithAllDataSpecification(int hubId, bool excludeTitle = false,
                                                    bool excludeSlogan = false, bool excludeDescription = false)
            : base(b => hubId == -1 || b.HubId == hubId)
        {
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
        }

    }
}
