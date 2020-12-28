using DataBrowser.AC.Utility;
using DataBrowser.Domain.Serialization;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Constants;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Query.Nodes;
using DataBrowser.Services.Interfaces;
using EndPointConnector.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WSHUB.Models.Response;

namespace WSHUB.Controllers
{
    [ApiController]
    public class DataSetsController : ApiBaseController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DataSetsController> _logger;
        private readonly IRequestContext _requestContext;

        public DataSetsController(ILogger<DataSetsController> logger,
            IRequestContext requestContext,
            IHttpContextAccessor httpContextAccessor,
            IMediatorService mediatorService)
            : base(mediatorService)
        {
            _logger = logger;
            _requestContext = requestContext;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        ///     Get dataflow structure (dimensions list)
        /// </summary>
        /// <param name="nodeId">Node id</param>
        /// <param name="datasetId">Dataset id</param>
        /// <response code="200">Node found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("Nodes/{nodeId}/Datasets/{datasetId}/Structure/{viewId:int?}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DatasetCriteriaViewModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetStructure(string nodeId, string datasetId, int? viewId = null)
        {
            _logger.LogDebug("START GetStructure");

            var isActive = await QueryAsync(new NodeIsActiveQuery(_requestContext.NodeId));
            if (!isActive)
            {
                _logger.LogDebug($"Node {_requestContext.NodeId} not found\\actived");
                var resultNotFound = new ContentResult
                {
                    ContentType = "application/text",
                    Content = "Node not found",
                    StatusCode = 404
                };
                return resultNotFound;
            }

            var inputHanlde = new StructureCriteriaForDataflowRequest
            {
                DataflowId = RequestAdapter.ConvertDataflowUriToDataflowId(datasetId),
                ViewId = viewId
            };
            var useCaseResult = await QueryAsync(inputHanlde);

            var result = new ContentResult
            {
                ContentType = useCaseResult != null ? "application/json" : "application/text",
                Content = useCaseResult != null
                ? DataBrowserJsonSerializer.SerializeObject(
                    DatasetCriteriaViewModel.ConvertFromStructureDto(useCaseResult, _requestContext.UserLang))
                : "Dataset not found",
                StatusCode = useCaseResult != null ? 200 : 404
            };

            return result;
        }

        /// <summary>
        ///     Get all codelist used in dataflow with all item used in dataflow.
        /// </summary>
        /// <param name="agency">Dataset agencu</param>
        /// <param name="datasetId">Dataset id</param>
        /// <param name="dimensionId">Get for single column</param>
        /// <response code="200">Node found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("Nodes/{nodeId}/Datasets/{datasetId}/Columns/Partial/Values")]
        [HttpGet("Nodes/{nodeId}/Datasets/{datasetId}/Column/{dimensionId}/Partial/values")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DatasetCriteriaViewModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetPartial(string nodeId, string datasetId, string dimensionId)
        {
            _logger.LogDebug("START GetPartial");

            var isActive = await QueryAsync(new NodeIsActiveQuery(_requestContext.NodeId));
            if (!isActive)
            {
                _logger.LogDebug($"Node {_requestContext.NodeId} not found\\actived");
                var resultNotFound = new ContentResult
                {
                    ContentType = "application/text",
                    Content = "Node not found",
                    StatusCode = 400
                };
                return resultNotFound;
            }

            var inputHanlde = new GetCodelistPartialForDataflowRequest
            {
                DataflowId = RequestAdapter.ConvertDataflowUriToDataflowId(datasetId),
                DimensionIds = dimensionId != null ? new List<string> {dimensionId} : null
            };
            var useCaseResult = await UseCaseAsync(inputHanlde);

            var result = new ContentResult
            {
                ContentType = useCaseResult != null ? "application/json" : "application/text",
                Content = useCaseResult != null
                ? DataBrowserJsonSerializer.SerializeObject(
                    DatasetCriteriaViewModel.ConvertFromDataDto(useCaseResult.ArtefactContainer,
                        _requestContext.UserLang))
                : "Dataset not found",
                StatusCode = useCaseResult != null ? 200 : 404
            };

            return result;
        }

        /// <summary>
        ///     Get all codelist used in dataflow with all item associated at codelist.
        /// </summary>
        /// <param name="nodeId">Node id</param>
        /// <param name="datasetId">Dataset id</param>
        /// <param name="dimensionId">Get for single column</param>
        /// <response code="200">Node found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("Nodes/{nodeId}/Datasets/{datasetId}/Columns/Full/Values")]
        [HttpGet("Nodes/{nodeId}/Datasets/{datasetId}/Column/{dimensionId}/Full/values")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DatasetCriteriaViewModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetFull(string nodeId, string datasetId, string dimensionId)
        {
            _logger.LogDebug("START GetFull");


            var isActive = await QueryAsync(new NodeIsActiveQuery(_requestContext.NodeId));
            if (!isActive)
            {
                _logger.LogDebug($"Node {_requestContext.NodeId} not found\\actived");
                var resultNotFound = new ContentResult
                {
                    ContentType = "application/text",
                    Content = "Node not found",
                    StatusCode = 400
                };
                return resultNotFound;
            }

            var inputHanlde = new GetCodelistFullInDataflowRequest
            {
                DataflowId = RequestAdapter.ConvertDataflowUriToDataflowId(datasetId),
                DimensionIds = dimensionId != null ? new List<string> {dimensionId} : null
            };
            var useCaseResult = await QueryAsync(inputHanlde);

            var result = new ContentResult
            {
                ContentType = useCaseResult != null ? "application/json" : "application/text",
                Content = useCaseResult != null
                ? DataBrowserJsonSerializer.SerializeObject(
                    DatasetCriteriaViewModel.ConvertFromDataDto(useCaseResult.ArtefactContainer,
                        _requestContext.UserLang))
                : "Dataset not found",
                StatusCode = useCaseResult != null ? 200 : 404
            };

            return result;
        }

        /// <summary>
        ///     Get the codelist associated at dimension in dataflow. The result can be filtred with the body data.
        /// </summary>
        /// <param name="nodeId">Node id</param>
        /// <param name="datasetId">Dataset id</param>
        /// <param name="dimensionId">Dimension id</param>
        /// <param name="dataCriterias">Filter for specific dimension</param>
        /// <response code="200">Node found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>Json</returns>
        [HttpPost("Nodes/{nodeId}/Datasets/{datasetId}/PartialCodelists/{dimensionId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DatasetCriteriaViewModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetConstraintFilter([FromBody] List<FilterCriteria> dataCriterias,
            string nodeId, string datasetId, string dimensionId)
        {
            _logger.LogDebug("START GetConstraintFilter");

            var isActive = await QueryAsync(new NodeIsActiveQuery(_requestContext.NodeId));
            if (!isActive)
            {
                _logger.LogDebug($"Node {_requestContext.NodeId} not found\\actived");
                var resultNotFound = new ContentResult
                {
                    ContentType = "application/text",
                    Content = "Node not found",
                    StatusCode = 400
                };
                return resultNotFound;
            }

            var inputHanlde = new GetCodelistDynamicForDataflowRequest
            {
                DataflowId = RequestAdapter.ConvertDataflowUriToDataflowId(datasetId),
                DataCriterias = dataCriterias,
                DimensionId = dimensionId
            };
            var useCaseResult = await QueryAsync(inputHanlde);

            var result = new ContentResult
            {
                ContentType = useCaseResult != null ? "application/json" : "application/text",
                Content = useCaseResult != null
                ? DataBrowserJsonSerializer.SerializeObject(
                    DatasetCriteriaViewModel.ConvertFromDataDto(useCaseResult.ArtefactContainer,
                        _requestContext.UserLang))
                : "Dataset not found",
                StatusCode = useCaseResult != null ? 200 : 404
            };

            return result;
        }

        /// <summary>
        ///     Get all data dataflow. The result can be filtred with the body data.
        /// </summary>
        /// <param name="nodeId">Node id</param>
        /// <param name="datasetId">Dataset id</param>
        /// <param name="dataCriterias">Filter for specific dimension</param>
        /// <response code="200">Node found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPost("Nodes/{nodeId}/Datasets/{datasetId}/Data")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetDataWithFilterFromDataflowUseCase(
            [FromBody] List<FilterCriteria> dataCriterias,
            string nodeId, string datasetId)
        {
            _logger.LogDebug("START GetDataWithFilterFromDataflowUseCase");


            var isActive = await QueryAsync(new NodeIsActiveQuery(_requestContext.NodeId));
            if (!isActive)
            {
                _logger.LogDebug($"Node {_requestContext.NodeId} not found\\actived");
                var resultNotFound = new ContentResult
                {
                    ContentType = "application/text",
                    Content = "Node not found",
                    StatusCode = 400
                };
                return resultNotFound;
            }

            var responseWatch = Stopwatch.StartNew();
            var inputHanlde = new DataFromDataflowRequest
            {
                DataflowId = RequestAdapter.ConvertDataflowUriToDataflowId(datasetId),
                DataCriterias = dataCriterias
            };


            var useCaseResult = await QueryAsync(inputHanlde);
            if (_httpContextAccessor != null && useCaseResult.Timers != null && useCaseResult.Timers.Count > 0)
            {
                responseWatch.Stop();

                var otherTime = responseWatch.ElapsedMilliseconds - useCaseResult.Timers
                    .Where(i => i.Value.Contains("ms")).Sum(i => Convert.ToInt64(i.Value.Replace("ms", "")));
                if (otherTime > 0)
                {
                    useCaseResult.Timers.Add("others", $"{otherTime}ms");
                }

                useCaseResult.Timers.Add("total", $"{responseWatch.ElapsedMilliseconds}ms");
                _httpContextAccessor.HttpContext.Response.Headers.Add("BackEndTimers",
                    DataBrowserJsonSerializer.SerializeObject(useCaseResult.Timers));
            }


            if (useCaseResult == null)
            {
                var resultNoData = new ContentResult
                {
                    ContentType = "application/text",
                    Content = "Dataset not found",
                    StatusCode = 404
                };
                return resultNoData;
            }

            var statusCodeReponse = 200;

            if (useCaseResult.LimitExceeded)
            {
                var textLimit = "";
                if (useCaseResult.ItemsCount.HasValue)
                {
                    textLimit = $". Items: {useCaseResult.ItemsCount}";
                }

                if (useCaseResult.ItemsMax.HasValue)
                {
                    textLimit = $". Max: {useCaseResult.ItemsMax}";
                }

                var resultNoData = new ContentResult
                {
                    ContentType = "application/text",
                    StatusCode = 413,
                    Content = $"Request return too many values{textLimit}"
                };
                return resultNoData;
            }

            if (useCaseResult.ItemsCount == 0 &&
                string.IsNullOrWhiteSpace(useCaseResult.JsonData) //ItemsCount not avaiable
            )
            {
                var resultNoData = new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = 204
                };
                return resultNoData;
            }

            if (useCaseResult.ItemsCount > 0)
            {
                _httpContextAccessor.HttpContext.Response.Headers.Add("ItemsCount",
                    useCaseResult.ItemsCount.ToString());
                if (useCaseResult.ItemsFrom > -1 &&
                    useCaseResult.ItemsTo > -1)
                {
                    if (useCaseResult.ItemsFrom > 0 || useCaseResult.ItemsTo < useCaseResult.ItemsCount - 1)
                    {
                        //_httpContextAccessor.HttpContext.Response.Headers.Add("Content-Range", string.Format("values {0}-{1}/{2}", useCaseResult.ItemsFrom.ToString(), useCaseResult.ItemsTo.ToString(), useCaseResult.ItemsCount.ToString()));
                        //_httpContextAccessor.HttpContext.Response.Headers.Add("Accept-Ranges", "values");
                        statusCodeReponse = 206; //Partial Response
                    }
                }
            }


            if (HttpContext?.User == null || !HttpContext.User.IsInRole(UserAndGroup.RoleAdministrator))
            {
                useCaseResult.Timers = null;
            }

            var result = new ContentResult
            {
                ContentType = "application/json",
                Content = useCaseResult.JsonData,
                StatusCode = statusCodeReponse
            };

            return result;
        }

        /// <summary>
        ///     Get all data dataflow.
        /// </summary>
        /// <param name="nodeId">Node id</param>
        /// <param name="datasetId">Dataset id</param>
        /// <response code="200">Node found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("Nodes/{nodeId}/Datasets/{datasetId}/Data/Count")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(long))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetCountObservationsFromDataflowUseCase(string nodeId, string datasetId)
        {
            return await GetCountObservationsWithFilterFromDataflowUseCase(null, nodeId, datasetId);
        }

        /// <summary>
        ///     Get all data dataflow. The result can be filtred with the body data.
        /// </summary>
        /// <param name="nodeId">Node id</param>
        /// <param name="datasetId">Dataset id</param>
        /// <param name="dataCriterias">Filter for specific dimension</param>
        /// <response code="200">Node found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPost("Nodes/{nodeId}/Datasets/{datasetId}/Data/Count")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(long))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetCountObservationsWithFilterFromDataflowUseCase(
            [FromBody] List<FilterCriteria> dataCriterias,
            string nodeId, string datasetId)
        {
            _logger.LogDebug("START GetCountObservationsWithFilterFromDataflowUseCase");

            var isActive = await QueryAsync(new NodeIsActiveQuery(_requestContext.NodeId));
            if (!isActive)
            {
                _logger.LogDebug($"Node {_requestContext.NodeId} not found\\actived");
                var resultNotFound = new ContentResult
                {
                    ContentType = "application/text",
                    Content = "Node not found",
                    StatusCode = 400
                };
                return resultNotFound;
            }

            var inputHanlde = new CountObservationsFromDataflowRequest
            {
                DataflowId = RequestAdapter.ConvertDataflowUriToDataflowId(datasetId),
                DataCriterias = dataCriterias
            };
            var useCaseResult = await QueryAsync(inputHanlde);

            var result = new ContentResult
            {
                ContentType = useCaseResult != null ? "application/json" : "application/text",
                Content = useCaseResult != null ? useCaseResult.Count.ToString() : "Dataset not found",
                StatusCode = useCaseResult != null ? 200 : 404
            };
            return result;
        }


        /// <summary>
        ///     Download dataflow
        /// </summary>
        /// <param name="nodeId">Node id</param>
        /// <param name="datasetId">Dataset id</param>
        /// <param name="format">
        ///     Format of dataset download: genericdata, genericdata20, jsondata, structurespecificdata, csv,
        ///     compactdata, edidata
        /// </param>
        /// <param name="obersevation">Obeservation</param>
        /// <response code="200">Dataflow found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Dataflow not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPost("Nodes/{nodeId}/Datasets/{datasetId}/{downloadType?}/{format?}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(File))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DownloadDataflow([FromBody] List<FilterCriteria> dataCriterias,
            string nodeId,
            string datasetId,
            string downloadType = "download",
            string format = "jsondata")
        {
            _logger.LogDebug("START DownloadDataflow");

            if (string.IsNullOrWhiteSpace(downloadType) &&
                !downloadType.Equals("Download", StringComparison.InvariantCultureIgnoreCase) &&
                !downloadType.Equals("DownloadZip"))
            {
                var resultNotFound = new ContentResult
                {
                    ContentType = "application/text",
                    Content = "Download type unknow",
                    StatusCode = 400
                };
                return resultNotFound;
            }

            var isActive = await QueryAsync(new NodeIsActiveQuery(_requestContext.NodeId));
            if (!isActive)
            {
                _logger.LogDebug($"Node {_requestContext.NodeId} not found\\actived");
                var resultNotFound = new ContentResult
                {
                    ContentType = "application/text",
                    Content = "Node not found",
                    StatusCode = 404
                };
                return resultNotFound;
            }

            datasetId = RequestAdapter.ConvertDataflowUriToDataflowId(datasetId);

            var downloadResult = await QueryAsync(new DownloadDataflowFromNodeEndPointRequest(datasetId,
                format,
                downloadType.Equals("DownloadZip"),
                dataCriterias));

            var result = new ContentResult();
            if (downloadResult.Response.HaveError)
            {
                result.ContentType = "application/text";
                result.Content = "Some error on download dataflow";
                result.StatusCode = 500;
            }
            else if (downloadResult.Response.NotFoundResource)
            {
                result.ContentType = "application/text";
                result.Content = "Dataflow not found";
                result.StatusCode = 404;
            }
            else if (downloadResult.Response.NotAcceptableDataResponse)
            {
                result.ContentType = "application/text";
                result.Content =
                    $"Dataflow download not support {format}. {downloadResult.Response.Errors?.Join("\n")}";
                result.StatusCode = 406;
            }
            else if (downloadResult.IsCompressed)
            {
                return File(System.IO.File.OpenRead(downloadResult.Response.Data),
                    "application/zip",
                    Path.GetFileName(downloadResult.Response.Data));
            }
            else
            {
                return File(System.IO.File.OpenRead(downloadResult.Response.Data),
                    !string.IsNullOrWhiteSpace(downloadResult.Response.ResponseType)
                        ? downloadResult.Response.ResponseType
                        : "application/text",
                    Path.GetFileName(downloadResult.Response.Data));
            }

            return result;
        }
    }
}