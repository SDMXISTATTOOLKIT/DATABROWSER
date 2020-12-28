using DataBrowser.Interfaces.EndPoint;
using EndPointConnector.Interfaces.JsonStat;
using EndPointConnector.Interfaces.Sdmx;
using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.Interfaces.Sdmx.Nsi;
using EndPointConnector.Interfaces.Sdmx.Nsi.Get;
using EndPointConnector.Models;
using EndPointConnector.Models.Dto;
using EndPointConnector.ParserSdmx;
using Microsoft.Extensions.Logging;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Engine;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Sister.EndPointConnector.Sdmx.Connector.Nsi
{
    public class NsiConnector : INsiConnector
    {
        private readonly EndPointSdmxConfig _endPointSDMXNodeConfig;
        private readonly IFromSdmxJsonToJsonStatConverterFactory _fromSDMXJsonToJStatConverterFactory;
        private readonly IFromSdmxXmlToJsonStatConverterFactory _fromSdmxXmlToJStatConverterFactory;
        private readonly ILogger<NsiConnector> _logger;
        private readonly INsiGet _nsiGet;
        private readonly ISdmxCache _sdmxCache;
        private readonly SdmxParser _sdmxParser;

        public NsiConnector(INsiGet nsiGet,
            SdmxEndPointCostant.ConnectorType endPointType,
            ILoggerFactory loggerFactory,
            ISdmxCache sdmxCache,
            EndPointSdmxConfig endPointSDMXNodeConfig,
            IFromSdmxXmlToJsonStatConverterFactory fromSdmxXmlToJsonStatConverterFactory,
            IFromSdmxJsonToJsonStatConverterFactory fromSDMXJsonToJStatConverterFactory)
        {
            _nsiGet = nsiGet;
            EndPointType = endPointType;
            _sdmxParser = new SdmxParser(loggerFactory);
            _logger = loggerFactory.CreateLogger<NsiConnector>();
            _sdmxCache = sdmxCache;
            _endPointSDMXNodeConfig = endPointSDMXNodeConfig;
            _fromSdmxXmlToJStatConverterFactory = fromSdmxXmlToJsonStatConverterFactory;
            _fromSDMXJsonToJStatConverterFactory = fromSDMXJsonToJStatConverterFactory;
        }

        public SdmxEndPointCostant.ConnectorType EndPointType { get; }

        public EndPointCustomAnnotationConfig EndPointCustomAnnotationConfig =>
            _endPointSDMXNodeConfig?.AnnotationConfig;

        public EndPointSdmxConfig EndPointConfig => _endPointSDMXNodeConfig;

        public async Task<ISdmxObjects> GetArtefactAsync(SdmxStructureEnumType type, string id, string agency,
            string version, StructureReferenceDetailEnumType refDetail = StructureReferenceDetailEnumType.None,
            string respDetail = "", bool useCache = false, bool includeCrossReference = true, bool orderItems = false)
        {
            return await _nsiGet.GetArtefactAsync(type, id, agency, version, refDetail, respDetail, useCache,
                includeCrossReference, orderItems);
            //return _sdmxParser.GetSdmxObjectsFromNsiSdmxXml(responseXml, true, true, SdmxSchemaEnumType.VersionTwoPointOne);
        }

        public async Task<ISdmxObjects> SendQueryStructureRequestAsync(IEnumerable<IStructureReference> references,
            bool resolveReferences)
        {
            return await _nsiGet.SendQueryStructureRequestV20Async(references, resolveReferences);
        }

        public async Task<NodeCatalogDto> GetNodeCatalogAsync(string lang, bool useCache = false)
        {
            _logger.LogDebug("START GetNodeCatalogAsync");
            NodeCatalogDto nodeCatalogDto;
            var keyCache = _sdmxCache.CreateKey(nameof(GetNodeCatalogAsync), lang, _endPointSDMXNodeConfig.Code);
            if (_sdmxCache != null)
            {
                nodeCatalogDto = await _sdmxCache.GetJsonAsync<NodeCatalogDto>(keyCache);
                if (nodeCatalogDto != null)
                {
                    _logger.LogDebug("END from cache");
                    return nodeCatalogDto;
                }
            }

            _logger.LogDebug("GetDataForTreeAsync");
            var sdmxObjects = await _nsiGet.GetDataForTreeAsync(useCache);

            _logger.LogDebug("GetOrderArtefacts");

            sdmxObjects = SDMXUtils.GetOrderArtefacts(sdmxObjects, _endPointSDMXNodeConfig.AnnotationConfig, lang,
                SdmxStructureEnumType.CategoryScheme);

            _logger.LogDebug("GetTreeFromSdmx");
            nodeCatalogDto = _sdmxParser.GetTreeFromSdmx(sdmxObjects, _endPointSDMXNodeConfig);

            await _sdmxCache.SetJsonAsync(keyCache, nodeCatalogDto);

            _logger.LogDebug("END GetNodeCatalogAsync");
            return nodeCatalogDto;
        }

        public async Task<GenericResponseData<IDataReaderEngine>> GetDataflowDataAsync(IDataflowObject df,
            IDataStructureObject kf, List<FilterCriteria> filterCriteria, bool useAttr, bool useCache = false)
        {
            //NEED CACHE ?
            return await _nsiGet.GetDataflowDataReaderAsync(df, kf, filterCriteria);
        }

        public async Task<GenericResponseData<XmlDocument>> GetDataflowXmlCompactDataAsync(IDataflowObject df,
            IDataStructureObject kf, List<FilterCriteria> filterCriteria, bool useCache = false)
        {
            //NEED CACHE ?
            var result = await _nsiGet.GetDataflowXmlDataAsync(df, kf, filterCriteria);
            var resultData = new GenericResponseData<XmlDocument>
            {
                Data = result.Data?.XmlDocument,
                ItemsCount = result.ItemsCount,
                ItemsFrom = result.ItemsFrom,
                ItemsMax = result.ItemsMax,
                ItemsTo = result.ItemsTo,
                LimitExceeded = result.LimitExceeded,
                Timers = result.Timers
            };
            return resultData;
        }

        public async Task<GenericResponseData<string>> GetDataflowJsonSdmxDataAsync(IDataflowObject df,
            IDataStructureObject kf, List<FilterCriteria> filterCriteria, bool useCache = false)
        {
            //TOOD NEED CACHE ?
            return await _nsiGet.GetDataflowJsonSdmxDataAsync(df, kf, filterCriteria);
        }

        public async Task<GenericResponseData<string>> GetDataflowJsonStatDataAsync(IDataflowObject df,
            IDataStructureObject kf, List<FilterCriteria> filterCriteria, ISdmxObjects extraDataflowData, bool useCache = false)
        {
            //TOOD NEED CACHE ?

            var genericDataWithTimer = new GenericResponseData<string>();


            var responseWatch = Stopwatch.StartNew();
            var operationName = "";
            if (isConfigNsiForXmlSdmxData())
            {
                var dataflowData = await _nsiGet.GetDataflowXmlDataAsync(df, kf, filterCriteria);
                operationName = "xml";
                genericDataWithTimer.ItemsCount = dataflowData.ItemsCount;
                genericDataWithTimer.ItemsFrom = dataflowData.ItemsFrom;
                genericDataWithTimer.ItemsTo = dataflowData.ItemsTo;
                genericDataWithTimer.ItemsMax = dataflowData.ItemsMax;
                genericDataWithTimer.LimitExceeded = dataflowData.LimitExceeded;
                genericDataWithTimer.Timers = dataflowData.Timers ?? new Dictionary<string, string>();
                if (dataflowData?.Data?.XmlDocument != null)
                {
                    try
                    {
                        if (extraDataflowData == null)
                        {
                            _logger.LogDebug("extraDataflowData is null, get from NSI");
                            extraDataflowData = await _nsiGet.GetDataflowWithUsedData(df);
                        }

                        var dataStuctureFromRequest = extraDataflowData?.DataStructures?.FirstOrDefault();
                        var dataStucture = dataStuctureFromRequest ?? kf;

                        responseWatch.Restart();
                        var config = GenerateJsonStatParserConfig(_endPointSDMXNodeConfig);
                        var converter = _fromSdmxXmlToJStatConverterFactory.GetConverter(dataflowData.Data.XmlDocument,
                            df, dataStucture, extraDataflowData?.Codelists, extraDataflowData?.ConceptSchemes,
                            _endPointSDMXNodeConfig.UserLang, config);
                        genericDataWithTimer.Data = converter.Convert();
                        responseWatch.Stop();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Convert to json stat" + ex.Message);
                        throw;
                    }
                }
            }
            else
            {
                var dataflowData = await _nsiGet.GetDataflowJsonSdmxDataAsync(df, kf, filterCriteria);
                operationName = "json";
                genericDataWithTimer.ItemsCount = dataflowData.ItemsCount;
                genericDataWithTimer.ItemsFrom = dataflowData.ItemsFrom;
                genericDataWithTimer.ItemsTo = dataflowData.ItemsTo;
                genericDataWithTimer.ItemsMax = dataflowData.ItemsMax;
                genericDataWithTimer.LimitExceeded = dataflowData.LimitExceeded;
                genericDataWithTimer.Timers = dataflowData.Timers ?? new Dictionary<string, string>();
                if (dataflowData?.Data != null)
                {
                    try
                    {
                        responseWatch.Restart();
                        var config = GenerateJsonStatParserConfig(_endPointSDMXNodeConfig);
                        var converter = _fromSDMXJsonToJStatConverterFactory.GetConverter(dataflowData.Data,
                            _endPointSDMXNodeConfig.UserLang, config, kf);
                        genericDataWithTimer.Data = converter.Convert();
                        responseWatch.Stop();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Convert to json stat" + ex.Message);
                        throw;
                    }
                }
            }


            genericDataWithTimer.Timers.Add($"{operationName}ToJsonStat",
                $"{responseWatch.ElapsedMilliseconds}ms");

            return genericDataWithTimer;
        }


        public async Task<ISdmxObjects> GetCodeListCostraintAsync(IDataflowObject dataflow, IDataStructureObject dsd,
            string component, bool useCache = false, bool orderItems = false)
        {
            return await _nsiGet.GetCodeListCostraintAsync(dataflow, dsd, component, useCache, orderItems);
        }

        public async Task<ISdmxObjects> GetCategorySchemesAndCategorisationsAsync(bool useCache = false)
        {
            return await _nsiGet.GetCategorySchemesAndCategorisationsAsync(useCache);
        }

        public async Task<ISdmxObjects> GetDataflowsAsync(bool useCache = false)
        {
            return await _nsiGet.GetDataflowsAsync(useCache);
        }

        public async Task<ISdmxObjects> GetCodeListCostraintFilterAsync(IDataflowObject dataflow,
            IDataStructureObject dsd, string criteriaId, List<FilterCriteria> filterComponents, bool useCache = false,
            bool orderItems = false)
        {
            return await _nsiGet.GetCodeListCostraintFilterAsync(dataflow, dsd, criteriaId, filterComponents, useCache,
                orderItems);
        }

        public async Task<long> GetDataflowObservationCountAsync(IDataflowObject dataflow, IDataStructureObject dsd,
            List<FilterCriteria> filterComponents, bool useCache = false)
        {
            return await _nsiGet.GetDataflowObservationCountAsync(dataflow, dsd, filterComponents);
        }

        public async Task<GenericResponseData<string>> DownloadDataflowsAsync(IDataflowObject df,
            IDataStructureObject kf, List<FilterCriteria> filterCriteria, string downloadFormat,
            int? maxObservations = null)
        {
            return await _nsiGet.DownloadDataflowsAsync(df, kf, filterCriteria, downloadFormat, maxObservations);
        }

        public async Task<ISdmxObjects> GetOnlyDataflowsValidForCatalogWithDsdAsync(bool useCache = false)
        {
            return await _nsiGet.GetOnlyDataflowsValidForCatalogWithDsdAndCodelistAsync(useCache);
        }

        protected static IFromSDMXToJsonStatConverterConfig GenerateJsonStatParserConfig(EndPointSdmxConfig endPointSDMXNodeConfig)
        {
            var config = DefaultJsonStatConverterConfig.GetNew();

            if (!string.IsNullOrEmpty(endPointSDMXNodeConfig.AnnotationConfig.GEO_ID))
            {
                config.GeoAnnotationId = endPointSDMXNodeConfig.AnnotationConfig.GEO_ID;
            }

            if (!string.IsNullOrEmpty(endPointSDMXNodeConfig.AnnotationConfig.NOT_DISPLAYED))
            {
                config.NotDisplayedAnnotationId = endPointSDMXNodeConfig.AnnotationConfig.NOT_DISPLAYED;
            }

            if (!string.IsNullOrEmpty(endPointSDMXNodeConfig.AnnotationConfig.ORDER_CODELIST))
            {
                config.OrderAnnotationId = endPointSDMXNodeConfig.AnnotationConfig.ORDER_CODELIST;
            }

            if (!string.IsNullOrEmpty(endPointSDMXNodeConfig.LabelDimensionTerritorial))
            {
                var labels = endPointSDMXNodeConfig.LabelDimensionTerritorial.Split(";").ToList();
                if (labels?.Count > 0)
                {
                    config.TerritorialDimensionIds = labels;
                }
            }

            if (!string.IsNullOrEmpty(endPointSDMXNodeConfig.LabelDimensionTemporal))
            {
                var labels = endPointSDMXNodeConfig.LabelDimensionTemporal.Split(";").ToList();
                if (labels?.Count > 0)
                {
                    config.TemporalDimensionIds = labels;
                }
            }

            return config;
        }

        private bool isConfigNsiForXmlSdmxData()
        {
            return _endPointSDMXNodeConfig.OptimizeCallWithSoap ||
                   _endPointSDMXNodeConfig.RestDataResponseXml ||
                   _endPointSDMXNodeConfig.EndPointType != SdmxEndPointCostant.ConnectorType.Rest;
        }

        public async Task<ISdmxObjects> GetDataflowWithUsedData(IDataflowObject df)
        {
            return await _nsiGet.GetDataflowWithUsedData(df);
        }
    }
}