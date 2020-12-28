using System.Security.Claims;

namespace DataBrowser.Interfaces.Constants
{
    public static class PermissionType
    {
        public enum NodePermission
        {
            Full,
            ManageCache,
            ManageTemplate,
            ManageConfig,
            ManageView,
            ViewPrivateData
        }

        public const string PermissionCroosNodeType = "Permission_CroosNode";
        public const string PermissionSingleNodeType = "Permission_SingleNode_";

        public static string GetPermissionNameForSingleNode(int nodeId)
        {
            return $"{PermissionSingleNodeType}{nodeId}";
        }

        public static string GetPermissionNameForCroosNode()
        {
            return $"{PermissionCroosNodeType}";
        }

        public static string ConvertClaimToPermissionViewModel(Claim claim)
        {
            if (claim == null) return null;
            return $"{claim.Value}{claim.Type.Replace("Permission", "")}";
        }
    }
}