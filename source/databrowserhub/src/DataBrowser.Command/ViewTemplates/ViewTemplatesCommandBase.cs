using System;
using System.Security.Claims;

namespace DataBrowser.Command.ViewTemplates
{
    public class ViewTemplatesCommandBase
    {
        public ViewTemplatesCommandBase(int nodeId,
            bool filterByPermissionNodeView = false,
            bool filterByPermissionNodeTemplate = false,
            ClaimsPrincipal specificUser = null)
        {
            if (nodeId <= 0) throw new ArgumentNullException("NodeId");

            NodeId = nodeId;
            FilterByPermissionNodeView = filterByPermissionNodeView;
            FilterByPermissionNodeTemplate = filterByPermissionNodeTemplate;
            SpecificUser = specificUser;
        }

        public int NodeId { get; set; }
        public bool FilterByPermissionNodeView { get; set; }
        public bool FilterByPermissionNodeTemplate { get; set; }
        public ClaimsPrincipal SpecificUser { get; set; }
    }
}