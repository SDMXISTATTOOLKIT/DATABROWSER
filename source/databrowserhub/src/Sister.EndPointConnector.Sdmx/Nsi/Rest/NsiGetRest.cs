using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Cache.Key;
using DataBrowser.Interfaces.EndPoint;
using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.Interfaces.Sdmx.Nsi;
using EndPointConnector.Interfaces.Sdmx.Nsi.Get;
using EndPointConnector.Models;
using EndPointConnector.ParserSdmx;
using Microsoft.Extensions.Logging;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Engine;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.DataParser.Engine;
using Org.Sdmxsource.Sdmx.DataParser.Engine.Reader;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;
using Org.Sdmxsource.Util.Io;
using Sister.EndPointConnector.Sdmx.Nsi.Rest.Get;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Sister.EndPointConnector.Sdmx.Nsi.Rest
{
    public class NsiGetRest : INsiGet
    {
        private readonly INsiEndPointHttpRequest _endPointHttpRequest;
        private readonly EndPointSdmxConfig _endPointSDMXNodeConfig;
        private readonly ILogger<NsiGetRest> _logger;
        private readonly NsiGetArtefactRest _nsiGetArtefact;
        private readonly NsiGetDataRest _nsiGetData;
        private readonly NsiGetStructureRest _nsiGetGeneric;
        private readonly INsiGet _nsiGetSoap;
        private readonly SdmxParser _sdmxParser;

        public NsiGetRest(INsiEndPointHttpRequest endPointHttpRequest,
            EndPointSdmxConfig endPointSDMXNodeConfig,
            ILoggerFactory loggerFactory,
            INsiGet nsiGetSoap)
        {
            _logger = loggerFactory.CreateLogger<NsiGetRest>();
            _sdmxParser = new SdmxParser(loggerFactory);
            _endPointHttpRequest = endPointHttpRequest;
            _endPointSDMXNodeConfig = endPointSDMXNodeConfig;
            _nsiGetArtefact = new NsiGetArtefactRest(loggerFactory, _endPointSDMXNodeConfig);
            _nsiGetGeneric = new NsiGetStructureRest(loggerFactory);
            _nsiGetData = new NsiGetDataRest(loggerFactory, _endPointSDMXNodeConfig);
            _nsiGetSoap = nsiGetSoap;
        }

        #region NsiGetArtefactRest

        public async Task<ISdmxObjects> GetArtefactAsync(SdmxStructureEnumType type, string id, string agency,
            string version, StructureReferenceDetailEnumType refDetail = StructureReferenceDetailEnumType.None,
            string respDetail = "", bool useCache = false, bool includeCrossReference = true, bool orderItems = false)
        {
            _logger.LogDebug("START GetArtefactAsync");

            var parameters = _nsiGetArtefact.GetArtefact(type, id, agency, version, refDetail, respDetail);

            var request = _endPointHttpRequest.CreateRequest(parameters, SdmxEndPointCostant.RequestType.Structure,
                HttpMethod.Get, includeCrossReference, _endPointSDMXNodeConfig.XmlResultNeedFix);
            var response = await _endPointHttpRequest.SendRequestAsync(request);

            if (response.NoResultFound)
            {
                return new SdmxObjectsImpl();
            }

            _logger.LogDebug("GetSdmxObjectsFromNsiSdmxXml");
            var result = _sdmxParser.GetSdmxObjectsFromNsiResponse(response,
                            includeCrossReference: includeCrossReference, schemaType: SdmxSchemaEnumType.VersionTwoPointOne);

            if (orderItems)
            {
                result = SDMXUtils.GetOrderArtefacts(result, _endPointSDMXNodeConfig?.AnnotationConfig,
                    _endPointSDMXNodeConfig.UserLang);
            }

            _logger.LogDebug("END GetArtefactAsync");
            return result;
        }

        #endregion

        private async Task<ISdmxObjects> sendQueryStructureRequestAsync(string request)
        {
            var httpRequest = _endPointHttpRequest.CreateRequest(request, SdmxEndPointCostant.RequestType.Structure,
                HttpMethod.Get);
            var response = await _endPointHttpRequest.SendRequestAsync(httpRequest);
            if (response.NoResultFound)
            {
                return new SdmxObjectsImpl();
            }

            return _sdmxParser.GetSdmxObjectsFromNsiResponse(response);
        }

        #region NsiGetStructureRest

        public async Task<ISdmxObjects> GetDataForTreeAsync(bool useCache = false)
        {
            _logger.LogDebug("START GetDataForTreeAsync");

            ISdmxObjects responseObject;

            var request = _nsiGetGeneric.GetCategorySchemesAndCategorisations();
            responseObject = await sendQueryStructureRequestAsync(request);

            if (_endPointSDMXNodeConfig.CategorySchemaExcludes != null)
            {
                foreach (var itemCatSchema in _endPointSDMXNodeConfig.CategorySchemaExcludes)
                {
                    var itmSplit = itemCatSchema.Split('+');
                    var itemExclude = responseObject.CategorySchemes.FirstOrDefault(i =>
                        i.AgencyId.Equals(itmSplit[0], StringComparison.InvariantCultureIgnoreCase) &&
                        i.Id.Equals(itmSplit[1], StringComparison.InvariantCultureIgnoreCase) &&
                        i.Version.Equals(itmSplit[2], StringComparison.InvariantCultureIgnoreCase));
                    if (itemExclude != null)
                    {
                        responseObject.RemoveCategoryScheme(itemExclude);
                    }
                }
            }

            request = _nsiGetGeneric.GetDataflows(false);
            responseObject.Merge(await sendQueryStructureRequestAsync(request));

            _logger.LogDebug("END GetDataForTreeAsync");
            return responseObject;
        }

        public Task<ISdmxObjects> SendQueryStructureRequestV20Async(IEnumerable<IStructureReference> references,
            bool resolveReferences)
        {
            throw new NotImplementedException();
        }

        public async Task<ISdmxObjects> GetCodeListCostraintAsync(IDataflowObject dataflow, IDataStructureObject dsd,
            string component, bool useCache = false, bool orderItems = false)
        {
            _logger.LogDebug("START GetCodeListCostraintAsync");

            _logger.LogDebug($"component id {component}");

            var responseObject = new SdmxObjectsImpl();

            var requestsCodelist = _nsiGetGeneric.GetCodeListCostraint(dataflow, dsd, component);

            foreach (var groupType in requestsCodelist)
            {
                foreach (var request in groupType.Value)
                {
                    var httpRequest = _endPointHttpRequest.CreateRequest(request,
                        SdmxEndPointCostant.RequestType.Structure, HttpMethod.Get);
                    var response = await _endPointHttpRequest.SendRequestAsync(httpRequest);
                    var sdmxObjToMerge = _sdmxParser.GetSdmxObjectsFromNsiResponse(response);

                    if (groupType.Key == ComponentType.EnumComponentType.TimeDimension)
                    {
                        //Convert result in codelist
                        ICodelistMutableObject mutable = new CodelistMutableCore
                        {
                            Id = CustomCodelistConstants.TimePeriodCodeList,
                            AgencyId = CustomCodelistConstants.Agency,
                            Version = CustomCodelistConstants.Version
                        };
                        mutable.AddName("en", "Time Dimension Start and End periods");

                        var content = sdmxObjToMerge.ContentConstraintObjects.FirstOrDefault();
                        if (content?.ReferencePeriod?.StartTime != null)
                        {
                            var mutableCode = new CodeMutableCore
                            { Id = content.ReferencePeriod.StartTime.Date.ToString("yyyy-MM-dd") };
                            mutableCode.AddName("en", "Start Time period");
                            mutable.AddItem(mutableCode);
                        }

                        if (content?.ReferencePeriod?.EndTime != null)
                        {
                            var mutableCode = new CodeMutableCore
                            { Id = content.ReferencePeriod.EndTime.Date.ToString("yyyy-MM-dd") };
                            mutableCode.AddName("en", "End Time period");
                            mutable.AddItem(mutableCode);
                        }

                        responseObject.Merge(new SdmxObjectsImpl(mutable.ImmutableInstance));
                    }
                    else
                    {
                        if (orderItems)
                        {
                            sdmxObjToMerge = SDMXUtils.GetOrderArtefacts(sdmxObjToMerge,
                                _endPointSDMXNodeConfig.AnnotationConfig, _endPointSDMXNodeConfig.UserLang);
                        }

                        responseObject.Merge(sdmxObjToMerge);
                    }
                }
            }

            _logger.LogDebug("END GetCodeListCostraintAsync");

            return responseObject;
        }

        public async Task<ISdmxObjects> GetCodeListCostraintFilterAsync(IDataflowObject dataflow,
            IDataStructureObject dsd, string criteriaId, List<FilterCriteria> filterComponents, bool useCache = false,
            bool orderItems = false)
        {
            _logger.LogDebug("START GetCodeListCostraintFilterAsync");

            try
            {
                //TODO remove after Eurostat 
                if (!string.IsNullOrWhiteSpace(_endPointSDMXNodeConfig.EndPointV20))
                { 
                    return await _nsiGetSoap.GetCodeListCostraintFilterAsync(dataflow, dsd, criteriaId, filterComponents, useCache, orderItems);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SOAP GetCodeListCostraintFilterAsync, try use REST: {ex.Message}");
            }

            DataflowDataRange timeRange = null;
            try
            {
                timeRange = await GetFrequences(dataflow, dsd, filterComponents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error to get NSI DataflowDataRange");
            }

            var request = _nsiGetGeneric.GetCodeListCostraintFilter(dataflow, dsd, criteriaId, filterComponents, timeRange);

            ISdmxObjects responseObject = null;
            var httpRequest = _endPointHttpRequest.CreateRequest(request, SdmxEndPointCostant.RequestType.Structure,
                HttpMethod.Get);

            using (var response = await _endPointHttpRequest.SendRequestAsync(httpRequest))
            {
                if (response.NoResultFound)
                {
                    _logger.LogDebug("No codelist or result found");
                    return new SdmxObjectsImpl();
                }

                if (response.NsiErrorResponse)
                {
                    throw new Exception(response.TextResponse);
                }

                _logger.LogDebug("No codelist or result found");

                responseObject = _sdmxParser.GetSdmxObjectsFromNsiResponse(response);
            }

            if (criteriaId != null && criteriaId.Equals("TIME_PERIOD"))
            {

                DateTime startTime = default;

                ICodelistMutableObject codelistMutable = new CodelistMutableCore
                {
                    Id = CustomCodelistConstants.TimePeriodCodeList,
                    AgencyId = CustomCodelistConstants.Agency,
                    Version = CustomCodelistConstants.Version
                };
                codelistMutable.AddName("en", "Time Dimension Start and End periods");

                if (responseObject?.ContentConstraintObjects?.FirstOrDefault()?.ReferencePeriod?.StartTime?.Date !=
                    null &&
                    responseObject.ContentConstraintObjects.FirstOrDefault().ReferencePeriod.StartTime.Date != default)
                {
                    startTime = responseObject.ContentConstraintObjects.FirstOrDefault().ReferencePeriod.StartTime.Date;
                    ICodeMutableObject itemMutable = new CodeMutableCore
                    {
                        Id = startTime.ToString("yyyy-MM-dd")
                    };
                    itemMutable.AddName("en", "Start Time period");
                    codelistMutable.AddItem(itemMutable);
                }

                if (responseObject?.ContentConstraintObjects?.FirstOrDefault()?.ReferencePeriod?.EndTime?.Date !=
                    null &&
                    responseObject.ContentConstraintObjects.FirstOrDefault().ReferencePeriod.EndTime.Date != default)
                {
                    var endTime = responseObject.ContentConstraintObjects.FirstOrDefault().ReferencePeriod.EndTime.Date;
                    if (endTime.CompareTo(startTime) == 0)
                    {
                        var newEndTime = new DateTime(startTime.Year, 12, 31);
                        endTime = newEndTime.CompareTo(startTime) == 0 ? newEndTime.AddDays(1) : newEndTime;
                    }
                    ICodeMutableObject itemMutable = new CodeMutableCore
                    {
                        Id = endTime.ToString("yyyy-MM-dd")
                    };
                    itemMutable.AddName("en", "End Time period");
                    codelistMutable.AddItem(itemMutable);
                }

                responseObject.AddCodelist(codelistMutable.ImmutableInstance);
            }

            if (orderItems)
            {
                responseObject = SDMXUtils.GetOrderArtefacts(responseObject, _endPointSDMXNodeConfig.AnnotationConfig,
                    _endPointSDMXNodeConfig.UserLang, SdmxStructureEnumType.CodeList);
            }

            _logger.LogDebug("END GetCodeListCostraintFilterAsync");
            return responseObject;
        }

        public async Task<DataflowDataRange> GetFrequences(IDataflowObject dataflow, IDataStructureObject dsd, List<FilterCriteria> filterCriteria,
            bool useCache = false, bool orderItems = false)
        {
            _logger.LogDebug("START GetCodeListCostraintFilterAsync");

            var timeDimensionid = "";
            foreach (var component in dsd.Components)
            {
                var dimension = component as IDimension;
                if (dimension == null)
                {
                    continue;
                }

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

            ISdmxObjects responseObject = null;
            var httpRequest = _endPointHttpRequest.CreateRequest(request, SdmxEndPointCostant.RequestType.Structure,
                HttpMethod.Get);

            using (var response = await _endPointHttpRequest.SendRequestAsync(httpRequest))
            {
                if (response.NoResultFound)
                {
                    _logger.LogDebug("No codelist or result found");
                    return null;
                }

                if (response.NsiErrorResponse)
                {
                    throw new Exception(response.TextResponse);
                }

                _logger.LogDebug("No codelist or result found");

                responseObject = _sdmxParser.GetSdmxObjectsFromNsiResponse(response);
            }

            if (orderItems)
            {
                responseObject = SDMXUtils.GetOrderArtefacts(responseObject, _endPointSDMXNodeConfig.AnnotationConfig,
                    _endPointSDMXNodeConfig.UserLang, SdmxStructureEnumType.CodeList);
            }

            ICodelistObject codelist = null;
            foreach (var component in dsd.Components)
            {
                var dimension = component as IDimension;
                if (dimension == null)
                {
                    continue;
                }

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

        public async Task<ISdmxObjects> GetCategorySchemesAndCategorisationsAsync(bool useCache = false)
        {
            _logger.LogDebug("START GetCategorySchemesAndCategorisationsAsync");

            var request = _nsiGetGeneric.GetCategorySchemesAndCategorisations();

            var responseObject = await sendQueryStructureRequestAsync(request);

            _logger.LogDebug("END GetCategorySchemesAndCategorisationsAsync");
            return responseObject;
        }

        public async Task<ISdmxObjects> GetDataflowsAsync(bool useCache = false)
        {
            _logger.LogDebug("START GetDataflowsAsync");

            ISdmxObjects responseObject;

            var request = _nsiGetGeneric.GetDataflows(false);
            responseObject = await sendQueryStructureRequestAsync(request);

            _logger.LogDebug("END GetDataflowsAsync");
            return responseObject;
        }

        public async Task<ISdmxObjects> GetOnlyDataflowsValidForCatalogWithDsdAndCodelistAsync(bool useCache = false)
        {
            _logger.LogDebug("START GetDataflowsAsync");

            ISdmxObjects responseObject;

            var request = _nsiGetGeneric.GetDataflows(true);
            var dataflowContainer = await sendQueryStructureRequestAsync(request);
            var objDf = dataflowContainer?.Dataflows?.Where(i =>
                _sdmxParser.DataflowIsValidForCatalog(_endPointSDMXNodeConfig, i));
            var codelistObs = await GetArtefactAsync(SdmxStructureEnumType.CodeList, null, null, null, StructureReferenceDetailEnumType.None);

            responseObject = new SdmxObjectsImpl();
            if (objDf != null)
            {
                foreach (var i in objDf)
                {
                    responseObject.AddDataflow(i);
                }

                foreach (var i in dataflowContainer.DataStructures)
                {
                    responseObject.AddDataStructure(i);
                }

                foreach (var i in codelistObs.Codelists)
                {
                    responseObject.AddCodelist(i);
                }
            }

            _logger.LogDebug("END GetDataflowsAsync");
            return responseObject;
        }

        #endregion

        #region NsiGetDataRest

        public async Task<GenericResponseData<IDataReaderEngine>> GetDataflowDataReaderAsync(IDataflowObject df,
            IDataStructureObject kf, List<FilterCriteria> filterCriteria)
        {
            _logger.LogDebug("START GetDataflowDataReaderAsync");

            if (_nsiGetSoap != null)
            {
                return await _nsiGetSoap.GetDataflowDataReaderAsync(df, kf, filterCriteria);
            }

            var sdmxwsFunctionV21 = SdmxEndPointCostant.SDMXWSFunctionV21.GetCompactData;
            var cross = SDMXUtils.DataflowDsdIsCrossSectional(kf);
            //SOAP 2.0!Utils.IsTimeSeries(kf);
            if (cross)
            {
                sdmxwsFunctionV21 = SdmxEndPointCostant.SDMXWSFunctionV21.GetCrossSectionalData;
            }

            DataflowDataRange timeRange = null;
            try
            {
                timeRange = await GetFrequences(df, kf, filterCriteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error to get NSI DataflowDataRange");
            }

            var request = _nsiGetData.GetDataflowData(df, kf, filterCriteria, timeRange);

            int? maxObsValue = null;
            //var maxObsValue = SDMXUtils.GetMaxObservationFromAnnotations(df, kf, _endPointSDMXNodeConfig.EndPointAppConfig.MaxObs) ?? _endPointSDMXNodeConfig.MaxObservationsAfterCriteria;

            var sdmxRequest = _endPointHttpRequest.CreateRequest(request.QueryString,
                SdmxEndPointCostant.RequestType.DataStructureSpecificV21Xml, request.HttpMethod,
                maxObservations: maxObsValue, keys: request.Keys, contentData: null, contentType: request.ContentType);

            IDataReaderEngine dataReaderEngine = null;
            var sdmxDataWithTimer = new GenericResponseData<IDataReaderEngine>
            {
                Data = dataReaderEngine
            };

            using (var sdmxResponse = await _endPointHttpRequest.SendRequestAsync(sdmxRequest, true))
            {
                if (sdmxResponse.NoResultFound)
                {
                    new GenericResponseData<string> { ItemsCount = 0 };
                }

                if (sdmxResponse?.ResponseContentRange?.Unit != null &&
                    sdmxResponse.ResponseContentRange.Unit.Equals("values", StringComparison.InvariantCultureIgnoreCase)
                )
                {
                    sdmxDataWithTimer.ItemsCount = sdmxResponse.ResponseContentRange.HasLength
                        ? sdmxResponse.ResponseContentRange.Length.Value
                        : -1;
                    sdmxDataWithTimer.ItemsFrom = sdmxResponse.ResponseContentRange.From.HasValue
                        ? sdmxResponse.ResponseContentRange.From.Value
                        : -1;
                    sdmxDataWithTimer.ItemsTo = sdmxResponse.ResponseContentRange.To.HasValue
                        ? sdmxResponse.ResponseContentRange.To.Value
                        : -1;
                }

                using (var dataLocation = new FileReadableDataLocation(sdmxResponse.FileResponse))
                {
                    switch (sdmxwsFunctionV21)
                    {
                        case SdmxEndPointCostant.SDMXWSFunctionV21.GetCompactData:
                            dataReaderEngine = new CompactDataReaderEngine(dataLocation, df, kf);
                            break;

                        case SdmxEndPointCostant.SDMXWSFunctionV21.GetCrossSectionalData:
                            var dsdCrossSectional = (ICrossSectionalDataStructureObject)kf;
                            dataReaderEngine = new CrossSectionalDataReaderEngine(dataLocation, dsdCrossSectional, df);
                            break;

                        default:
                            throw new ArgumentException($"Data parser [{sdmxwsFunctionV21}] not found ");
                    }
                }
            }

            _logger.LogDebug("END GetDataflowDataReaderAsync");


            return sdmxDataWithTimer;
        }

        public async Task<GenericResponseData<XmlDataContainer>> GetDataflowXmlDataAsync(IDataflowObject df,
            IDataStructureObject kf, List<FilterCriteria> filterCriteria, bool includeCodelists = false)
        {
            _logger.LogDebug("START GetDataflowXmlDataAsync");


            DataflowDataRange timeRange = null;
            try
            {
                timeRange = await GetFrequences(df, kf, filterCriteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error to get NSI DataflowDataRange");
            }

            var request = _nsiGetData.GetDataflowData(df, kf, filterCriteria, timeRange);

            //var maxObsValue = SDMXUtils.GetMaxObservationFromAnnotations(df, kf, _endPointSDMXNodeConfig.EndPointAppConfig.MaxObs) ?? _endPointSDMXNodeConfig.MaxObservationsAfterCriteria;

            var sdmxRequest = _endPointHttpRequest.CreateRequest(request.QueryString,
                SdmxEndPointCostant.RequestType.DataStructureSpecificV21Xml, request.HttpMethod, keys: request.Keys,
                contentData: null, contentType: request.ContentType);

            using (var sdmxResponse = await _endPointHttpRequest.SendRequestAsync(sdmxRequest))
            {
                _logger.LogDebug("Request for data dataflow complted");

                if (sdmxResponse.NoResultFound)
                {
                    new GenericResponseData<string> { ItemsCount = 0 };
                }

                var sdmxDataWithTimer = new GenericResponseData<XmlDataContainer>
                {
                    Data = new XmlDataContainer
                    {
                        DataStructures = null,
                        XmlDocument = sdmxResponse.XmlResponse,
                        Codelists = null,
                        ConceptSchemes = null
                    }
                };

                if (sdmxResponse?.ResponseContentRange?.Unit != null &&
                    sdmxResponse.ResponseContentRange.Unit.Equals("values", StringComparison.InvariantCultureIgnoreCase)
                )
                {
                    sdmxDataWithTimer.ItemsCount = sdmxResponse.ResponseContentRange.HasLength
                        ? sdmxResponse.ResponseContentRange.Length.Value
                        : -1;
                    sdmxDataWithTimer.ItemsFrom = sdmxResponse.ResponseContentRange.From.HasValue
                        ? sdmxResponse.ResponseContentRange.From.Value
                        : -1;
                    sdmxDataWithTimer.ItemsTo = sdmxResponse.ResponseContentRange.To.HasValue
                        ? sdmxResponse.ResponseContentRange.To.Value
                        : -1;
                }

                sdmxDataWithTimer.Timers = new Dictionary<string, string>
                {
                    { "nsiResponse", $"{sdmxResponse.ResponseTime}ms" },
                    { "nsiResponseDownload", $"{sdmxResponse.DownloadResponseTime}ms" },
                    { "nsiResponseDownloadSize", $"{sdmxResponse.DownloadResponseSize}Kb" }
                };

                return sdmxDataWithTimer;
            }
        }

        public async Task<GenericResponseData<string>> GetDataflowJsonSdmxDataAsync(IDataflowObject df,
            IDataStructureObject kf, List<FilterCriteria> filterCriteria)
        {
            _logger.LogDebug("START GetDataflowJsonSdmxDataAsync");

            DataflowDataRange timeRange = null;
            try
            {
                timeRange = await GetFrequences(df, kf, filterCriteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error to get NSI DataflowDataRange");
            }

            var request = _nsiGetData.GetDataflowData(df, kf, filterCriteria, timeRange);

            //var maxObsValue = SDMXUtils.GetMaxObservationFromAnnotations(df, kf, _endPointSDMXNodeConfig.EndPointAppConfig.MaxObs) ?? _endPointSDMXNodeConfig.MaxObservationsAfterCriteria;

            var sdmxRequest = _endPointHttpRequest.CreateRequest(request.QueryString,
                SdmxEndPointCostant.RequestType.DataSdmxJson, request.HttpMethod, keys: request.Keys, contentData: null,
                contentType: request.ContentType);

            using (var sdmxResponse = await _endPointHttpRequest.SendRequestAsync(sdmxRequest, true))
            {
                _logger.LogDebug("Request for data dataflow complted");

                if (sdmxResponse.NoResultFound)
                {
                    return new GenericResponseData<string>
                    {
                        ItemsCount = 0,
                        Timers = new Dictionary<string, string> { { "nsiResponse", $"{sdmxResponse.ResponseTime}ms" } }
                    };
                }

                var response = new GenericResponseData<string>
                {
                    Timers = new Dictionary<string, string>(),
                    Data = !string.IsNullOrWhiteSpace(sdmxResponse.FileResponse)
                    ? await File.ReadAllTextAsync(sdmxResponse.FileResponse)
                    : sdmxResponse.TextResponse
                };

                if (sdmxResponse?.ResponseContentRange?.Unit != null &&
                    sdmxResponse.ResponseContentRange.Unit.Equals("values", StringComparison.InvariantCultureIgnoreCase)
                )
                {
                    response.ItemsCount = sdmxResponse.ResponseContentRange.HasLength
                        ? sdmxResponse.ResponseContentRange.Length.Value
                        : -1;
                    response.ItemsFrom = sdmxResponse.ResponseContentRange.From.HasValue
                        ? sdmxResponse.ResponseContentRange.From.Value
                        : -1;
                    response.ItemsTo = sdmxResponse.ResponseContentRange.To.HasValue
                        ? sdmxResponse.ResponseContentRange.To.Value
                        : -1;
                }

                response.Timers.Add("nsiResponse", $"{sdmxResponse.ResponseTime}ms");
                response.Timers.Add("nsiResponseDownload", $"{sdmxResponse.DownloadResponseTime}ms");
                response.Timers.Add("nsiResponseDownloadSize", $"{sdmxResponse.DownloadResponseSize}Kb");
                //response.Timers.Add("NSI Response Processed", $"{sdmxResponse.ObjectCompletedTime}ms");
                //response.Timers.Add("NSI Response Completed", $"{sdmxResponse.ResponseTime + sdmxResponse.DownloadResponseTime}ms");

                _logger.LogDebug($"END {MethodBase.GetCurrentMethod().Name}");

                return response;
            }
        }

        public async Task<long> GetDataflowObservationCountAsync(IDataflowObject df, IDataStructureObject kf,
            List<FilterCriteria> filterCriteria)
        {
            _logger.LogDebug("START GetDataflowObservationCountAsync");

            if (_nsiGetSoap != null)
            {
                return await _nsiGetSoap.GetDataflowObservationCountAsync(df, kf, filterCriteria);
            }

            DataflowDataRange timeRange = null;
            try
            {
                timeRange = await GetFrequences(df, kf, filterCriteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error to get NSI DataflowDataRange");
            }

            var request = _nsiGetData.GetDataflowData(df, kf, filterCriteria, timeRange);
            var sdmxRequest = _endPointHttpRequest.CreateRequest(request.QueryString,
                SdmxEndPointCostant.RequestType.DataCountString, HttpMethod.Get, keys: request.Keys, contentData: null,
                contentType: request.ContentType);

            using (var sdmxResponse = await _endPointHttpRequest.SendRequestAsync(sdmxRequest))
            {
                _logger.LogDebug("Request for data dataflow complted");
                if (sdmxResponse.NoResultFound)
                {
                    return 0;
                }

                if (sdmxResponse?.ResponseContentRange?.Unit != null &&
                    sdmxResponse.ResponseContentRange.Unit.Equals("values", StringComparison.InvariantCultureIgnoreCase)
                )
                {
                    var count = sdmxResponse.ResponseContentRange.HasLength
                        ? sdmxResponse.ResponseContentRange.Length.Value
                        : -1;

                    _logger.LogDebug($"Count value: {count}");

                    _logger.LogDebug("END GetDataflowObservationCountAsync");

                    return count;
                }
            }

            return -1;
        }

        public async Task<GenericResponseData<string>> DownloadDataflowsAsync(IDataflowObject df,
            IDataStructureObject kf, List<FilterCriteria> filterCriteria, string downloadFormat,
            int? maxObservations = null)
        {
            _logger.LogDebug($"START{MethodBase.GetCurrentMethod().Name}");

            var requestType = SdmxEndPointCostant.RequestType.DataSdmxJson;
            switch (downloadFormat.ToUpperInvariant())
            {
                case "GENERICDATA":
                    requestType = SdmxEndPointCostant.RequestType.DataGenericV20Xml;
                    break;
                case "GENERICDATA20":
                    requestType = SdmxEndPointCostant.RequestType.DataGenericV21Xml;
                    break;
                case "JSONDATA":
                    requestType = SdmxEndPointCostant.RequestType.DataSdmxJson;
                    break;
                case "JSONSTRUCTURE":
                    requestType = SdmxEndPointCostant.RequestType.StructureJson;
                    break;
                case "STRUCTURE":
                    requestType = SdmxEndPointCostant.RequestType.StructureXml;
                    break;
                case "STRUCTURESPECIFICDATA":
                    requestType = SdmxEndPointCostant.RequestType.DataStructureSpecificV21Xml;
                    break;
                case "CSV":
                    requestType = SdmxEndPointCostant.RequestType.DataCsvSdmxString;
                    break;
                case "COMPACTDATA":
                    requestType = SdmxEndPointCostant.RequestType.DataCompactXml;
                    break;
                case "EDIDATA":
                    requestType = SdmxEndPointCostant.RequestType.DataEdiXml;
                    break;
            }

            DataflowDataRange timeRange = null;
            try
            {
                timeRange = await GetFrequences(df, kf, filterCriteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error to get NSI DataflowDataRange");
            }

            var request = _nsiGetData.GetDataflowData(df, kf, filterCriteria, timeRange);

            var sdmxRequest = _endPointHttpRequest.CreateRequest(request.QueryString, requestType, request.HttpMethod,
                maxObservations: maxObservations, contentType: request.ContentType, keys: request.Keys);

            var sdmxResponse = await _endPointHttpRequest.SendRequestAsync(sdmxRequest, true);

            if (sdmxResponse.NoResultFound)
            {
                return new GenericResponseData<string>
                {
                    NotFoundResource = true,
                    Errors = new List<string> { "Dataflow not found" },
                    ItemsCount = 0,
                    Timers = new Dictionary<string, string> { { "nsiResponse", $"{sdmxResponse.ResponseTime}ms" } }
                };
            }

            if (sdmxResponse.ResponseDataTypeNotAcceptable)
            {
                return new GenericResponseData<string>
                {
                    NotAcceptableDataResponse = true,
                    Errors = new List<string> { sdmxResponse.TextResponse },
                    ItemsCount = 0,
                    Timers = new Dictionary<string, string> { { "nsiResponse", $"{sdmxResponse.ResponseTime}ms" } }
                };
            }

            var response = new GenericResponseData<string>
            {
                Timers = new Dictionary<string, string>(),
                Data = sdmxResponse.FileResponse,
                ResponseType = sdmxRequest?.Headers?.Accept?.FirstOrDefault()?.MediaType
            };

            if (sdmxResponse?.ResponseContentRange?.Unit != null &&
                sdmxResponse.ResponseContentRange.Unit.Equals("values", StringComparison.InvariantCultureIgnoreCase))
            {
                response.ItemsCount = sdmxResponse.ResponseContentRange.HasLength
                    ? sdmxResponse.ResponseContentRange.Length.Value
                    : -1;
                response.ItemsFrom = sdmxResponse.ResponseContentRange.From.HasValue
                    ? sdmxResponse.ResponseContentRange.From.Value
                    : -1;
                response.ItemsTo = sdmxResponse.ResponseContentRange.To.HasValue
                    ? sdmxResponse.ResponseContentRange.To.Value
                    : -1;
            }


            response.Timers.Add("nsiResponse", $"{sdmxResponse.ResponseTime}ms");
            response.Timers.Add("nsiResponseDownload", $"{sdmxResponse.DownloadResponseTime}ms");
            response.Timers.Add("nsiResponseDownloadSize", $"{sdmxResponse.DownloadResponseSize}Kb");
            //response.Timers.Add("NSI Response Processed", $"{sdmxResponse.ObjectCompletedTime}ms");
            //response.Timers.Add("NSI Response Completed", $"{sdmxResponse.ResponseTime + sdmxResponse.DownloadResponseTime}ms");

            _logger.LogDebug($"END {MethodBase.GetCurrentMethod().Name}");
            return response;
        }

        public async Task<ISdmxObjects> GetDataflowWithUsedData(IDataflowObject df)
        {
            ISdmxObjects result;
            if (_endPointSDMXNodeConfig.CallDataflowWithoutPartial)
            {
                result = await GetArtefactAsync(SdmxStructureEnumType.Dataflow, df.Id, df.AgencyId,
                    df.Version, StructureReferenceDetailEnumType.All, "Full");
            }
            else
            {
                result =await GetArtefactAsync(SdmxStructureEnumType.Dataflow, df.Id, df.AgencyId,
                    df.Version, StructureReferenceDetailEnumType.All, "ReferencePartial");
            }

            return result;
        }

        #endregion


    }
}