using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DataBrowser.Interfaces.EndPoint;
using EndPointConnector.Interfaces.Sdmx;
using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.Interfaces.Sdmx.Nsi;
using EndPointConnector.Interfaces.Sdmx.Nsi.Get;
using EndPointConnector.Models;
using EndPointConnector.ParserSdmx;
using Estat.Sri.CustomRequests.Factory;
using Estat.Sri.CustomRequests.Manager;
using Estat.Sri.CustomRequests.Model;
using Microsoft.Extensions.Logging;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Engine;
using Org.Sdmxsource.Sdmx.Api.Model.Format;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference.Complex;
using Org.Sdmxsource.Sdmx.DataParser.Engine;
using Org.Sdmxsource.Sdmx.DataParser.Engine.Reader;
using Org.Sdmxsource.Sdmx.Structureparser.Model;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Util.Io;
using Sister.EndPointConnector.Sdmx.Nsi.Soap.Get;
using static EndPointConnector.Interfaces.Sdmx.Models.SdmxEndPointCostant;
using static TracertLOg.Tracing;

namespace Sister.EndPointConnector.Sdmx.Nsi.Soap
{
    public class NsiGetSoap : INsiGet
    {
        private readonly INsiEndPointHttpRequest _endPointHttpRequest;
        private readonly EndPointSdmxConfig _endPointSDMXNodeConfig;
        private readonly ILogger<NsiGetSoap> _logger;
        private readonly NsiGetArtefactSoap _nsiGetArtefact;
        private readonly NsiGetDataSoap _nsiGetData;
        private readonly NsiGetStructureSoap _nsiGetGeneric;
        private readonly INsiGetV20 _nsiGetV20;
        private readonly SdmxParser _sdmxParser;

        public NsiGetSoap(INsiEndPointHttpRequest endPointHttpRequest,
            INsiGetV20 nsiGetV20,
            EndPointSdmxConfig endPointSDMXNodeConfig,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<NsiGetSoap>();
            _nsiGetV20 = nsiGetV20;
            _sdmxParser = new SdmxParser(loggerFactory);
            _endPointSDMXNodeConfig = endPointSDMXNodeConfig;
            _endPointHttpRequest = endPointHttpRequest;
            _nsiGetArtefact = new NsiGetArtefactSoap(endPointSDMXNodeConfig, loggerFactory);
            _nsiGetGeneric = new NsiGetStructureSoap(loggerFactory);
            _nsiGetData = new NsiGetDataSoap(endPointSDMXNodeConfig, loggerFactory);
        }

        #region NsiGetArtefactSoap

        public async Task<ISdmxObjects> GetArtefactAsync(SdmxStructureEnumType type, string id, string agency,
            string version, StructureReferenceDetailEnumType refDetail = StructureReferenceDetailEnumType.None,
            string respDetail = "", bool useCache = false, bool includeCrossReference = true, bool orderItems = false)
        {
            _logger.LogDebug("START GetArtefactAsync");

            var requestQueryMessage = _nsiGetArtefact.GetArtefact(type, id, agency, version, refDetail, respDetail);

            var requestHttp = _endPointHttpRequest.CreateRequest(requestQueryMessage, type, true, includeCrossReference,
                _endPointSDMXNodeConfig.XmlResultNeedFix);
            var response = await _endPointHttpRequest.SendRequestAsync(requestHttp);

            _logger.LogDebug("GetSdmxObjectsFromNsiSdmxXml");
            var result = _sdmxParser.GetSdmxObjectsFromNsiResponse(response,
                includeCrossReference: includeCrossReference, schemaType: SdmxSchemaEnumType.VersionTwoPointOne);

            if (orderItems)
                result = SDMXUtils.GetOrderArtefacts(result, _endPointSDMXNodeConfig.AnnotationConfig,
                    _endPointSDMXNodeConfig.UserLang, SdmxStructureEnumType.CategoryScheme);

            _logger.LogDebug("END GetArtefactAsync");
            return result;
        }

        #endregion

