using System.Security.Claims;

namespace DataBrowser.Query.Dashboards
{
    public class DashboardQueryBase
    {
        public DashboardQueryBase(bool filterByPermission = false,
            ClaimsPrincipal specificUser = null)
        {
            FilterByPermission = filterByPermission;
            SpecificUser = specificUser;
        }

        public bool FilterByPermission { get; set; }
        public ClaimsPrincipal SpecificUser { get; set; }
    }
}