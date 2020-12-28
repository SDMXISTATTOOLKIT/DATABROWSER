using System.Collections.Generic;
using System.Threading.Tasks;
using DataBrowser.Command.Dashboards;
using DataBrowser.Command.Dashboards.Model;
using DataBrowser.Domain.Serialization;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Constants;
using DataBrowser.Query.Dashboards;
using DataBrowser.Query.Dashboards.ModelView;
using DataBrowser.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WSHUB.Models.Request;
using WSHUB.Models.Response;

namespace WSHUB.Controllers
{
    public class DashboardController : ApiBaseController
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IRequestContext _requestContext;


        public DashboardController(ILogger<DashboardController> logger,
            IRequestContext requestContext,
            IMediatorService mediatorService)
            : base(mediatorService)
        {
            _logger = logger;
            _requestContext = requestContext;
        }

        /// <summary>
        ///     Create new Dashboard.
        /// </summary>
        /// <response code="201">Create Dashboard</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>int</returns>
        [HttpPost("/Dashboards")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.AuthenticatedUser)]
        public async Task<ActionResult> CreateDashboard([FromBody] DashboardCreateModelView dashboardRequest)
        {
            _logger.LogDebug("START CreateDashboard");

            var resultId = await CommandAsync(new CreateDashboardCommand(dashboardRequest.ConvertToDto(),
                filterByPermissionViewTemplate: true, checkWritePermission: true));

            var result = new ContentResult();
            result.ContentType = "application/text";
            result.Content = resultId > 0 ? resultId.ToString() : "Not found";
            result.StatusCode = resultId > 0 ? 201 : 404;
            return result;
        }

        /// <summary>
        ///     Edit an existing Dashboard.
        /// </summary>
        /// <returns>int</returns>
        [HttpPut("/Dashboards/{dashboardId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.AuthenticatedUser)]
        public async Task<ActionResult> EditDashboard(int dashboardId,
            [FromBody] DashboardCreateModelView dashboardRequest)
        {
            _logger.LogDebug("START EditDashboard");

            var editOk = await CommandAsync(new EditDashboardCommand(dashboardRequest.ConvertToDto(),
                filterByPermissionViewTemplate: true, checkWritePermission: true));

            var result = new ContentResult();
            result.ContentType = "application/text";
            result.Content = editOk ? null : "Not found";
            result.StatusCode = editOk ? 204 : 404;
            return result;
        }

        /// <summary>
        ///     Get all Dashboards for the current user.
        /// </summary>
        /// <response code="201">Get all Dashboards</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>List of Dashboard </returns>
        [HttpGet("/Dashboards")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DashboardMinimalInfoViewModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetDashboards()
        {
            _logger.LogDebug("START GetDashboards");

            var dtoList = await QueryAsync(new DashboardsMinimalInfoByUserOrPublicQuery(HttpContext.User));

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = DataBrowserJsonSerializer.SerializeObject(dtoList);
            result.StatusCode = 200;
            return result;
        }

        /// <summary>
        ///     Get Dashboard by id
        /// </summary>
        /// <param name="dashboardId"> the Dashboard id</param>
        /// <response code="201">Get Dashboard</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>Dashboard</returns>
        [HttpGet("/Dashboards/{DashboardId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DashboardViewModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetDashboard(int dashboardId)
        {
            _logger.LogDebug("START GetDashboard");

            var dashboardResult = await QueryAsync(new DashboardByIdQuery(dashboardId, true, filterByPermission: true,
                filterBySpecificUser: HttpContext.User));

            var result = new ContentResult();
            result.ContentType = dashboardResult != null ? "application/json" : "application/text";
            result.Content = dashboardResult != null
                ? DataBrowserJsonSerializer.SerializeObject(dashboardResult)
                : "Not found";
            result.StatusCode = dashboardResult != null ? 200 : 404;
            return result;
        }

        /// <summary>
        ///     Get all Hub Dashboards.
        /// </summary>
        /// <response code="201">Hub Dashboards</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>int</returns>
        [HttpGet("/Dashboards/Hub")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(List<DashboardViewModel>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetDashboardsAssignedToHub()
        {
            _logger.LogDebug("START GetDashboardsAssignedToHub");

            // -1 means a random hub from the repo 
            var resultList = await QueryAsync(new DashboardListByHubQuery(-1, true, HttpContext.User, true));

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = resultList != null
                ? DataBrowserJsonSerializer.SerializeObject(resultList)
                : DataBrowserJsonSerializer.SerializeObject(new List<DashboardViewModel>());
            result.StatusCode = 200;
            return result;
        }


        /// <summary>
        ///     Get all Node Dashboards.
        /// </summary>
        /// <response code="201">Node Dashboards</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>int</returns>
        [HttpGet("/Dashboards/Nodes/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DashboardViewModel>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetDashboardsAssignedToNode(int nodeId)
        {
            _logger.LogDebug("START GetDashboardsAssignedToNode");

            var resultList = await QueryAsync(new DashboardListByNodeIdQuery(nodeId, true, HttpContext.User, true));

            var result = new ContentResult();
            result.ContentType = resultList != null ? "application/json" : "application/text";
            result.Content = resultList != null
                ? DataBrowserJsonSerializer.SerializeObject(resultList)
                : DataBrowserJsonSerializer.SerializeObject(new List<DashboardViewModel>());
            result.StatusCode = 200;
            return result;
        }

        /// <summary>
        ///     Delete DashboardViews.
        /// </summary>
        /// <response code="204">Resource deleted.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns></returns>
        [HttpDelete("/Dashboards/{dashboardId}/Views")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.AuthenticatedUser)]
        public async Task<ActionResult> DeleteAllDashboardViews(int dashboardId)
        {
            _logger.LogDebug("START DeleteAllDashboardViews");
            var commandResult = await CommandAsync(new UnAssignAllDashboardNodeCommand(_requestContext.DashboardId));

            var result = new ContentResult();
            result.ContentType = "application/text";
            result.StatusCode = commandResult ? 204 : 404;
            return result;
        }

        /// <summary>
        ///     Delete Dashboard
        /// </summary>
        /// <response code="204">Resource deleted.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns></returns>
        [HttpDelete("/Dashboards/{dashboardId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(RemoveDashboardResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(DeleteGenericEntityModelView))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.AuthenticatedUser)]
        public async Task<ActionResult> DeleteDashboard(int dashboardId)
        {
            _logger.LogDebug("START DeleteDashboard");
            var commandResult = await CommandAsync(new RemoveDashboardCommand(_requestContext.DashboardId));

            var result = new ContentResult();
            result.ContentType = !commandResult.NotFound ? "application/json" : "application/text";
            result.Content = !commandResult.NotFound
                ? DataBrowserJsonSerializer.SerializeObject(
                    DeleteGenericEntityModelView.ConvertFromDashboard(commandResult, _requestContext.UserLang))
                : "Dashboard not found";
            if (commandResult.NotFound)
                result.StatusCode = 404;
            else if (commandResult.Nodes != null && commandResult.Nodes.Count > 0 ||
                     commandResult.AssignToHub)
                result.StatusCode = 409;
            else
                result.StatusCode = 204;
            return result;
        }

        /// <summary>
        ///     Delete Dashboard node association
        /// </summary>
        /// <response code="204">Resource deleted.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns></returns>
        [HttpDelete("/Dashboards/{dashboardId}/Nodes/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.AuthenticatedUser)]
        public async Task<ActionResult> RemoveDashboardAssociationToNode(int dashboardId, int nodeId)
        {
            _logger.LogDebug("START RemoveDashboardAssociationToNode");
            var commandResult =
                await CommandAsync(
                    new UnassignDashboardNodeCommand(_requestContext.DashboardId, _requestContext.NodeId));

            var result = new ContentResult();
            result.ContentType = "application/text";
            result.StatusCode = commandResult ? 204 : 404;
            return result;
        }

        /// <summary>
        ///     Delete Dashboard
        /// </summary>
        /// <response code="204">Resource deleted.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns></returns>
        [HttpDelete("/Dashboards/{dashboardId}/Hub")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<ActionResult> RemoveDashboardAssociationToHub(int dashboardId)
        {
            _logger.LogDebug("START RemoveDashboardAssociationToHub");
            var commandResult = await CommandAsync(new UnassignDashboardHubCommand(_requestContext.DashboardId, 1));

            var result = new ContentResult();
            result.ContentType = "application/text";
            result.StatusCode = commandResult ? 204 : 404;
            return result;
        }

        /// <summary>
        ///     Create new Dashboard.
        /// </summary>
        /// <response code="201">Create Dashboard</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>int</returns>
        [HttpPost("/Dashboards/Order/Hub")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<ActionResult> AssignDashboardHubOrder([FromBody] List<int> dashboardIds, int hubId = -1)
        {
            _logger.LogDebug("START AssignDashboardHubOrder");

            var assignResult =
                await CommandAsync(new AssignDashboardHubOrderCommand(hubId, dashboardIds, HttpContext.User));

            var result = new ContentResult();
            result.ContentType = "application/text";
            result.Content = assignResult ? assignResult.ToString() : "Not found";
            result.StatusCode = assignResult ? 204 : 404;
            return result;
        }

        /// <summary>
        ///     Create new Dashboard.
        /// </summary>
        /// <response code="201">Create Dashboard</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>int</returns>
        [HttpPost("/Dashboards/Order/Nodes/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.AuthenticatedUser)]
        public async Task<ActionResult> AssignDashboardNodeOrder([FromBody] List<int> dashboardIds, int nodeId)
        {
            _logger.LogDebug("START AssignDashboardNodeOrder");

            var assignResult =
                await CommandAsync(new AssignDashboardNodeOrderCommand(nodeId, dashboardIds, HttpContext.User, true));

            var result = new ContentResult();
            result.ContentType = "application/text";
            result.Content = assignResult ? assignResult.ToString() : "Not found";
            result.StatusCode = assignResult ? 204 : 404;
            return result;
        }

        /// <summary>
        ///     Create new Dashboard to Node association.
        /// </summary>
        /// <response code="201">Create Dashboard</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>int</returns>
        [HttpPost("/Dashboards/{dashboardId}/Nodes/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.AuthenticatedUser)]
        public async Task<ActionResult> AssignDashboardToSingleNode(int dashboardId, int nodeId)
        {
            _logger.LogDebug("START AssignDashboardToSingleNode");

            var resultId = await CommandAsync(new AssignDashboardToNodeCommand(_requestContext.DashboardId, nodeId,
                checkAssignPermission: true));

            var result = new ContentResult();
            result.ContentType = "application/text";
            result.Content = resultId ? resultId.ToString() : "Not found";
            result.StatusCode = resultId ? 204 : 404;
            return result;
        }

        /// <summary>
        ///     Create new Dashboard to Hub association.
        /// </summary>
        /// <response code="201">Create Dashboard</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>int</returns>
        [HttpPost("/Dashboards/{dashboardId}/Hub")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<ActionResult> AssignDashboardToHub(int dashboardId)
        {
            _logger.LogDebug("START AssignDashboardToHub");

            var resultId = await CommandAsync(new AssignDashboardToHubCommand(_requestContext.DashboardId, -1,
                checkAssignPermission: true, specificUser: HttpContext.User));


            var result = new ContentResult();
            result.ContentType = "application/text";
            result.Content = resultId ? resultId.ToString() : "Not found";
            result.StatusCode = resultId ? 204 : 404;
            return result;
        }
    }
}