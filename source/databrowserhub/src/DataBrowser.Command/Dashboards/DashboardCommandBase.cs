using System.Security.Claims;

namespace DataBrowser.Command.Dashboards
{
    public class DashboardCommandBase
    {
        public DashboardCommandBase(ClaimsPrincipal specificUser = null,
            bool filterByPermissionViewTemplate = false,
            bool checkWritePermission = false,
            bool checkReadPermion = false)
        {
            FilterByPermissionViewTemplate = filterByPermissionViewTemplate;
            SpecificUser = specificUser;
            CheckWritePermission = checkWritePermission;
            CheckReadPermion = checkReadPermion;
        }

        public bool FilterByPermissionViewTemplate { get; set; }
        public bool CheckWritePermission { get; set; }
        public bool CheckReadPermion { get; set; }
        public ClaimsPrincipal SpecificUser { get; set; }
    }
}