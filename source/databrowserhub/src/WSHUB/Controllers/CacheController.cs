using CsvHelper;
using DataBrowser.AC.Utility;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Domain.Serialization;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Cache.Key;
using DataBrowser.Interfaces.Constants;
using DataBrowser.Interfaces.Dto;
using DataBrowser.Interfaces.Workers;
using DataBrowser.Query.Nodes;
using DataBrowser.Services.Interfaces;
using DataBrowser.Specifications.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WSHUB.Models.Request;
using WSHUB.Models.Response;
using WSHUB.Utils;

namespace WSHUB.Controllers
{
    [ApiController]
    public class CacheController : ApiBaseController
    {
        private readonly IDataBrowserMemoryCache _dataBrowserMemoryCache;

        private readonly int _defaultDataflowCacheTTL = 3600;
        private readonly ILogger<GenericController> _logger;
        private readonly IRequestContext _requestContext;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceProvider _serviceProvider;

        public CacheController(ILogger<GenericController> logger,
            IRequestContext requestContext,
            IMediatorService mediatorService,
            IDataBrowserMemoryCache dataBrowserMemoryCache,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider)
            : base(mediatorService)
        {
            _logger = logger;
            _requestContext = requestContext;
            _dataBrowserMemoryCache = dataBrowserMemoryCache;
            _loggerFactory = loggerFactory;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        ///     Create new node.
        /// </summary>
        /// <response code="204">Clear memory cache</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPost("MemoryCache/Clear")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public ActionResult ClearMemoryCache([FromServices] IDataBrowserMemoryCache dataBrowserMemoryCache)
        {
            if (dataBrowserMemoryCache == null)
            {
                var resultNoCache = new ContentResult();
                resultNoCache.ContentType = "application/text";
                resultNoCache.Content = "Memory cache not configured";
                resultNoCache.StatusCode = 404;
                return resultNoCache;
            }

            dataBrowserMemoryCache.ClearCacheAsync();

            var result = new ContentResult();
            result.StatusCode = 204;
            return result;
        }

        /// <summary>
        ///     Clear cache for catalog tree.
        /// </summary>
        /// <response code="204">Clear memory cache</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPost("MemoryCache/Clear/CatalogTree/Nodes/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ManageCache)]
        public async Task<ActionResult> ClearMemoryCacheNodeCatalogTree(
            [FromServices] IDataBrowserMemoryCache dataBrowserMemoryCache, int nodeId)
        {
            if (dataBrowserMemoryCache == null)
            {
                var resultNoCache = new ContentResult();
                resultNoCache.ContentType = "application/text";
                resultNoCache.Content = "Memory cache not configured";
                resultNoCache.StatusCode = 404;
                return resultNoCache;
            }

            try
            {
                await dataBrowserMemoryCache.ClearNodeCacheAsync(nodeId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during clear catalog cache", ex);
                throw;
            }

            var result = new ContentResult();
            result.StatusCode = 204;
            return result;
        }

        /// <summary>
        ///     Clear cache for catalog tree.
        /// </summary>
        /// <response code="204">Clear memory cache</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPost("MemoryCache/Clear/CatalogTree/NodesCode/{nodeCode}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ClearMemoryCacheNodeCatalogTreeFromNodeCode(
                            [FromServices] IRepository<Node> repository,
                            [FromServices] IFilterNode filterNode,
                            [FromServices] IDataBrowserMemoryCache dataBrowserMemoryCache, 
                            string nodeCode)
        {
            if (dataBrowserMemoryCache == null)
            {
                var resultNoCache = new ContentResult();
                resultNoCache.ContentType = "application/text";
                resultNoCache.Content = "Memory cache not configured";
                resultNoCache.StatusCode = 404;
                return resultNoCache;
            }

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

            if (!filterNode.CheckPermissionNodeManageCache(nodeId))
            {
                var resultForbidden = new ContentResult();
                resultForbidden.ContentType = "application/text";
                resultForbidden.Content = "Forbidden";
                resultForbidden.StatusCode = 403;
                return resultForbidden;
            }

            await dataBrowserMemoryCache.ClearNodeCacheAsync(nodeId);

            var result = new ContentResult();
            result.StatusCode = 204;
            return result;
        }

        /// <summary>
        ///     Clear all dataflow cache.
        /// </summary>
        /// <response code="204">Clear Dataflow cache</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPost("DataflowCache/ClearAll/Nodes/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ManageCache)]
        public async Task<ActionResult> ClearDataCache([FromServices] IDataflowDataCache dataMemoryCache)
        {
            if (dataMemoryCache == null)
            {
                var resultNoCache = new ContentResult();
                resultNoCache.ContentType = "application/text";
                resultNoCache.Content = "Dataflow cache not configured";
                resultNoCache.StatusCode = 404;
                return resultNoCache;
            }

            await dataMemoryCache.ClearCacheDataflowDataAsync(_requestContext.NodeId);

            var result = new ContentResult();
            result.StatusCode = 204;
            return result;
        }

        /// <summary>
        ///     Create new node.
        /// </summary>
        /// <response code="204">Clear Dataflow cache</response>
        /// <response code="400">Bad request.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPost("DataflowCache/Clear/{cacheInfoId}/Nodes/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ManageCache)]
        public async Task<ActionResult> ClearDataCache(Guid cacheInfoId,
            [FromServices] IDataflowDataCache dataMemoryCache)
        {
            if (dataMemoryCache == null)
            {
                var resultNoCache = new ContentResult();
                resultNoCache.ContentType = "application/text";
                resultNoCache.Content = "Dataflow cache not configured";
                resultNoCache.StatusCode = 404;
                return resultNoCache;
            }

            await dataMemoryCache.ClearSingleItemCache(cacheInfoId, _requestContext.NodeId);

            var result = new ContentResult();
            result.StatusCode = 204;
            return result;
        }

        /// <summary>
        ///     Get dataflow data cache info.
        /// </summary>
        /// <response code="200">Get all cache for specific node</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("DataflowCache/DataflowData/Nodes/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DataflowDataCacheInfo>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ManageCache)]
        public async Task<ActionResult> GetAllDataflowDataCacheInfoNode(int nodeId,
            [FromServices] IDataflowDataCache dataflowDataCache)
        {
            _logger.LogDebug("START GetAllDataflowDataCacheInfoNode");

            var nodeResult = await QueryAsync(new NodeByIdQuery(nodeId));
            if (nodeResult == null)
                return new ContentResult
                {
                    ContentType = "application/text",
                    Content = "Node not found",
                    StatusCode = StatusCodes.Status404NotFound
                };

            NodeCatalogModelView nodeCatalogModelView = null;
            var catalogTreeJson =
                await GetTreeHelper.GetCatalogTreeAsync(_mediatorService, _dataBrowserMemoryCache, _requestContext, _loggerFactory, _serviceProvider);
            if (!string.IsNullOrWhiteSpace(catalogTreeJson))
                nodeCatalogModelView = JsonConvert.DeserializeObject<NodeCatalogModelView>(catalogTreeJson);

            var filtered = new List<DataflowDataCacheInfo>();
            var nodeResults = await dataflowDataCache.GetInfoFromNodeId(_requestContext.NodeId);
            if (catalogTreeJson == null)
                nodeCatalogModelView = new NodeCatalogModelView
                {
                    DatasetMap = new Dictionary<string, DatasetModelView>(),
                    DatasetUncategorized = new List<DatasetUncategorizedModelView>()
                };

            //default cache TTL
            var dataflowTTL = nodeResult.TtlDataflow != null ? nodeResult.TtlDataflow.Value : _defaultDataflowCacheTTL;

            var cachedDatasetIdSet = new HashSet<string>();
            foreach (var item in nodeResults)
            {
                var findTitle = false;
                var actualDatasetId = item.DataflowId.Replace("+", ",", StringComparison.InvariantCultureIgnoreCase);
                if (nodeCatalogModelView.DatasetMap != null &&
                    nodeCatalogModelView.DatasetMap.ContainsKey(actualDatasetId))
                {
                    item.Title = nodeCatalogModelView.DatasetMap[actualDatasetId].Title;
                    filtered.Add(item);
                    cachedDatasetIdSet.Add(actualDatasetId);
                    findTitle = true;
                }
                else if (nodeCatalogModelView.DatasetUncategorized != null)
                {
                    var datasetInfo =
                        nodeCatalogModelView.DatasetUncategorized.FirstOrDefault(i =>
                            i.Identifier.Equals(actualDatasetId));
                    if (datasetInfo != null)
                    {
                        item.Title = datasetInfo.Title;
                        filtered.Add(item);
                        cachedDatasetIdSet.Add(actualDatasetId);
                        findTitle = true;
                    }
                }

                if (!findTitle)
                {
                    item.Title = item.DataflowId;
                    filtered.Add(item);
                    cachedDatasetIdSet.Add(actualDatasetId);
                }
            }

            //add categorized dataflows
            if (nodeCatalogModelView.DatasetMap != null)
            {
                foreach (var dataflowEntry in nodeCatalogModelView.DatasetMap)
                {
                    var actualDatasetId = dataflowEntry.Key;
                    if (!cachedDatasetIdSet.Contains(actualDatasetId))
                    {
                        var fakeCachedDatasetEntry =
                            CreateDataflowDataCacheInfoFromModelView(dataflowEntry.Value, actualDatasetId, dataflowTTL);
                        filtered.Add(fakeCachedDatasetEntry);
                        cachedDatasetIdSet.Add(actualDatasetId);
                    }
                }
            }

            //add uncategorized dataflows
            if (nodeCatalogModelView?.DatasetUncategorized != null)
                foreach (var dataflow in nodeCatalogModelView.DatasetUncategorized)
                {
                    var actualDatasetId = dataflow.Identifier;
                    if (!cachedDatasetIdSet.Contains(actualDatasetId))
                    {
                        var fakeCachedDatasetEntry =
                            CreateDataflowDataCacheInfoFromModelView(dataflow, actualDatasetId, dataflowTTL);
                        filtered.Add(fakeCachedDatasetEntry);
                        cachedDatasetIdSet.Add(actualDatasetId);
                    }
                }

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = DataBrowserJsonSerializer.SerializeObject(filtered);
            result.StatusCode = 200;
            return result;
        }


        private DataflowDataCacheInfo CreateDataflowDataCacheInfoFromModelView(DatasetModelView modelView,
            string identifier, int TTL)
        {
            var result = new DataflowDataCacheInfo
            {
                Id = Guid.Empty,
                NodeId = _requestContext.NodeId,
                DataflowId = identifier,
                TTL = TTL,
                CacheSize = 0,
                CachedDataflow = 0,
                CachedDataAccess = 0,
                Title = modelView.Title
            };
            return result;
        }

        /// <summary>
        ///     Change ttl value from dataflow data cache info.
        /// </summary>
        /// <response code="200">Edit cache item</response>
        /// <response code="404">Cache item not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPut("DataflowCache/DataflowData/{cacheInfoId}/Nodes/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ManageCache)]
        public async Task<ActionResult> ChangeTTLDataflowDataCache(int nodeId, Guid cacheInfoId,
            [FromServices] IDataflowDataCache dataflowDataCache,
            [FromBody] UpdateDataflowDataCacheInfoRequest updateDataflowDataCacheInfoRequest)
        {
            _logger.LogDebug("START ChangeTTLDataflowDataCache");

            var resultUpd =
                await dataflowDataCache.UpdateDataflowTTLFromNodeId(cacheInfoId,
                    updateDataflowDataCacheInfoRequest.Ttl);

            var result = new ContentResult();
            result.ContentType = resultUpd ? null : "application/text";
            result.Content = resultUpd ? null : "CacheInfo Id not found";
            result.StatusCode = resultUpd ? 204 : 404;
            return result;
        }


        /// <summary>
        ///     Change ttl value from dataflow data cache info.
        /// </summary>
        /// <response code="200">Edit cache item</response>
        /// <response code="404">Cache item not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPost("DataflowCache/DataflowData/Nodes/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ManageCache)]
        public async Task<ActionResult> CreateDataflowDataCacheInfo(int nodeId,
            [FromServices] IDataflowDataCache dataflowDataCache,
            [FromBody] DataflowDataCacheInfo dataflowDataCacheInfo)
        {
            _logger.LogDebug("START CreateDataflowDataCacheInfo");

            var resultCreate = await dataflowDataCache.CreateDataflowDataCacheInfo(dataflowDataCacheInfo);

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = resultCreate != null
                ? DataBrowserJsonSerializer.SerializeObject(resultCreate)
                : "Cache info was not created";
            result.StatusCode = resultCreate != null ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;
            return result;
        }

        /// <summary>
        ///     Change ttl value from dataflow data cache info.
        /// </summary>
        /// <response code="200">Edit cache item</response>
        /// <response code="404">Cache item not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("DataflowCache/Export/Nodes/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ManageCache)]
        public async Task<ActionResult> CacheExport(int nodeId, [FromServices] IDataflowDataCache dataflowDataCache)
        {
            _logger.LogDebug("START CacheExport");

            var exports = await dataflowDataCache.ExportAccessNumber(nodeId);

            var fileTmp = DataBrowserDirectory.GetTempFileName(DataBrowserDirectory.TempFileType.Download);
            using (var writer = new StreamWriter(fileTmp))
            {
                using (var csvWriter = new CsvWriter(writer))
                {
                    csvWriter.Configuration.Delimiter = ";";
                    csvWriter.Configuration.HasHeaderRecord = true;
                    csvWriter.Configuration.AutoMap<DataflowDataCacheExport>();
                    csvWriter.WriteRecords(exports);
                }
            }

            Stream stream = System.IO.File.OpenRead(fileTmp);
            var mimeType = "text/csv";
            return new FileStreamResult(stream, mimeType)
            {
                FileDownloadName = $"Dataflow_NodeId{nodeId}_Export.csv"
            };
        }

        /// <summary>
        ///     Change ttl value from dataflow data cache info.
        /// </summary>
        /// <response code="200">Edit cache item</response>
        /// <response code="404">Cache item not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPost("DataflowCache/RefreshAll/Nodes/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PolicyName.ServiceUser)]
        public async Task<ActionResult> RefreshAll(int nodeId,
            [FromServices] IDataflowDataCacheGenerator dataflowDataCacheGenerator)
        {
            _logger.LogDebug("START RefreshAll");

            await dataflowDataCacheGenerator.RefreshAllDataflowAsync();

            var result = new ContentResult();
            result.ContentType = null;
            result.Content = null;
            result.StatusCode = 204;
            return result;
        }

        /// <summary>
        ///     Clear all caches for specific node
        /// </summary>
        /// <response code="200">Edit cache item</response>
        /// <response code="404">Cache item not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPost("Caches/NodesCode/{nodeCode}/ClearDataflow/{dataflowId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<ActionResult> ClearSingleDataflowCacheFromCode([FromServices] IRepository<Node> repository,
                                                        [FromServices] IDataflowDataCache dataflowDataCache,
                                                        [FromServices] IDataBrowserMemoryCache dataBrowserMemoryCache,
                                                        [FromServices] IFilterNode filterNode,
                                                        string nodeCode,
                                                        string dataflowId)
        {
            _logger.LogDebug($"START ClearSingleDataflowCache nodeCode: {nodeCode}");

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

            if (!filterNode.CheckPermissionNodeManageCache(nodeId))
            {
                var resultForbidden = new ContentResult();
                resultForbidden.ContentType = "application/text";
                resultForbidden.Content = "Forbidden";
                resultForbidden.StatusCode = 403;
                return resultForbidden;
            }

            await dataflowDataCache.ClearSingleDataflowCache(RequestAdapter.ConvertDataflowUriToDataflowId(dataflowId), nodeId);

            var result = new ContentResult();
            result.ContentType = "application/text";
            result.Content = null;
            result.StatusCode = StatusCodes.Status204NoContent;
            return result;
        }

