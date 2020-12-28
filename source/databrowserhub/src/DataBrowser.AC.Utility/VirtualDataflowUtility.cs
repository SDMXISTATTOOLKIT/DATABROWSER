using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBrowser.DomainServices.Interfaces;
using DataBrowser.Interfaces;
using EndPointConnector.Interfaces;
using EndPointConnector.Models;
using EndPointConnector.Models.Dto;
using Microsoft.Extensions.Logging;

namespace DataBrowser.AC.Utility
{
    public static class VirtualDataflowUtility
    {
        public static async Task<Tuple<bool, IEndPointConnector, INodeConfiguration, ArtefactContainer>>
            PopolateDataForDataflowIfVirtualAsync(
                Dataflow virtualDataflow,
                IRequestContext requestContext,
                IEndPointConnectorFactory endPointConnectorFactory,
                INodeConfiguration originalNodeConfig,
                ILoggerFactory loggerFactory,
                INodeConfigService nodeConfigService)
        {
            var _logger = loggerFactory.CreateLogger(typeof(VirtualDataflowUtility).FullName);

            _logger.LogDebug("START PopolateDataForDataflowIfVirtualAsync");
            IEndPointConnector endPointConnector = null;
            var nodeConfig = originalNodeConfig;
            ArtefactContainer containerArtefact = null;
            var antiLoop = 0;
            var isVirtual = virtualDataflow.DataflowType;
            var isVirtualDataflow = false;
            while (isVirtual == DataflowType.IsVirtual && antiLoop < 5)
            {
                _logger.LogDebug("Is virtual");
                isVirtualDataflow = true;
                nodeConfig = await createVirtualConfigurationAsync(virtualDataflow, requestContext, nodeConfig, _logger,
                    nodeConfigService);
                endPointConnector = await endPointConnectorFactory.Create(nodeConfig, requestContext);
                endPointConnector.TryUseCache = false;
                containerArtefact = await endPointConnector.GetArtefactAsync(ArtefactType.ArtefactEnumType.Dataflow,
                    virtualDataflow.Id, ArtefactType.ReferenceDetailEnumType.Children);
                if (containerArtefact.Dataflows?.FirstOrDefault() == null)
                {
                    isVirtualDataflow = false;
                    containerArtefact = null;
                    _logger.LogDebug($"not found real dataflow for {virtualDataflow.Id} in loop number {antiLoop}");
                    break;
                }

                isVirtual = containerArtefact.Dataflows.First().DataflowType;
                endPointConnector.TryUseCache = true;
                antiLoop++;
            }

            return Tuple.Create(isVirtualDataflow, endPointConnector, nodeConfig, containerArtefact);
        }

        public static async Task<ArtefactContainer> FindVirtualDataflowAndRecreateAllDataAsync(
            ArtefactContainer artefactContainer,
            IRequestContext _requestContext,
            IEndPointConnectorFactory _endPointConnectorFactory,
            INodeConfigService nodeConfigService,
            INodeConfiguration nodeConfig,
            ILoggerFactory loggerFactory)
        {
            var _logger = loggerFactory.CreateLogger(typeof(VirtualDataflowUtility).FullName);

            if (artefactContainer?.Dataflows == null) return artefactContainer;

            var containerResult = new ArtefactContainer
            {
                Codelists = artefactContainer.Codelists,
                Criterias = artefactContainer.Criterias,
                Dataflows = new List<Dataflow>(),
                Dsds = new List<Dsd>()
            };

            foreach (var dataflowVirtual in artefactContainer.Dataflows)
            {
                if (dataflowVirtual.DataflowType != DataflowType.IsVirtual)
                {
                    containerResult.Dataflows.Add(dataflowVirtual);
                    addDsdLinkedToDataflow(artefactContainer, containerResult, dataflowVirtual);
                    continue;
                }

                try
                {
                    ArtefactContainer containerArtefact = null;

                    var tuple = await PopolateDataForDataflowIfVirtualAsync(
                        dataflowVirtual,
                        _requestContext,
                        _endPointConnectorFactory,
                        nodeConfig,
                        loggerFactory,
                        nodeConfigService);
                    if (!tuple.Item1)
                    {
                        _logger.LogWarning(
                            $"Virtual dataflow {dataflowVirtual.Id} not found, can't recreate dataflow data");
                        continue;
                    }

                    containerArtefact = tuple.Item4;
                    var dataflowReal = containerArtefact?.Dataflows?.First();
                    if (dataflowReal == null)
                    {
                        _logger.LogWarning(
                            $"Read dataflow from virtual dataflow {dataflowVirtual.Id} not found, can't recreate dataflow data");
                        continue;
                    }

                    dataflowReal.DataflowType = DataflowType.DerivedFromVirtual;
                    dataflowReal.DerivedDataflowData = new Dataflow.DerivedDataflowdata
                    { EndPointUrl = nodeConfig.EndPoint, NodeCode = nodeConfig.Code };

                    containerResult.Dataflows.Add(dataflowReal);
                    addDsdLinkedToDataflow(artefactContainer, containerResult, dataflowVirtual);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error to convert virtual dataset {dataflowVirtual.Id}. Error: {ex.Message}");
                }
            }

            return containerResult;
        }

