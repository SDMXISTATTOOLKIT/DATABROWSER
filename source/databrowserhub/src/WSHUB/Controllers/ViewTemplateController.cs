using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DataBrowser.Command.ViewTemplates;
using DataBrowser.Command.ViewTemplates.Model;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Serialization;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Constants;
using DataBrowser.Interfaces.ModelViews;
using DataBrowser.Query.Nodes;
using DataBrowser.Query.ViewTemplates;
using DataBrowser.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WSHUB.Models.Response;

namespace WSHUB.Controllers
{
    [ApiController]
    public class ViewTemplateController : ApiBaseController
    {
        private readonly ILogger<ViewTemplateController> _logger;
        private readonly IRequestContext _requestContext;


        public ViewTemplateController(ILogger<ViewTemplateController> logger,
            IRequestContext requestContext,
            IMediatorService mediatorService)
            : base(mediatorService)
        {
            _logger = logger;
            _requestContext = requestContext;
        }


        /// <summary>
        ///     Create new Template.
        /// </summary>
        /// <response code="201">Create ViewTemplate</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>ViewTemplate</returns>
        [HttpPost("/Nodes/{nodeId}/Templates")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ViewTemplateViewModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ManageTemplate)]
        public async Task<ActionResult> CreateTemplate([FromBody] ViewTemplateViewModel viewTemplateCreateRequest)
        {
            _logger.LogDebug("START CreateTemplate");

            if (viewTemplateCreateRequest.Type != ViewTemplateType.Template)
                return BadRequest($"Type must be {ViewTemplateType.Template}");

            viewTemplateCreateRequest.Type = ViewTemplateType.Template;
            viewTemplateCreateRequest.NodeId = _requestContext.NodeId;

            return await CreateViewTemplateAsync(viewTemplateCreateRequest);
        }

