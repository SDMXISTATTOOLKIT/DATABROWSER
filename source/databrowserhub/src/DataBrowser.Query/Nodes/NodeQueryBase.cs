using System.Security.Claims;

namespace DataBrowser.Query.Nodes
{
    public abstract class NodeQueryBase
    {
        protected NodeQueryBase(bool filterByPermissionNodeConfig = false,
            bool filterByPermissionNodeView = false,
            bool filterByPermissionNodeTemplate = false,
            bool filterByPermissionNodeCache = false,
            bool filterIsInAnd = true,
            ClaimsPrincipal filterBySpecificUser = null)
        {
            FilterByPermissionNodeConfig = filterByPermissionNodeConfig;
            FilterByPermissionNodeView = filterByPermissionNodeView;
            FilterByPermissionNodeTemplate = filterByPermissionNodeTemplate;
            FilterByPermissionNodeCache = filterByPermissionNodeCache;
            FilterBySpecificUser = filterBySpecificUser;
            FilterIsInAnd = filterIsInAnd;
        }

        public bool FilterByPermissionNodeConfig { get; set; }
        public bool FilterByPermissionNodeView { get; set; }
        public bool FilterByPermissionNodeTemplate { get; set; }
        public bool FilterByPermissionNodeCache { get; set; }
        public bool FilterIsInAnd { get; set; }
        public ClaimsPrincipal FilterBySpecificUser { get; set; }
    }
}