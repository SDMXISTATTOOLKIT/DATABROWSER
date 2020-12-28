using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBrowser.AC.Responses.Services;
using DataBrowser.AC.Utility.Helpers.ExtMethod;
using DataBrowser.Command.Nodes;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Domain.Serialization;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Constants;
using DataBrowser.Query.Dashboards;
using DataBrowser.Query.Nodes;
using DataBrowser.Services.Interfaces;
using DataBrowser.Specifications.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WSHUB.Models;
using WSHUB.Models.Request;
using WSHUB.Models.Response;
using WSHUB.Utils;

namespace WSHUB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NodesController : ApiBaseController
    {
        private readonly ILogger<NodesController> _logger;
        private readonly IRequestContext _requestContext;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceProvider _serviceProvider;

        public NodesController(ILogger<NodesController> logger,
            IRequestContext requestContext,
            IMediatorService mediatorService,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider)
            : base(mediatorService)
        {
            _logger = logger;
            _requestContext = requestContext;
            _loggerFactory = loggerFactory;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        ///     Create new node.
        /// </summary>
        /// <response code="201">Create node</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NodeDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<ActionResult> CreateNode([FromBody] NodeCreateRequest nodeCreateRequest)
        {
            _logger.LogDebug("START CreateNode");
            var nodeId = await CommandAsync(new CreateNodeCommand(nodeCreateRequest));

            NodeDto nodeResult = null;
            if (nodeId > 0) nodeResult = await QueryAsync(new NodeByIdQuery(nodeId));

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = DataBrowserJsonSerializer.SerializeObject(nodeResult);
            result.StatusCode = 201;
            return result;
        }

        /// <summary>
        ///     Update node.
        /// </summary>
        /// <response code="204">Node updated.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPut("{nodeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ManageConfig)]
        public async Task<ActionResult> UpdateNode([FromBody] NodeUpdateRequest nodeUpdateRequest, int nodeId)
        {
            _logger.LogDebug("START UpdateNode");

            var commandResult = await CommandAsync(new EditNodeCommand(nodeUpdateRequest));

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = DataBrowserJsonSerializer.SerializeObject(commandResult);
            result.StatusCode = commandResult != null ? 200 : 404;
            return result;
        }

        /// <summary>
        ///     Delete node.
        /// </summary>
        /// <param name="nodeId">Node id to delete</param>
        /// <response code="204">Resource deleted.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns></returns>
        [HttpDelete("{nodeId}/{forceDelete?}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(DeleteGenericEntityModelView))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<ActionResult> DeleteNode(int nodeId, bool? forceDelete = false)
        {
            _logger.LogDebug("START DeleteNode");

            if (!forceDelete.HasValue ||
                !forceDelete.Value)
            {
                var dashboardWithView =
                    await QueryAsync(new DashboardListWithViewAssignToNodeIdQuery(nodeId, HttpContext.User));

                if (dashboardWithView != null &&
                    dashboardWithView.Any())
                {
                    var respondeModelView = new DeleteGenericEntityModelView();
                    respondeModelView.DeleteResult = false;
                    respondeModelView.UsedBy = dashboardWithView.SelectMany(i => i.Views)
                        .Where(i => i.Value.NodeId == nodeId).Select(i => new DeleteGenericEntityModelView.GenericEntity
                        {
                            Type = "view", Id = i.Value.ViewTemplateId,
                            Title = i.Value.Title.GetTranslateItem(_requestContext.UserLang)
                        }).ToList();

                    var resultConflict = new ContentResult();
                    resultConflict.ContentType = "application/json";
                    resultConflict.Content = DataBrowserJsonSerializer.SerializeObject(respondeModelView);
                    resultConflict.StatusCode = 409;
                    return resultConflict;
                }
            }

            var commandResult = await CommandAsync(new RemoveNodeCommand(nodeId));

            var result = new ContentResult();
            result.ContentType = commandResult ? "application/json" : "application/text";
            result.StatusCode = commandResult ? 204 : 404;
            return result;
        }

        /// <summary>
        ///     Get node with all data configurations
        /// </summary>
        /// <param name="nodeId">Node id</param>
        /// <response code="200">Node found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("{nodeId}/config")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NodeDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ManageConfig)]
        public async Task<ActionResult> GetNode(int nodeId)
        {
            _logger.LogDebug("START GetNode");

            var nodeResult = await QueryAsync(new NodeByIdQuery(nodeId));

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = nodeResult != null
                ? DataBrowserJsonSerializer.SerializeObject(nodeResult)
                : "Node not found";
            result.StatusCode = nodeResult != null ? 200 : 404;
            return result;
        }

        /// <summary>
        ///     Update node orders.
        /// </summary>
        /// <response code="204">Nodes updated.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="401">UnAuthorize</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPut("Order")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<ActionResult> OrderNodes([FromBody] List<int> orderNodes)
        {
            _logger.LogDebug("START OrderNodes");

            var commandResult = await CommandAsync(new OrderNodeCommand(orderNodes));

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.StatusCode = 204;

            _logger.LogDebug("END OrderNodes");
            return result;
        }

        /// <summary>
        ///     Get all nodes with only minimal info.
        /// </summary>
        /// <response code="200">Node found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NodeModelView>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAllNode()
        {
            _logger.LogDebug("START GetAllNode");
            var nodeResults = await QueryAsync(new ActiveNodeListWithMinimalInfoQuery());

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = nodeResults != null
                ? DataBrowserJsonSerializer.SerializeObject(nodeResults
                    .Select(i => i.ConvertToNodeMinimalInfoModelView(_requestContext.UserLang)).ToList())
                : "[]";
            result.StatusCode = 200;
            return result;
        }

        /// <summary>
        ///     Get node with all minimal configurations.
        /// </summary>
        /// <response code="200">Node found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("Config")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<NodeMinimalInfoDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAllNodeConfig()
        {
            _logger.LogDebug("START GetAllNodeConfig");
            var nodeResults = await QueryAsync(new AllNodesWithMinimalInfoQuery(filterByPermissionNodeCache: true,
                filterByPermissionNodeConfig: true,
                filterByPermissionNodeTemplate: true,
                filterIsInAnd: false));

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = nodeResults != null ? DataBrowserJsonSerializer.SerializeObject(nodeResults) : "[]";
            result.StatusCode = 200;
            return result;
        }

        /// <summary>
        ///     Get node with only data view
        /// </summary>
        /// <param name="nodeId">Node id</param>
        /// <response code="200">Node found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("{nodeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NodeModelView))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetNodeDataView(int nodeId)
        {
            _logger.LogDebug("START GetNodeDataView");

            var isActive = await QueryAsync(new NodeIsActiveQuery(_requestContext.NodeId));
            if (!isActive)
            {
                var nodesNotFound = new ContentResult();
                nodesNotFound.ContentType = "application/text";
                nodesNotFound.Content = "Node not found";
                nodesNotFound.StatusCode = 404;
                return nodesNotFound;
            }

            var nodeResult = await QueryAsync(new NodeByIdQuery(_requestContext.NodeId));

            var dashboards =
                await _mediatorService.QueryAsync(new DashboardListByNodeIdQuery(_requestContext.NodeId, false,
                    HttpContext?.User, true));

            var modelView = nodeResult.ConvertToNodeUserModelView(_requestContext.UserLang, dashboards);

            var result = new ContentResult();
            result.ContentType = nodeResult != null ? "application/json" : "application/text";
            result.Content = nodeResult != null
                ? DataBrowserJsonSerializer.SerializeObject(modelView)
                : "Node not found";
            result.StatusCode = nodeResult != null ? 200 : 404;
            return result;
        }

        /// <summary>
        ///     Get the catalog
        /// </summary>
        /// <response code="200">Artefact found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Artefact not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("{nodeId}/Catalog")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetTree([FromServices] IDataBrowserMemoryCache dataBrowserMemoryCache)
        {
            _logger.LogDebug($"START Catalog Tree {_requestContext.NodeId}");

            var nodeCatalogJson =
                await GetTreeHelper.GetCatalogTreeAsync(_mediatorService, dataBrowserMemoryCache, _requestContext, _loggerFactory, _serviceProvider);

            var result = new ContentResult();
            result.ContentType = nodeCatalogJson != null ? "application/json" : "application/text";
            result.Content = nodeCatalogJson != null ? nodeCatalogJson : "Node not found";
            result.StatusCode = nodeCatalogJson != null ? 200 : 404;

            _logger.LogDebug($"END Catalog Tree {_requestContext.NodeId}");
            return result;
        }

        /// <summary>
        ///     Get the catalog
        /// </summary>
        /// <response code="200">Artefact found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Artefact not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("/NodesCode/{nodeCode}/Catalog")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetTreeCode(
                            [FromServices] IDataBrowserMemoryCache dataBrowserMemoryCache,
                            [FromServices] IRepository<Node> repository,
                            string nodeCode)
        {
            _logger.LogDebug($"START Catalog Tree Code {_requestContext.NodeCode}");

            var node = await repository.FindAsync(new NodeByCodeSpecification(nodeCode, NodeByCodeSpecification.ExtraInclude.Nothing));
            if (node?.FirstOrDefault() == null)
            {
                var resultNodFound = new ContentResult();
                resultNodFound.ContentType = "application/text";
                resultNodFound.Content = "Node not found";
                resultNodFound.StatusCode = 404;
                return resultNodFound;
            }

            var nodeId = node.First().NodeId;

            _requestContext.OverwriteNodeId(nodeId);
            _requestContext.OverwriteNodeCode(nodeCode);

            var nodeCatalogJson =
                await GetTreeHelper.GetCatalogTreeAsync(_mediatorService, dataBrowserMemoryCache, _requestContext, _loggerFactory, _serviceProvider);

            var result = new ContentResult();
            result.ContentType = nodeCatalogJson != null ? "application/json" : "application/text";
            result.Content = nodeCatalogJson != null ? nodeCatalogJson : "Node not found";
            result.StatusCode = nodeCatalogJson != null ? 200 : 404;

            _logger.LogDebug($"END Catalog Tree {_requestContext.NodeId}");
            return result;
        }
    }
}