        /// <summary>
        ///     Create new View.
        /// </summary>
        /// <response code="201">Create ViewTemplate</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>ViewTemplate</returns>
        [HttpPost("/Nodes/{nodeId}/Views")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ViewTemplateViewModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ManageView)]
        public async Task<ActionResult> CreateView([FromBody] ViewTemplateViewModel viewTemplateCreateRequest)
        {
            _logger.LogDebug("START CreateView");

            var isActive = await QueryAsync(new NodeIsActiveQuery(_requestContext.NodeId));
            if (!isActive)
            {
                var nodesNotFound = new ContentResult();
                nodesNotFound.ContentType = "application/text";
                nodesNotFound.Content = "Node not found";
                nodesNotFound.StatusCode = 404;
                return nodesNotFound;
            }

            if (viewTemplateCreateRequest.Type != ViewTemplateType.View)
                return BadRequest($"Type must be {ViewTemplateType.View}");

            viewTemplateCreateRequest.Type = ViewTemplateType.View;
            viewTemplateCreateRequest.NodeId = _requestContext.NodeId;

            return await CreateViewTemplateAsync(viewTemplateCreateRequest);
        }


        

        private async Task<ContentResult> CreateViewTemplateAsync(ViewTemplateViewModel viewTemplateCreateRequest)
        {
            viewTemplateCreateRequest.DatasetId =
                viewTemplateCreateRequest.DatasetId.Replace(",", "+", StringComparison.InvariantCultureIgnoreCase);
            var creationResponse = await CommandAsync(new CreateViewTemplateCommand(
                ViewTemplateViewModel.ConvertToDto(viewTemplateCreateRequest),
                _requestContext.NodeId,
                specificUser: HttpContext.User));

            ViewTemplateDto createViewTemplateResult = null;
            if (creationResponse.HasErrors == false)
            {
                return await MakeCreationSuccessContentResult(creationResponse, creationResponse.Id, viewTemplateCreateRequest.Type);
            }
            else
            {
                return MakeCreationErrorContentResult(creationResponse);
            }
        }

        protected async Task<ContentResult> MakeCreationSuccessContentResult(CreateOrUpdateViewTemplateResult creationResponse, int createdId, ViewTemplateType type )
        {
            var createViewTemplateResult = await QueryAsync(new ViewTemplateById_Type_NodeIdQuery(createdId,
                    _requestContext.NodeId, type));
            var responseBody = new ContentResult();
            responseBody.ContentType = "application/json";
            responseBody.Content =
                DataBrowserJsonSerializer.SerializeObject(
                    ViewTemplateViewModel.ConvertFromDto(createViewTemplateResult));
            responseBody.StatusCode = 201;
            return responseBody;
        }

        protected ContentResult MakeCreationErrorContentResult(CreateOrUpdateViewTemplateResult creationResult)
        {
            var responseBody = new ContentResult();
            responseBody.ContentType = "application/text";
            responseBody.Content = "Generic ERROR";
            responseBody.StatusCode = 404;

            if (creationResult.ErrorType == CreateOrUpdateViewTemplateErrorType.PERMISSION)
            {
                responseBody.Content = "Permission ERROR";
                responseBody.StatusCode = 403;
            }
            if (creationResult.ErrorType == CreateOrUpdateViewTemplateErrorType.TITLE_COLLISION)
            {
                responseBody.Content = creationResult.Errors[0];
                responseBody.StatusCode = 409;
            }

            return responseBody;
        }


        /// <summary>
        ///     Get a Template.
        /// </summary>
        /// <response code="201">Create ViewTemplate</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>ViewTemplate</returns>
        [HttpGet("/Nodes/{nodeId}/Templates/{templateId}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ViewTemplateViewModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ManageTemplate)]
        public async Task<ActionResult> GetTemplateById(int nodeId, int templateId)
        {
            _logger.LogDebug("START GetTemplateById");

            var isActive = await QueryAsync(new NodeIsActiveQuery(_requestContext.NodeId));
            if (!isActive)
            {
                var nodesNotFound = new ContentResult();
                nodesNotFound.ContentType = "application/text";
                nodesNotFound.Content = "Node not found";
                nodesNotFound.StatusCode = 404;
                return nodesNotFound;
            }

            var viewTemplateResult =
                await QueryAsync(new ViewTemplateById_Type_NodeIdQuery(templateId, _requestContext.NodeId,
                    ViewTemplateType.Template));

            var result = new ContentResult();
            result.ContentType = viewTemplateResult != null ? "application/json" : "application/text";
            result.Content = viewTemplateResult != null
                ? DataBrowserJsonSerializer.SerializeObject(ViewTemplateViewModel.ConvertFromDto(viewTemplateResult))
                : "Template not found";
            result.StatusCode = viewTemplateResult != null ? 200 : 404;
            return result;
        }

        /// <summary>
        ///     Get a View.
        /// </summary>
        /// <response code="201">Create ViewTemplate</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>ViewTemplate</returns>
        [HttpGet("/Nodes/{nodeId}/Views/{viewId}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ViewTemplateViewModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ManageView)]
        public async Task<ActionResult> GetViewById(int nodeId, int viewId)
        {
            _logger.LogDebug("START GetViewById");

            var isActive = await QueryAsync(new NodeIsActiveQuery(_requestContext.NodeId));
            if (!isActive)
            {
                var nodesNotFound = new ContentResult();
                nodesNotFound.ContentType = "application/text";
                nodesNotFound.Content = "Node not found";
                nodesNotFound.StatusCode = 404;
                return nodesNotFound;
            }

            var viewTemplateResult =
                await QueryAsync(new ViewTemplateById_Type_NodeIdQuery(viewId, _requestContext.NodeId,
                    ViewTemplateType.View, true));

            ViewTemplateViewModel viewViewModel = ViewTemplateViewModel.ConvertFromDto(viewTemplateResult);

            var result = new ContentResult();
            result.ContentType = viewViewModel != null ? "application/json" : "application/text";
            result.Content = viewViewModel != null
                ? DataBrowserJsonSerializer.SerializeObject(viewViewModel)
                : "View not found";
            result.StatusCode = viewTemplateResult != null ? 200 : 404;
            return result;
        }


        /// <summary>
        ///     Get all Template from a node.
        /// </summary>
        /// <response code="201">Create ViewTemplate</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>List of ViewTemplate</returns>
        [HttpGet("/Nodes/{nodeId}/Templates")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(List<ViewTemplateViewModel>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.AuthenticatedUser)]
        public async Task<ActionResult> GetTemplateListByNodeId()
        {
            _logger.LogDebug("START GetTemplateListByNodeId");

            var isActive = await QueryAsync(new NodeIsActiveQuery(_requestContext.NodeId));
            if (!isActive)
            {
                var nodesNotFound = new ContentResult();
                nodesNotFound.ContentType = "application/text";
                nodesNotFound.Content = "Node not found";
                nodesNotFound.StatusCode = 404;
                return nodesNotFound;
            }

            var viewTemplateResult = await QueryAsync(new ViewTemplateListByType_NodeIdQuery(_requestContext.NodeId,
                ViewTemplateType.Template, filterByPermissionNodeTemplate: true));

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content =
                DataBrowserJsonSerializer.SerializeObject(ViewTemplateViewModel.ConvertFromDto(viewTemplateResult));
            result.StatusCode = 200;
            return result;
        }

        /// <summary>
        ///     Get all View from a node.
        /// </summary>
        /// <response code="201">Create ViewTemplate</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>List of ViewTemplate</returns>
        [HttpGet("/Nodes/{nodeId}/Views")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(List<ViewTemplateViewModel>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.AuthenticatedUser)]
        public async Task<ActionResult> GetViewListByNodeId()
        {
            _logger.LogDebug("START GetViewListByNodeId");

            var isActive = await QueryAsync(new NodeIsActiveQuery(_requestContext.NodeId));
            if (!isActive)
            {
                var nodesNotFound = new ContentResult();
                nodesNotFound.ContentType = "application/text";
                nodesNotFound.Content = "Node not found";
                nodesNotFound.StatusCode = 404;
                return nodesNotFound;
            }

            var viewTemplateResult =
                await QueryAsync(
                    new ViewTemplateListByType_NodeIdQuery(_requestContext.NodeId, ViewTemplateType.View, true));

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content =
                DataBrowserJsonSerializer.SerializeObject(ViewTemplateViewModel.ConvertFromDto(viewTemplateResult));
            result.StatusCode = 200;
            return result;
        }

        /// <summary>
        ///     Get all View.
        /// </summary>
        /// <response code="201">Create ViewTemplate</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>List of ViewTemplate</returns>
        [HttpGet("/Views")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(List<ViewTemplateViewModel>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ManageView)]
        public async Task<ActionResult> GetViewList()
        {
            _logger.LogDebug("START GetViewList");

            var viewTemplateResult = await QueryAsync(new ViewTemplateListByTypeQuery(ViewTemplateType.View, true));

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content =
                DataBrowserJsonSerializer.SerializeObject(ViewTemplateViewModel.ConvertFromDto(viewTemplateResult));
            result.StatusCode = 200;
            return result;
        }

        /// <summary>
        ///     Delete Template.
        /// </summary>
        /// <param name="templateId">viewTemplate id to delete</param>
        /// <response code="204">Resource deleted.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns></returns>
        [HttpDelete("/Nodes/{nodeId}/Templates/{templateId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RemoveViewTemplateResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ManageTemplate)]
        public async Task<ActionResult> DeleteviewTemplate(int templateId)
        {
            _logger.LogDebug("START DeleteviewTemplate");

            var isActive = await QueryAsync(new NodeIsActiveQuery(_requestContext.NodeId));
            if (!isActive)
            {
                var nodesNotFound = new ContentResult();
                nodesNotFound.ContentType = "application/text";
                nodesNotFound.Content = "Node not found";
                nodesNotFound.StatusCode = 404;
                return nodesNotFound;
            }

            var commandResult = await CommandAsync(new RemoveViewTemplateCommand(templateId, _requestContext.NodeId));

            var result = new ContentResult();
            result.ContentType = !commandResult.NotFound ? "application/json" : "application/text";
            result.Content = !commandResult.NotFound
                ? DataBrowserJsonSerializer.SerializeObject(commandResult)
                : "Template not found";
            result.StatusCode = !commandResult.NotFound ? 200 : 404;
            return result;
        }

        /// <summary>
        ///     Delete View.
        /// </summary>
        /// <param name="viewId">viewTemplate id to delete</param>
        /// <response code="204">Resource deleted.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns></returns>
        [HttpDelete("/Nodes/{nodeId}/Views/{viewId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RemoveViewTemplateResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(DeleteGenericEntityModelView))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ManageView)]
        public async Task<ActionResult> DeleteView(int viewId)
        {
            _logger.LogDebug("START DeleteView");

            var isActive = await QueryAsync(new NodeIsActiveQuery(_requestContext.NodeId));
            if (!isActive)
            {
                var nodesNotFound = new ContentResult();
                nodesNotFound.ContentType = "application/text";
                nodesNotFound.Content = "Node not found";
                nodesNotFound.StatusCode = 404;
                return nodesNotFound;
            }

            var commandResult = await CommandAsync(new RemoveViewTemplateCommand(viewId, _requestContext.NodeId, true));

            var result = new ContentResult();
            result.ContentType = !commandResult.NotFound ? "application/json" : "application/text";
            result.Content = !commandResult.NotFound
                ? DataBrowserJsonSerializer.SerializeObject(
                    DeleteGenericEntityModelView.ConvertFromViewTemplate(commandResult, _requestContext.UserLang))
                : "View not found";
            if (commandResult.NotFound)
                result.StatusCode = 404;
            else if (commandResult.Dashboards != null &&
                     commandResult.Dashboards.Count > 0)
                result.StatusCode = 409;
            else
                result.StatusCode = 200;
            return result;
        }
    }
}