        public Task<ISdmxObjects> GetOnlyDataflowsValidForCatalogWithDsdAndCodelistAsync(bool useCache = false)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponseData<string>> DownloadDataflowsAsync(IDataflowObject df, IDataStructureObject kf,
            List<FilterCriteria> filterCriterian, string downloadFormat, int? maxObservations = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Sends the specified <paramref name="complexStructureQuery" /> to the Web Service defined by <see cref="_config" />
        /// </summary>
        /// <param name="complexStructureQuery">The <see cref="IComplexStructureQuery" /></param>
        /// <returns>The ISdmxObjects returned by the Web Service</returns>
        private async Task<ISdmxObjects> sendQueryStructureRequestAsync(IComplexStructureQuery complexStructureQuery,
            SDMXWSFunctionV21 sdmxwsFunctionV21)
        {
            IToolIndicator toolIndicator = null;
            IStructureQueryFormat<XDocument> queryFormat = new ComplexQueryFormatV21();

            //IComplexStructureQueryFactory<XDocument> factory = new ComplexStructureQueryFactoryV21<XDocument>();
            IComplexStructureQueryFactory<XDocument> factory = new ComplexStructureQueryFactoryV21(toolIndicator);
            IComplexStructureQueryBuilderManager<XDocument> complexStructureQueryBuilderManager =
                new ComplexStructureQueryBuilderManager<XDocument>(factory);
            var wdoc = complexStructureQueryBuilderManager.BuildComplexStructureQuery(complexStructureQuery,
                queryFormat);
            var doc = new XmlDocument();
            doc.LoadXml(wdoc.ToString());

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("compose query in sendQueryStructureRequestAsync:");
                _logger.LogTrace(doc.OuterXml);
            }

            var httpRequest = _endPointHttpRequest.CreateRequest(doc, sdmxwsFunctionV21, true);
            var response = await _endPointHttpRequest.SendRequestAsync(httpRequest);

            return _sdmxParser.GetSdmxObjectsFromNsiResponse(response);
        }

        #region NsiGetStructureSoap

        public async Task<ISdmxObjects> GetDataForTreeAsync(bool useCache = false)
        {
            _logger.LogDebug("START GetDataForTreeAsync");

            ISdmxObjects responseObject;

            var request = _nsiGetGeneric.GetCategorySchemesAndCategorisations();
            responseObject = await sendQueryStructureRequestAsync(request, SDMXWSFunctionV21.GetCategoryScheme);

            foreach (var itemCatSchema in _endPointSDMXNodeConfig.CategorySchemaExcludes)
            {
                var itmSplit = itemCatSchema.Split('+');
                var itemExclude = responseObject.CategorySchemes.FirstOrDefault(i =>
                    i.AgencyId.Equals(itmSplit[0], StringComparison.InvariantCultureIgnoreCase) &&
                    i.Id.Equals(itmSplit[1], StringComparison.InvariantCultureIgnoreCase) &&
                    i.Version.Equals(itmSplit[2], StringComparison.InvariantCultureIgnoreCase));
                if (itemExclude != null) responseObject.RemoveCategoryScheme(itemExclude);
            }

            request = _nsiGetGeneric.GetDataflows();
            responseObject.Merge(await sendQueryStructureRequestAsync(request, SDMXWSFunctionV21.GetDataflow));

            _logger.LogDebug("END GetDataForTreeAsync");
            return responseObject;
        }

        public async Task<ISdmxObjects> SendQueryStructureRequestV20Async(IEnumerable<IStructureReference> references,
            bool resolveReferences)
        {
            return await _nsiGetV20.SendQueryStructureRequestV20Async(references, resolveReferences);
        }

        public async Task<ISdmxObjects> GetCodeListCostraintAsync(IDataflowObject dataflow, IDataStructureObject dsd,
            string component, bool useCache = false, bool orderItems = false)
        {
            _logger.LogDebug("START GetCodeListCostraintAsync");

            _logger.LogDebug($"component id {component}");
            var codelistConstraints = _nsiGetGeneric.GetCodeListCostraint(dataflow, dsd, component, useCache);

            var responseObject = new SdmxObjectsImpl();

            foreach (var references in codelistConstraints)
            {
                _logger.LogDebug("SOAPV20 SendQueryStructureRequestV20Async");
                var sdmxObjToMerge = await _nsiGetV20.SendQueryStructureRequestV20Async(references, false);

                if (orderItems)
                    sdmxObjToMerge = SDMXUtils.GetOrderArtefacts(sdmxObjToMerge,
                        _endPointSDMXNodeConfig.AnnotationConfig, _endPointSDMXNodeConfig.UserLang,
                        SdmxStructureEnumType.CodeList);

                responseObject.Merge(sdmxObjToMerge);
            }

            _logger.LogDebug("END GetCodeListCostraintAsync");
            return responseObject;
        }

