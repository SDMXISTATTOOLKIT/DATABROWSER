using System.Security.Claims;
using DataBrowser.Domain.Dtos;
using DataBrowser.Interfaces.Authentication;
using Microsoft.Extensions.Logging;

namespace DataBrowser.AC.Utility.Helpers
{
    public static class ViewTemplateHelper
    {
        public static bool HavePermission(bool filterByPermissionNodeView,
            bool filterByPermissionNodeTemplate,
            int filterBySpecificNodeId,
            ClaimsPrincipal filterBySpecificUser,
            ViewTemplateDto viewTemplate,
            IFilterTemplate filterTemplate,
            IFilterView filterView,
            ILogger logger)
        {
            if (viewTemplate.Type == ViewTemplateType.View)
                return HaveViewPermission(filterByPermissionNodeView,
                    filterBySpecificNodeId,
                    filterBySpecificUser,
                    viewTemplate,
                    filterView,
                    logger);

            return HaveTemplatePermission(filterByPermissionNodeTemplate,
                filterBySpecificNodeId,
                filterBySpecificUser,
                viewTemplate,
                filterTemplate,
                logger);
        }

        public static bool HaveViewPermission(bool filterByPermissionNodeView,
            int filterBySpecificNodeId,
            ClaimsPrincipal filterBySpecificUser,
            ViewTemplateDto view,
            IFilterView filterView,
            ILogger logger)
        {
            var havePermission = true;

            if (view.Type != ViewTemplateType.View) return false;
            if (view.NodeId != filterBySpecificNodeId &&
                filterBySpecificNodeId != -1)
                return false;
            if (filterView != null)
            {
                havePermission = havePermission && (!filterByPermissionNodeView ||
                                                    filterView.CheckPermission(view, filterBySpecificUser));
                logger.LogDebug(
                    $"FilterByPermissionNodeView {filterByPermissionNodeView} and result is: {havePermission}");
            }

            return havePermission;
        }

        public static bool HaveTemplatePermission(bool filterByPermissionNodeTemplate,
            int filterBySpecificNodeId,
            ClaimsPrincipal filterBySpecificUser,
            ViewTemplateDto template,
            IFilterTemplate filterTemplate,
            ILogger logger)
        {
            var havePermission = true;

            if (template.NodeId != filterBySpecificNodeId &&
                filterBySpecificNodeId != -1)
                return false;
            if (template.Type == ViewTemplateType.Template &&
                filterTemplate != null)
            {
                havePermission = havePermission && (!filterByPermissionNodeTemplate ||
                                                    filterTemplate.CheckPermission(template, filterBySpecificUser));
                logger.LogDebug(
                    $"FilterByPermissionNodeTemplate {filterByPermissionNodeTemplate} and result is: {havePermission}");
            }

            return havePermission;
        }
    }
}