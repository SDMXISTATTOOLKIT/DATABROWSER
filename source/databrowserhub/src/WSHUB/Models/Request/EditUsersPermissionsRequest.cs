using System.Collections.Generic;
using DataBrowser.Interfaces.Constants;

namespace WSHUB.Models.Request
{
    public class EditUsersPermissionsRequest
    {
        public int UserId { get; set; }
        public List<PermissionType.NodePermission> NodePermissions { get; set; }
    }
}