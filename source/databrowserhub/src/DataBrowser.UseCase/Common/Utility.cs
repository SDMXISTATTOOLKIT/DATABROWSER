using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBrowser.AC.Exceptions;
using DataBrowser.AC.Utility;
using DataBrowser.DomainServices.Interfaces;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Cache.Key;
using EndPointConnector.Interfaces;
using EndPointConnector.Models;
using Microsoft.Extensions.Logging;

namespace DataBrowser.UseCase.Common
{
    public static class Utility
    {
        public static Criteria CalculateHiddenCriteria(Criteria itemCriteria, Dataflow dataflow, Dsd dsd,string lang)
        {
            Criteria hiddenCriterias = null;
            if (dataflow.NotDisplay != null)
            {
                var criteria = dataflow.NotDisplay.FirstOrDefault(i =>
                    i.Key.Equals(lang, StringComparison.InvariantCultureIgnoreCase));
                if (criteria.Equals(default(KeyValuePair<string, List<Criteria>>)))
                    criteria = dataflow.NotDisplay.FirstOrDefault();

                foreach (var item in criteria.Value)
                    if (itemCriteria.Id.Equals(item.Id, StringComparison.InvariantCultureIgnoreCase))
                    {
                        hiddenCriterias = item;
                        break;
                    }
            }

            //dataflow annotation overrides dsd ones
            if (dsd.NotDisplay != null && hiddenCriterias == null)
            {
                var criteria = dsd.NotDisplay.FirstOrDefault(i =>
                    i.Key.Equals(lang, StringComparison.InvariantCultureIgnoreCase));
                if (criteria.Equals(default(KeyValuePair<string, List<Criteria>>)))
                    criteria = dsd.NotDisplay.FirstOrDefault();

                foreach (var item in criteria.Value)
                    if (itemCriteria.Id.Equals(item.Id, StringComparison.InvariantCultureIgnoreCase))
                    {
                        hiddenCriterias = item;
                        break;
                    }
            }

            return hiddenCriterias;
        }

        public static List<Criteria> RemoveHiddenCriteria(Dataflow dataflow, Dsd dsd, List<Criteria> criterias,
            string lang)
        {
            foreach (var itemCriteria in criterias)
            {
                Criteria hiddenCriterias = CalculateHiddenCriteria(itemCriteria, dataflow, dsd, lang);                

                if (hiddenCriterias == null) continue;

                var allCriteria = hiddenCriterias.Values;
                if (allCriteria == null || allCriteria.Count <= 0)
                {
                    if (itemCriteria.Values.Count == 1) itemCriteria.Values = new List<Code>();
                }
                else
                {
                    foreach (var removeItem in allCriteria)
                    {
                        var index = itemCriteria.Values.FindIndex(i =>
                            i.Id.Equals(removeItem.Id, StringComparison.InvariantCultureIgnoreCase));
                        if (index > -1) itemCriteria.Values.RemoveAt(index);
                    }
                }
            }

            return criterias;
        }

