using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Specifications.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Specifications.Hubs
{
    public class HubWithDashboardSpecification : BaseSpecification<Hub>
    {
        public HubWithDashboardSpecification()
            : base(b => true)
        {
            AddInclude("Dashboards");
        }
    }
}
