using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBrowser.AC.Utility;
using DataBrowser.DomainServices.Interfaces;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.UseCase.Common;
using EndPointConnector.Interfaces;
using EndPointConnector.Models;
using Microsoft.Extensions.Logging;

namespace DataBrowser.UseCase
{
    public class DownloadDataflowFromNodeEndPointUseCase : IUseCaseHandler<DownloadDataflowFromNodeEndPointRequest,
        DownloadDataflowFromNodeEndPointResponse>
    {
        private readonly IDataBrowserMemoryCache _dataBrowserMemoryCache;
        private readonly IEndPointConnectorFactory _endPointConnectorFactory;
        private readonly ILogger<DownloadDataflowFromNodeEndPointUseCase> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly INodeConfigService _nodeConfigService;
        private readonly IRequestContext _requestContext;

        public DownloadDataflowFromNodeEndPointUseCase(ILogger<DownloadDataflowFromNodeEndPointUseCase> logger,
            IRequestContext requestContext,
            IEndPointConnectorFactory endPointConnectorFactory,
            ILoggerFactory loggerFactory,
            INodeConfigService nodeConfigService,
            IDataBrowserMemoryCache dataBrowserMemoryCache)
        {
            _logger = logger;
            _requestContext = requestContext;
            _endPointConnectorFactory = endPointConnectorFactory;
            _loggerFactory = loggerFactory;
            _nodeConfigService = nodeConfigService;
            _dataBrowserMemoryCache = dataBrowserMemoryCache;
        }

        public async Task<DownloadDataflowFromNodeEndPointResponse> Handle(
            DownloadDataflowFromNodeEndPointRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Start Handle");
            if (request == null)
            {
                _logger.LogWarning("Null UseCase");
                return null;
            }

            var originalNodeContext = RequestContextUtility.GetOriginalUseCaseRequestNodeContext(_requestContext);

            _logger.LogDebug("Get Node");
            var nodeConfig = await _nodeConfigService.GenerateNodeConfigAsync(_requestContext.NodeId);
            var endPointConnector = await _endPointConnectorFactory.Create(nodeConfig, _requestContext);
            endPointConnector.TryUseCache = true;

            Dataflow dataflow = null;
            Dsd dsd = null;
            var tupleResult = await Utility.GetDataflowWithDsdAndEndpointConnectorFromCacheAsync(request.DataflowId,
                _dataBrowserMemoryCache, _requestContext, _loggerFactory, nodeConfig, endPointConnector,
                _nodeConfigService);
            if (tupleResult.Item1)
            {
                nodeConfig = tupleResult.Item2;
                endPointConnector = tupleResult.Item3;
                dataflow = tupleResult.Item4;
                dsd = tupleResult.Item5;
            }


            if (dataflow == null ||
                dsd == null)
            {
                var resultData = await Utility.GetDataflowInfoFromEndPointWithConnectorAsync(request.DataflowId,
                    nodeConfig, endPointConnector, _loggerFactory, _requestContext, _endPointConnectorFactory,
                    _nodeConfigService);
                nodeConfig = resultData.Item1;
                endPointConnector = resultData.Item2;
                dataflow = resultData.Item3;
                dsd = resultData.Item4;
            }

            nodeConfig.MaxObservationsAfterCriteria = int.MaxValue;

            var useCaseResult = new DownloadDataflowFromNodeEndPointResponse();
            useCaseResult.IsCompressed = false;
            switch (request.DataFormat.ToUpperInvariant())
            {
                case "JSONDATA":
                    useCaseResult.ExtensionFile = ".json";
                    break;
                case "CSV":
                    useCaseResult.ExtensionFile = ".csv";
                    break;
                default:
                    useCaseResult.ExtensionFile = ".xml";
                    break;
            }

            var downloadResponse = await endPointConnector.DownloadDataflowsAsync(dataflow, dsd,
                request.DataCriterias?.ToList(), request.DataFormat, int.MaxValue);

            string fileToWriteTo = null;
            try
            {
                fileToWriteTo = DataBrowserDirectory.GetTempFileName(DataBrowserDirectory.TempFileType.Download);
                File.Delete(fileToWriteTo);
                if (request.RequireResponseFileCompress && downloadDataReturnFileWithData(downloadResponse.Data))
                {
                    fileToWriteTo += $"{request.DataflowId}.zip";
                    using (var zip = ZipFile.Open(fileToWriteTo, ZipArchiveMode.Create))
                    {
                        zip.CreateEntryFromFile(downloadResponse.Data,
                            $"{request.DataflowId}{useCaseResult.ExtensionFile}");
                    }

                    useCaseResult.IsCompressed = true;
                }
                else if (!request.RequireResponseFileCompress && downloadDataReturnFileWithData(downloadResponse.Data))
                {
                    File.Copy(downloadResponse.Data, fileToWriteTo, true);
                }
            }
            finally
            {
                if (File.Exists(downloadResponse.Data)) File.Delete(downloadResponse.Data);
            }

            downloadResponse.Data = fileToWriteTo;

            useCaseResult.Response = downloadResponse;

            RequestContextUtility.RestoreOriginalUseCaseRequestNodeContext(_requestContext, originalNodeContext.Item1,
                originalNodeContext.Item2);

            _logger.LogDebug("End Handle");
            return useCaseResult;
        }

        private bool downloadDataReturnFileWithData(string dataFile)
        {
            return !string.IsNullOrWhiteSpace(dataFile) && File.Exists(dataFile);
        }
    }
}