        public static async Task<Tuple<bool, INodeConfiguration, IEndPointConnector, Dataflow, Dsd, List<Codelist>>>
            GetDataflowWithDsdAndEndpointConnectorFromCacheAsync(string dataflowId,
                IDataBrowserMemoryCache dataBrowserMemoryCache,
                IRequestContext requestContext,
                ILoggerFactory loggerFactory,
                INodeConfiguration nodeConfig,
                IEndPointConnector endPointConnector,
                INodeConfigService nodeConfigService,
                bool includeCodelist = false)
        {
            var logger = loggerFactory.CreateLogger(typeof(Utility).FullName);
            if (TryUseMemoryCache(dataBrowserMemoryCache, requestContext))
            {
                logger.LogDebug("GetDataflowWithDsdAndEndpointConnectorFromCacheAsync not use cache");
                return Tuple.Create<bool, INodeConfiguration, IEndPointConnector, Dataflow, Dsd, List<Codelist>>(false, null, null,
                    null, null, null);
            }

            logger.LogDebug("GetDataflowWithDsdAndEndpointConnectorFromCacheAsync use cache");
            var containerArtefact = dataBrowserMemoryCache.Get(new DataflowWithDsdCacheKey(requestContext.NodeId));
            if (containerArtefact == null)
            {
                logger.LogDebug("GetDataflowWithDsdAndEndpointConnectorFromCacheAsync no cache key");
                return Tuple.Create<bool, INodeConfiguration, IEndPointConnector, Dataflow, Dsd, List<Codelist>>(false, null, null,
                    null, null, null);
            }

            var dataflow = containerArtefact.Dataflows.FirstOrDefault(i =>
                i.Id.Equals(dataflowId, StringComparison.InvariantCultureIgnoreCase));
            var dsd = containerArtefact.Dsds?.FirstOrDefault(i => dataflow?.DataStructureRef?.Id != null &&
                                                                  i.Id.Equals(dataflow.DataStructureRef.Id,
                                                                      StringComparison.InvariantCultureIgnoreCase));

            if (dataflow.DataflowType == DataflowType.DerivedFromVirtual &&
                dataflow.DerivedDataflowData != null)
            {
                if (!nodeConfig.Code.Equals(dataflow.DerivedDataflowData.NodeCode,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    logger.LogDebug($"change node code {nodeConfig.Code} in {dataflow.DerivedDataflowData.NodeCode}");
                    nodeConfig = await nodeConfigService.GenerateNodeConfigAsync(dataflow.DerivedDataflowData.NodeCode);
                    if (nodeConfig == null)
                    {
                        logger.LogDebug(
                            $"not found node code {dataflow.DerivedDataflowData.NodeCode} for dataflow derived from virtual {dataflow}");
                        return Tuple.Create<bool, INodeConfiguration, IEndPointConnector, Dataflow, Dsd, List<Codelist>>(false, null,
                            null, null, null, null);
                    }
                }

                if (!nodeConfig.EndPoint.Equals(dataflow.DerivedDataflowData.EndPointUrl,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    logger.LogDebug(
                        $"change EndPointUrl {nodeConfig.EndPoint} in {dataflow.DerivedDataflowData.EndPointUrl}");
                    nodeConfig.EndPoint = dataflow.DerivedDataflowData.EndPointUrl;
                }

                VirtualDataflowUtility.ChangeRequestContextForVirtualDataflow(requestContext, nodeConfig);
            }

            var found = dataflow != null && dsd != null;
            logger.LogDebug($"END GetDataflowWithDsdAndEndpointConnectorFromCacheAsync result {found}");
            return Tuple.Create(found, nodeConfig, endPointConnector, dataflow, dsd, containerArtefact?.Codelists);
        }

        public static bool TryUseMemoryCache(IDataBrowserMemoryCache dataBrowserMemoryCache,
            IRequestContext requestContext)
        {
            if (dataBrowserMemoryCache == null ||
                !requestContext.IgnoreCache)
                return false;
            return true;
        }

        public static async Task<Tuple<INodeConfiguration, IEndPointConnector, Dataflow, Dsd>>
            GetDataflowInfoFromEndPointWithConnectorAsync(
                string dataflowId,
                INodeConfiguration nodeConfig,
                IEndPointConnector endPointConnector,
                ILoggerFactory loggerFactory,
                IRequestContext requestContext,
                IEndPointConnectorFactory endPointConnectorFactory,
                INodeConfigService nodeConfigService)
        {
            var logger = loggerFactory.CreateLogger(typeof(Utility).FullName);

            var containerArtefact = await endPointConnector.GetArtefactAsync(ArtefactType.ArtefactEnumType.Dataflow,
                dataflowId, ArtefactType.ReferenceDetailEnumType.Children);
            if (containerArtefact?.Dataflows?.FirstOrDefault() == null)
            {
                logger.LogDebug($"Artefact not found {ArtefactType.ArtefactEnumType.Dataflow}\t{dataflowId}");
                throw new ClientErrorException("DATAFLOW_NOT_FOUND", "Dataflow not found");
            }

            var endPointConfigResult = nodeConfig;
            var endPointConnectorResult = endPointConnector;

            var tuple = await VirtualDataflowUtility.PopolateDataForDataflowIfVirtualAsync(
                containerArtefact.Dataflows.First(),
                requestContext,
                endPointConnectorFactory,
                nodeConfig,
                loggerFactory,
                nodeConfigService);
            //Dataflow virtual overwrite the config
            if (tuple.Item1)
            {
                endPointConnectorResult = tuple.Item2;
                endPointConfigResult = tuple.Item3;
                containerArtefact = tuple.Item4;
            }

            if (containerArtefact.Dsds?.FirstOrDefault() == null)
            {
                logger.LogDebug($"Artefact not found {ArtefactType.ArtefactEnumType.Dsd}\t{dataflowId}");
                throw new ClientErrorException("DSD_NOT_FOUND", "Dsd not found");
            }

            return Tuple.Create(endPointConfigResult, endPointConnectorResult,
                containerArtefact.Dataflows.FirstOrDefault(), containerArtefact.Dsds.FirstOrDefault());
        }
    }
}