        public async Task<ISdmxObjects> GetCodeListCostraintFilterAsync(IDataflowObject dataflow,
            IDataStructureObject dsd, string criteriaId, List<FilterCriteria> filterComponents, bool useCache = false,
            bool orderItems = false)
        {
            _logger.LogDebug("START GetCodeListCostraintFilterAsync");

            DataflowDataRange timeRange = null;
            try
            {
                timeRange = await GetFrequences(dataflow, dsd, filterComponents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error to get NSI DataflowDataRange");
            }

            var refs = _nsiGetGeneric.GetCodeListCostraintFilter(dataflow, dsd, criteriaId, filterComponents, timeRange);
            return await _nsiGetV20.SendQueryStructureRequestV20Async(refs, false);

            //var queryStructureRequestBuilderManager = new QueryStructureRequestBuilderManager();

            //IStructureQueryFormat<XDocument> queryFormat = new QueryStructureRequestFormat();
            //var wdoc = queryStructureRequestBuilderManager.BuildStructureQuery(refs, queryFormat, false);

            //var doc = new XmlDocument();
            //doc.LoadXml(wdoc.ToString());

            //var httpRequest = _endPointHttpRequest.CreateRequest(doc, SdmxEndPointCostant.SDMXWSFunction.QueryStructure, true);
            //var response = await _endPointHttpRequest.SendRequestAsync(httpRequest);

            //_logger.LogDebug("END GetCodeListCostraintFilterAsync");
            //return _sdmxParser.GetSdmxObjectsFromNsiResponse(response);
        }

        public async Task<ISdmxObjects> GetCategorySchemesAndCategorisationsAsync(bool useCache = false)
        {
            _logger.LogDebug("START GetCategorySchemesAndCategorisationsAsync");

            ISdmxObjects responseObject;


            var complexStructureQueryCategoryScheme = _nsiGetGeneric.GetCategorySchemesAndCategorisations();
            responseObject = await sendQueryStructureRequestAsync(complexStructureQueryCategoryScheme,
                SDMXWSFunctionV21.GetCategoryScheme);

            _logger.LogDebug("END GetCategorySchemesAndCategorisationsAsync");
            return responseObject;
        }

        public async Task<ISdmxObjects> GetDataflowsAsync(bool useCache = false)
        {
            _logger.LogDebug("START GetDataflowsAsync");

            ISdmxObjects responseObject;

            var complexStructureQueryDataflow = _nsiGetGeneric.GetDataflows();
            responseObject =
                await sendQueryStructureRequestAsync(complexStructureQueryDataflow, SDMXWSFunctionV21.GetDataflow);

            _logger.LogDebug("END GetDataflowsAsync");
            return responseObject;
        }

        #endregion

        #region NsiGetDataSoap

        public async Task<GenericResponseData<IDataReaderEngine>> GetDataflowDataReaderAsync(IDataflowObject df,
            IDataStructureObject kf, List<FilterCriteria> filterCriteria)
        {
            _logger.LogDebug("START GetDataflowDataReaderAsync");

            var sdmxwsFunctionV21 = SDMXWSFunctionV21.GetStructureSpecificData;
            var cross = SDMXUtils.DataflowDsdIsCrossSectional(kf);
            //SOAP 2.0!Utils.IsTimeSeries(kf);
            if (cross) sdmxwsFunctionV21 = SDMXWSFunctionV21.GetCrossSectionalData;

            var countObservation = await GetDataflowObservationCountAsync(df, kf, filterCriteria);

            //int maxObsValue = SDMXUtils.GetMaxObservationFromAnnotations(df, kf, _endPointSDMXNodeConfig.EndPointAppConfig.MaxObs) ?? _endPointSDMXNodeConfig.MaxObservationsAfterCriteria;
            var maxObsValue = _endPointSDMXNodeConfig.MaxObservationsAfterCriteria;

            var sdmxDataWithTimer = new GenericResponseData<IDataReaderEngine>();
            sdmxDataWithTimer.ItemsMax = maxObsValue;
            sdmxDataWithTimer.ItemsCount = countObservation;
            if (countObservation > maxObsValue)
            {
                sdmxDataWithTimer.LimitExceeded = true;
                return sdmxDataWithTimer;
            }

            sdmxDataWithTimer.Timers = new Dictionary<string, string>();

            var request = _nsiGetData.GetDataflowData(df, kf, filterCriteria);
            var sdmxRequest = _endPointHttpRequest.CreateRequest(request, sdmxwsFunctionV21, true);

            using (var sdmxResponse = await _endPointHttpRequest.SendRequestAsync(sdmxRequest, true))
            {
                using (var dataLocation = new FileReadableDataLocation(sdmxResponse.FileResponse))
                {
                    IDataReaderEngine dataReader;
                    switch (sdmxwsFunctionV21)
                    {
                        case SDMXWSFunctionV21.GetCompactData:
                        case SDMXWSFunctionV21.GetStructureSpecificData:
                            dataReader = new CompactDataReaderEngine(dataLocation, df, kf);
                            //var readerCompact = new SdmxDataReader(kf, store);
                            //readerCompact.ReadData(compact);
                            break;

                        case SDMXWSFunctionV21.GetCrossSectionalData:
                            var dsdCrossSectional = (ICrossSectionalDataStructureObject) kf;
                            dataReader = new CrossSectionalDataReaderEngine(dataLocation, dsdCrossSectional, df);
                            //var reader = new SdmxDataReader(kf, store);
                            //reader.ReadData(crossSectional);
                            break;

                        default:
                            throw new ArgumentException($"Data parser [{sdmxwsFunctionV21}] not found ");
                    }

                    sdmxDataWithTimer.Timers.Add("nsiResponse", $"{sdmxResponse.ResponseTime}ms");
                    sdmxDataWithTimer.Timers.Add("nsiResponseDownload", $"{sdmxResponse.DownloadResponseTime}ms");
                    sdmxDataWithTimer.Timers.Add("nsiResponseDownloadSize",
                        $"{sdmxResponse.DownloadResponseSize}Kb");

                    sdmxDataWithTimer.Data = dataReader;
                }

                _logger.LogDebug("END GetDataflowDataReaderAsync");
            }

            return sdmxDataWithTimer;
        }

        public async Task<GenericResponseData<XmlDataContainer>> GetDataflowXmlDataAsync(IDataflowObject df,
            IDataStructureObject kf, List<FilterCriteria> filterCriteria, bool includeCodelists = true)
        {
            _logger.LogDebug("START GetDataflowXmlDataAsync");

            var sdmxwsFunctionV21 = SDMXWSFunctionV21.GetStructureSpecificData;
            var cross = SDMXUtils.DataflowDsdIsCrossSectional(kf);
            //SOAP 2.0!Utils.IsTimeSeries(kf);
            if (cross) sdmxwsFunctionV21 = SDMXWSFunctionV21.GetCrossSectionalData;

            var countObservation = await GetDataflowObservationCountAsync(df, kf, filterCriteria);


            //int maxObsValue = SDMXUtils.GetMaxObservationFromAnnotations(df, kf, _endPointSDMXNodeConfig.EndPointAppConfig.MaxObs) ?? _endPointSDMXNodeConfig.MaxObservationsAfterCriteria;
            var maxObsValue = _endPointSDMXNodeConfig.MaxObservationsAfterCriteria;

            var sdmxDataWithTimer = new GenericResponseData<XmlDataContainer>();
            sdmxDataWithTimer.Data = new XmlDataContainer();
            sdmxDataWithTimer.ItemsMax = maxObsValue;
            sdmxDataWithTimer.ItemsCount = countObservation;
            if (countObservation > maxObsValue)
            {
                sdmxDataWithTimer.LimitExceeded = true;
                return sdmxDataWithTimer;
            }

            sdmxDataWithTimer.Timers = new Dictionary<string, string>();

            var request = _nsiGetData.GetDataflowData(df, kf, filterCriteria);
            var sdmxRequest = _endPointHttpRequest.CreateRequest(request, sdmxwsFunctionV21, true);


            using (var sdmxResponse = await _endPointHttpRequest.SendRequestAsync(sdmxRequest))
            {
                sdmxDataWithTimer.Data.XmlDocument = sdmxResponse.XmlResponse;
                sdmxDataWithTimer.Timers.Add("nsiResponse", $"{sdmxResponse.ResponseTime}ms");
                sdmxDataWithTimer.Timers.Add("nsiResponseDownload", $"{sdmxResponse.DownloadResponseTime}ms");
                sdmxDataWithTimer.Timers.Add("nsiResponseDownloadSize", $"{sdmxResponse.DownloadResponseSize}Kb");
            }

            _logger.LogDebug("END GetDataflowXmlDataAsync");
            return sdmxDataWithTimer;
        }

        public Task<GenericResponseData<string>> GetDataflowJsonSdmxDataAsync(IDataflowObject df,
            IDataStructureObject kf, List<FilterCriteria> filterCriteria)
        {
            throw new NotImplementedException();
        }

        public async Task<long> GetDataflowObservationCountAsync(IDataflowObject df, IDataStructureObject kf,
            List<FilterCriteria> filterCriteria)
        {
            _logger.LogDebug("START GetDataflowObservationCountAsync");


            var request = _nsiGetData.GetCountObservation(df, kf, filterCriteria);
            var sdmxObject = await _nsiGetV20.SendQueryStructureRequestV20Async(request, false);

            var countId = sdmxObject?.Codelists?.FirstOrDefault()?.Items?.FirstOrDefault()?.Id;
            if (countId == null) return -1;

            var convertSuccess = long.TryParse(countId, out var resultValue);

            return convertSuccess ? resultValue : -1L;
        }

        public async Task<DataflowDataRange> GetFrequences(IDataflowObject dataflow, IDataStructureObject dsd, List<FilterCriteria> filterCriteria,
           bool useCache = false, bool orderItems = false)
        {
            _logger.LogDebug("START GetCodeListCostraintFilterAsync");

            var timeDimensionid = "";
            foreach (var component in dsd.Components)
            {
                var dimension = component as IDimension;
                if (dimension == null) continue;

                if (dimension.TimeDimension)
                {
                    timeDimensionid = dimension.Id;
                    break;
                }
            }
            if (string.IsNullOrWhiteSpace(timeDimensionid))
            {
                return null;
            }

            var haveRange = filterCriteria.Any(i => i.Type == FilterType.TimePeriod);
            if (!haveRange)
            {
                _logger.LogDebug("no filter with range in filter");
                return null;
            }

            var request = _nsiGetGeneric.GetFrequences(dataflow, dsd, filterCriteria);

            if (request == null)
            {
                return null;
            }

            ISdmxObjects responseObject = await _nsiGetV20.SendQueryStructureRequestV20Async(request, false);

            if (orderItems)
                responseObject = SDMXUtils.GetOrderArtefacts(responseObject, _endPointSDMXNodeConfig.AnnotationConfig,
                    _endPointSDMXNodeConfig.UserLang, SdmxStructureEnumType.CodeList);

            ICodelistObject codelist = null;
            foreach (var component in dsd.Components)
            {
                var dimension = component as IDimension;
                if (dimension == null) continue;

                if (dimension.TimeDimension)
                {
                    continue;
                }
                if (dimension.FrequencyDimension)
                {
                    var agency = dimension?.Representation?.Representation?.AgencyId;
                    var version = dimension?.Representation?.Representation?.Version;
                    var mainId = dimension?.Representation?.Representation?.MaintainableId;
                    if (agency != null &&
                        version != null &&
                         mainId != null)
                    {
                        codelist = responseObject.Codelists.FirstOrDefault(i => i.Id == mainId && i.Version == version && i.AgencyId == agency);
                    }
                }
            }


            var dataflowDataRange = new DataflowDataRange
            {
                EndRange = responseObject.ContentConstraintObjects.FirstOrDefault().ReferencePeriod.EndTime.Date,
                PeriodType = getMaxDetailtPeriod(codelist),
                RangePeriod = filterCriteria.FirstOrDefault(i => i.Type == FilterType.TimePeriod)?.Period ?? 1
            };

            _logger.LogDebug("END GetCodeListCostraintFilterAsync");
            return dataflowDataRange;
        }

        private string getMaxDetailtPeriod(ICodelistObject codelist)
        {
            if (codelist == null)
            {
                return "A";
            }

            if (codelist.Items.Any(i => i.Id.Equals("D", StringComparison.InvariantCultureIgnoreCase)))
            {
                return "D";
            }
            else if (codelist.Items.Any(i => i.Id.Equals("M", StringComparison.InvariantCultureIgnoreCase)))
            {
                return "M";
            }
            else if (codelist.Items.Any(i => i.Id.Equals("Q", StringComparison.InvariantCultureIgnoreCase)))
            {
                return "Q";
            }
            else if (codelist.Items.Any(i => i.Id.Equals("S", StringComparison.InvariantCultureIgnoreCase)))
            {
                return "S";
            }
            else if (codelist.Items.Any(i => i.Id.Equals("A", StringComparison.InvariantCultureIgnoreCase)))
            {
                return "A";
            }

            return "A";
        }

        public Task<ISdmxObjects> GetDataflowWithUsedData(IDataflowObject df)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}