        private static void addDsdLinkedToDataflow(ArtefactContainer artefactContainer,
            ArtefactContainer containerResult, Dataflow dataflow)
        {
            var dsd = artefactContainer.Dsds.FirstOrDefault(i =>
                dataflow.DataStructureRef?.Id != null && i.Id.Equals(dataflow.DataStructureRef.Id,
                    StringComparison.InvariantCultureIgnoreCase));
            if (dsd == null ||
                containerResult.Dsds.Any(i => i.Id.Equals(dsd.Id, StringComparison.InvariantCultureIgnoreCase)))
                return;
            containerResult.Dsds.Add(dsd);
        }

        public static async Task FindVirtualDataflowAndPopolateWithRealDataAsync(List<DatasetDto> datasetDtos,
            IRequestContext _requestContext,
            IEndPointConnectorFactory _endPointConnectorFactory,
            INodeConfigService nodeConfigService,
            ILoggerFactory loggerFactory,
            INodeConfiguration originalNodeConfig)
        {
            var _logger = loggerFactory.CreateLogger(typeof(VirtualDataflowUtility).FullName);

            if (datasetDtos == null) return;

            var removeDataflowFromCatalog = new List<DatasetDto>();

            foreach (var dataset in datasetDtos)
            {
                if (dataset.DataflowType != DataflowType.IsVirtual) continue;

                try
                {
                    var dataflow = new Dataflow
                    {
                        DataflowType = dataset.DataflowType,
                        VirtualType = dataset.VirtualType,
                        VirtualEndPointSoapV21 = dataset.VirtualEndPointSoapV21,
                        VirtualEndPointSoapV20 = dataset.VirtualEndPointSoapV20,
                        VirtualEndPointRest = dataset.VirtualEndPointRest,
                        VirtualSource = dataset.VirtualSource,
                        Id = dataset.Identifier
                    };

                    var tuple = await PopolateDataForDataflowIfVirtualAsync(
                        dataflow,
                        _requestContext,
                        _endPointConnectorFactory,
                        originalNodeConfig,
                        loggerFactory,
                        nodeConfigService);

                    INodeConfiguration endPointConfig = null;
                    IEndPointConnector endPointConnector = null;
                    ArtefactContainer containerArtefact = null;
                    if (tuple.Item1)
                    {
                        endPointConnector = tuple.Item2;
                        endPointConfig = tuple.Item3;
                        containerArtefact = tuple.Item4;
                    }

                    dataflow = containerArtefact?.Dataflows?.First();
                    if (dataflow == null)
                    {
                        _logger.LogWarning(
                            $"Virtual dataflow {dataset.Identifier} not found, can't overwrite the value of catalog tree with virtual data");
                        continue;
                    }
                    if ( 
                        (dataflow?.HiddenFromCatalog?.Values != null && dataflow.HiddenFromCatalog.Values.Any(i => i == true)) ||
                        (dataflow?.NonProductionDataflow?.Values != null && dataflow.NonProductionDataflow.Values.Any(i => i == true) && !originalNodeConfig.ShowDataflowNotInProduction)
                        )
                    {
                        removeDataflowFromCatalog.Add(dataset);
                    }

                    dataset.AttachedDataFiles = dataflow.AttachedDataFiles;
                    dataset.DefaultNote = dataflow.DefaultNote;
                    //dataset.Descriptions = dataflow.Descriptions;
                    dataset.Identifier = dataflow.Id;
                    dataset.Keywords = dataflow.Keywords;
                    dataset.LayoutFilter = dataflow.LayoutFilter;
                    dataset.MetadataUrl = dataflow.MetadataUrl;
                    dataset.Source = dataflow.DataflowSource;
                    //dataset.Titles = dataflow.Names;

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error to convert virtual dataset {dataset.Identifier}. Error: {ex.Message}");
                }
            }

            foreach (var item in removeDataflowFromCatalog)
            {
                datasetDtos.Remove(item);
            }
            
        }

