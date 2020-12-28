using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBrowser.AC.Utility.Helpers.ExtMethod;
using DataBrowser.Command.Hubs;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Serialization;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Constants;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Query.Hubs;
using DataBrowser.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WSHUB.Models.Response;

namespace WSHUB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HubController : ApiBaseController
    {
        private readonly ILogger<NodesController> _logger;
        private readonly IRequestContext _requestContext;

        public HubController(ILogger<NodesController> logger,
            IRequestContext requestContext,
            IMediatorService mediator)
            : base(mediator)
        {
            _logger = logger;
            _requestContext = requestContext;
        }

        /// <summary>
        ///     Update Hub.
        /// </summary>
        /// <response code="204">Node updated.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<ActionResult> UpdateHub([FromBody] HubDto hubUpdateRequest)
        {
            _logger.LogDebug("START UpdateHub");
            var useCaseResult = await CommandAsync(new EditHubCommand(hubUpdateRequest));

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = DataBrowserJsonSerializer.SerializeObject(useCaseResult);
            result.StatusCode = 200;
            return result;
        }

        /// <summary>
        ///     Get hub.
        /// </summary>
        /// <param name="hubId">Hub id</param>
        /// <response code="200">Node found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("{hubId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HubDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<ActionResult> GetHub(int hubId)
        {
            _logger.LogDebug("START GetHub");
            var hubs = await QueryAsync(new HubsListQuery());
            var hubResult = hubs.FirstOrDefault(i => i.HubId == hubId);

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = hubResult != null ? DataBrowserJsonSerializer.SerializeObject(hubResult) : "Hub not found";
            result.StatusCode = hubResult != null ? 200 : 404;
            return result;
        }

        /// <summary>
        ///     Get all hub.
        /// </summary>
        /// <response code="200">Node found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<HubMinimalModelView>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAllHubs()
        {
            _logger.LogDebug("START GetAllHubs");
            var hubsResult = await UseCaseAsync(new HubsListQuery());


            var hubModelView = new List<HubMinimalModelView>();
            var item = hubsResult.FirstOrDefault();
            if (item != null)
                hubModelView.Add(new HubMinimalModelView
                {
                    HubId = item.HubId,
                    LogoURL = item.LogoURL,
                    BackgroundMediaURL = item.BackgroundMediaURL,
                    SupportedLanguages = item.SupportedLanguages,
                    DefaultLanguage = item.DefaultLanguage,
                    MaxObservationsAfterCriteria = item.MaxObservationsAfterCriteria,
                    Title = item?.Title?.GetTranslateItem(_requestContext.UserLang),
                    Description = item?.Description?.GetTranslateItem(_requestContext.UserLang),
                    Slogan = item?.Slogan?.GetTranslateItem(_requestContext.UserLang),
                    Disclaimer = item?.Disclaimer?.GetTranslateItem(_requestContext.UserLang)
                });


            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = DataBrowserJsonSerializer.SerializeObject(hubsResult);
            result.StatusCode = 200;
            return result;
        }

        /// <summary>
        ///     Update hub.
        /// </summary>
        /// <response code="200">Node found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("Config")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<HubDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<ActionResult> GetAllHubsConfig()
        {
            _logger.LogDebug("START GetAllHubsConfig");
            var hubsResult = await QueryAsync(new HubsListQuery());

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = hubsResult != null && hubsResult.Count > 0
                ? DataBrowserJsonSerializer.SerializeObject(hubsResult)
                : "";
            result.StatusCode = hubsResult != null && hubsResult.Count > 0 ? 200 : 404;
            return result;
        }

        /// <summary>
        ///     Get hub with minimal node info.
        /// </summary>
        /// <param name="hubId">Hub id</param>
        /// <response code="200">Hub data found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Hub not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("{hubId}/MinimalInfo")]
        [HttpGet("MinimalInfo")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HubInfoMinimalModelView))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetNodeWithMinimalInfo(int hubId)
        {
            _logger.LogDebug("START GetNodeWithMinimalInfo");

            var getHubAndMinimalInfoRequest = new HubAndMinimalInfoRequest {HubId = hubId};
            var hubResult = await QueryAsync(getHubAndMinimalInfoRequest);

            HubInfoMinimalModelView hubInfoMinimalResponse = null;
            if (hubResult != null)
            {
                hubInfoMinimalResponse = new HubInfoMinimalModelView();
                hubInfoMinimalResponse.Hub = new HubInfoMinimalModelView.MinimalHub();
                hubInfoMinimalResponse.Hub.LogoURL = hubResult.Hub.LogoURL;
                hubInfoMinimalResponse.Hub.Description =
                    hubResult?.Hub?.Description?.GetTranslateItem(_requestContext.UserLang);
                hubInfoMinimalResponse.Hub.Slogan = hubResult.Hub.Slogan.GetTranslateItem(_requestContext.UserLang);
                hubInfoMinimalResponse.Hub.Disclaimer = hubResult.Hub.Disclaimer.GetTranslateItem(_requestContext.UserLang);
                hubInfoMinimalResponse.Hub.Name = hubResult.Hub.Title.GetTranslateItem(_requestContext.UserLang);
                hubInfoMinimalResponse.Hub.BackgroundMediaURL = hubResult.Hub.BackgroundMediaURL;
                hubInfoMinimalResponse.Hub.SupportedLanguages = hubResult.Hub?.SupportedLanguages;
                hubInfoMinimalResponse.Hub.DefaultLanguage = hubResult.Hub.DefaultLanguage;
                hubInfoMinimalResponse.Hub.MaxObservationsAfterCriteria = hubResult.Hub.MaxObservationsAfterCriteria;
                hubInfoMinimalResponse.Hub.MaxCells = hubResult.Hub.MaxCells;
                hubInfoMinimalResponse.Hub.Extras = hubResult.Hub.Extras;
                hubInfoMinimalResponse.Hub.Dashboards = hubResult.HubDashboards?.Select(i =>
                    new HubInfoMinimalModelView.MinimalHub.HubDashboard
                        {Id = i.Id, Title = i.Titles?.GetTranslateItem(_requestContext.UserLang)}).ToList();
                hubInfoMinimalResponse.Nodes = hubResult.Nodes?.OrderBy(i => i.Order)?.Select(i => new NodeModelView
                { //TODO create convert auto mapper
                    NodeId = i.NodeId,
                    Name = i.Title.GetTranslateItem(_requestContext.UserLang),
                    Default = i.Default,
                    Code = i.Code,
                    Order = i.Order,
                    ShowCategoryLevels = i.ShowCategoryLevels,
                    CatalogNavigationMode = i.CatalogNavigationMode,
                    BackgroundMediaURL = i.BackgroundMediaURL,
                    LogoURL = i.Logo,
                    Dashboards = i.Dashboards?.Select(i => new NodeModelView.Dashboard
                        {Id = i.Id, Title = i.Titles?.GetTranslateItem(_requestContext.UserLang)}).ToList()
                }).ToList();
            }

            var result = new ContentResult();
            result.ContentType = hubResult != null ? "application/json" : "application/text";
            result.Content = hubInfoMinimalResponse != null
                ? DataBrowserJsonSerializer.SerializeObject(hubInfoMinimalResponse)
                : "Hub not found";
            result.StatusCode = hubResult != null ? 200 : 404;
            return result;
        }
    }
}