        /// <summary>
        ///     Clear all caches for specific node
        /// </summary>
        /// <response code="200">Edit cache item</response>
        /// <response code="404">Cache item not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPost("Caches/Nodes/{nodeId}/ClearDataflow/{dataflowId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<ActionResult> ClearSingleDataflowCacheFromId([FromServices] IRepository<Node> repository,
                                                        [FromServices] IDataflowDataCache dataflowDataCache,
                                                        [FromServices] IDataBrowserMemoryCache dataBrowserMemoryCache,
                                                        [FromServices] IFilterNode filterNode,
                                                        int nodeId,
                                                        string dataflowId)
        {
            _logger.LogDebug($"START ClearSingleDataflowCache nodeId: {nodeId}");

            var node = await repository.GetByIdAsync(nodeId);
            if (node == null)
            {
                var resultNodFound = new ContentResult();
                resultNodFound.ContentType = "application/text";
                resultNodFound.Content = "Node not found";
                resultNodFound.StatusCode = 404;
                return resultNodFound;
            }

            if (!filterNode.CheckPermissionNodeManageCache(nodeId))
            {
                var resultForbidden = new ContentResult();
                resultForbidden.ContentType = "application/text";
                resultForbidden.Content = "Forbidden";
                resultForbidden.StatusCode = 403;
                return resultForbidden;
            }

            await dataflowDataCache.ClearSingleDataflowCache(RequestAdapter.ConvertDataflowUriToDataflowId(dataflowId), nodeId);

            var result = new ContentResult();
            result.ContentType = "application/text";
            result.Content = null;
            result.StatusCode = StatusCodes.Status204NoContent;
            return result;
        }

        /// <summary>
        ///     Clear all caches for specific node
        /// </summary>
        /// <response code="200">Edit cache item</response>
        /// <response code="404">Cache item not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpPost("Caches/NodesCode/{nodeCode}/ClearAll")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<ActionResult> ClearAllCaches([FromServices] IRepository<Node> repository,
                                                        [FromServices] IDataBrowserCachesService dataBrowserCachesService,
                                                        [FromServices] IFilterNode filterNode,
                                                        string nodeCode)
        {
            _logger.LogDebug("START ClearAllCaches");

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

            if (!filterNode.CheckPermissionNodeManageCache(nodeId))
            {
                var resultForbidden = new ContentResult();
                resultForbidden.ContentType = "application/text";
                resultForbidden.Content = "Forbidden";
                resultForbidden.StatusCode = 403;
                return resultForbidden;
            }

            await dataBrowserCachesService.ClearNodeCacheAsync(nodeId);

            var result = new ContentResult();
            result.ContentType = "application/text";
            result.Content = null;
            result.StatusCode = StatusCodes.Status204NoContent;
            return result;
        }
    }
}