        private static async Task<INodeConfiguration> createVirtualConfigurationAsync(Dataflow dataflow,
            IRequestContext requestContext,
            INodeConfiguration originalNodeConfig,
            ILogger _logger,
            INodeConfigService nodeConfigService)
        {
            if (dataflow == null) return null;

            _logger.LogDebug(
                $"dataflow Type:{dataflow.VirtualType}\tVirtualEndPointSoapV21:{dataflow.VirtualEndPointSoapV21}\tVirtualEndPointSoapV20:{dataflow.VirtualEndPointSoapV20}\tVirtualEndPointRest:{dataflow.VirtualEndPointRest}");


            switch (dataflow.VirtualType)
            {
                case VirtualType.SoapV21:
                    //endPointConfig.Type = "SDMX-SOAPv21";
                    //endPointConfig.EndPoint = dataflow.VirtualEndPointSoapV21;
                    originalNodeConfig.Type = "SDMX-REST";
                    originalNodeConfig.EndPoint = fixVirtualEndPoint(dataflow.VirtualEndPointSoapV21);
                    break;
                case VirtualType.SoapV20:
                    //endPointConfig.Type = "SDMX-SOAPv20";
                    //endPointConfig.EndPoint = dataflow.VirtualEndPointSoapV20;
                    originalNodeConfig.Type = "SDMX-REST";
                    originalNodeConfig.EndPoint = fixVirtualEndPoint(dataflow.VirtualEndPointSoapV20);
                    break;
                case VirtualType.Node:
                    originalNodeConfig = await nodeConfigService.GenerateNodeConfigAsync(dataflow.VirtualSource);
                    if (originalNodeConfig == null)
                        throw new Exception($"VirtualSource {dataflow.VirtualSource} not found");
                    ChangeRequestContextForVirtualDataflow(requestContext, originalNodeConfig);
                    break;
                case VirtualType.Rest:
                default:
                    originalNodeConfig.Type = "SDMX-REST";
                    originalNodeConfig.EndPoint = dataflow.VirtualEndPointRest;
                    break;
            }

            _logger.LogDebug($"Type:{originalNodeConfig.Type}\tEndPoint:{originalNodeConfig.EndPoint}\t");

            return originalNodeConfig;
        }

        private static string fixVirtualEndPoint(string source)
        {
            var splits = source.Split('/');

            if (splits.Length <= 0) return source;
            splits[splits.Length - 1] = "rest";

            return string.Join('/', splits);
        }

        public static void ChangeRequestContextForVirtualDataflow(IRequestContext requestContext,
            INodeConfiguration endPointConfig)
        {
            requestContext.OverwriteNodeId(endPointConfig.NodeId);
            requestContext.OverwriteNodeCode(endPointConfig.Code);
        }
    }
}