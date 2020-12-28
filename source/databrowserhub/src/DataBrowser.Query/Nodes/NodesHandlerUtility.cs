using DataBrowser.Interfaces.Authentication;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Query.Nodes
{
    public static class NodesHandlerUtility
    {
        public static bool CheckPermissionNode(NodeQueryBase request, int nodeId, IFilterNode filterNode,
            ILogger logger)
        {
            if (!request.FilterByPermissionNodeConfig &&
                !request.FilterByPermissionNodeCache &&
                !request.FilterByPermissionNodeTemplate &&
                !request.FilterByPermissionNodeView)
                return true;

            var permissionConfig = request.FilterIsInAnd;
            var permissionCache = request.FilterIsInAnd;
            var permissionView = request.FilterIsInAnd;
            var permissionTemplate = request.FilterIsInAnd;

            if (request.FilterByPermissionNodeConfig)
                permissionConfig = filterNode.CheckPermissionNodeManageConfig(nodeId, request.FilterBySpecificUser);
            if (request.FilterByPermissionNodeCache)
                permissionCache = filterNode.CheckPermissionNodeManageCache(nodeId, request.FilterBySpecificUser);
            if (request.FilterByPermissionNodeTemplate)
                permissionTemplate = filterNode.CheckPermissionNodeManageTemplate(nodeId, request.FilterBySpecificUser);
            if (request.FilterByPermissionNodeView)
                permissionView = filterNode.CheckPermissionNodeManageView(nodeId, request.FilterBySpecificUser);

            logger.LogDebug($@"CheckPermissionNode is in And: {request.FilterIsInAnd}
FilterByPermissionNodeConfig {request.FilterByPermissionNodeConfig} and result is: {permissionConfig}
FilterByPermissionNodeCache {request.FilterByPermissionNodeCache} and result is: {permissionCache}
FilterByPermissionNodeTemplate {request.FilterByPermissionNodeTemplate} and result is: {permissionView}
FilterByPermissionNodeView {request.FilterByPermissionNodeView} and result is: {permissionTemplate}");

            if (request.FilterIsInAnd)
                return permissionConfig && permissionCache && permissionView && permissionTemplate;
            return permissionConfig || permissionCache || permissionView || permissionTemplate;
        }
    }
}