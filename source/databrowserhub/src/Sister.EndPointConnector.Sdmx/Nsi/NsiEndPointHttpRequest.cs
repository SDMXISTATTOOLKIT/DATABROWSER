using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using EndPointConnector.Interfaces.Sdmx;
using EndPointConnector.Interfaces.Sdmx.Exceptions;
using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.Interfaces.Sdmx.Nsi;
using EndPointConnector.ParserSdmx;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Util.Xml;
using Sister.EndPointConnector.Sdmx.Constants;
using static TracertLOg.Tracing;

namespace Sister.EndPointConnector.Sdmx.Nsi
{
    public class NsiEndPointHttpRequest : INsiEndPointHttpRequest
    {
        private readonly ISdmxEndPointHttp _endPointHttp;
        private readonly EndPointSdmxConfig _endPointSDMXNodeConfig;
        private readonly ILogger<NsiEndPointHttpRequest> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly ISdmxCache _sdmxCache;
        private readonly SdmxParser _sdmxParser;
        private bool _nameSpaceIsConfigured;

        public NsiEndPointHttpRequest(EndPointSdmxConfig endPointSDMXNodeConfig,
            ISdmxEndPointHttp endPointHttp,
            ILoggerFactory loggerFactory,
            IMemoryCache memoryCache,
            ISdmxCache sdmxCache,
            bool nameSpaceIsConfigured = false)
        {
            _endPointSDMXNodeConfig = endPointSDMXNodeConfig;
            _endPointHttp = endPointHttp;
            _nameSpaceIsConfigured = nameSpaceIsConfigured;
            _sdmxCache = sdmxCache;
            _sdmxParser = new SdmxParser(loggerFactory);
            _logger = loggerFactory.CreateLogger<NsiEndPointHttpRequest>();
            _memoryCache = memoryCache;
        }

        public SdmxHttpRequestMessage CreateRequest(string queryString,
            SdmxEndPointCostant.RequestType requestType,
            HttpMethod httpMethod,
            bool includeCrossReference = true,
            bool fixXmlResponse = false,
            int? maxObservations = null,
            string contentType = null,
            Dictionary<string, string> keys = null,
            object contentData = null)
        {
            _logger.LogDebug($"START {MethodBase.GetCurrentMethod().Name}");
            if (!_nameSpaceIsConfigured)
            {
                _logger.LogWarning("Before of CreateRequest need to call ConfigureNameSpace()");
                throw new ArgumentNullException("call ConfigureNameSpace");
            }

            if (queryString == null)
            {
                _logger.LogWarning("Null paramiter, invalid request");
                throw new ArgumentNullException("paramiters");
            }

            var uriBase = _endPointSDMXNodeConfig.EndPointUrl.Trim();
            if (!uriBase.EndsWith("/", StringComparison.InvariantCultureIgnoreCase)) uriBase += "/";

            var currentMaxObs = maxObservations ?? _endPointSDMXNodeConfig.MaxObservationsAfterCriteria;

            _logger.LogDebug($"URI call: {uriBase}{queryString} \t Method: {httpMethod}");
            queryString =
                queryString.Replace("/../", "/ALL/").Replace("/./", "/ALL/"); //Fix incompatible URI paramiters for NSI
            var request = new SdmxHttpRequestMessage(httpMethod, $"{uriBase}{queryString}");
            if (requestType == SdmxEndPointCostant.RequestType.DataStructureSpecificV21Xml)
            {
                _logger.LogDebug("Accept Header: application/vnd.sdmx.structurespecificdata+xml;version=2.1");
                request.Headers.Remove("Accept");
                request.Headers.Add("Accept", "application/vnd.sdmx.structurespecificdata+xml;version=2.1");
                request.Headers.Remove("Range");
                request.Headers.Add("Range", $"values=0-{currentMaxObs - 1}");
            }
            else if (requestType == SdmxEndPointCostant.RequestType.DataGenericV20Xml)
            {
                _logger.LogDebug("Accept Header: application/vnd.sdmx.genericdata+xml;version=2.1;");
                request.Headers.Remove("Accept");
                request.Headers.Add("Accept", "application/vnd.sdmx.genericdata+xml;version=2.1");
                request.Headers.Remove("Range");
                request.Headers.Add("Range", $"values=0-{currentMaxObs - 1}");
            }
            else if (requestType == SdmxEndPointCostant.RequestType.DataGenericV21Xml)
            {
                _logger.LogDebug("Accept Header: application/vnd.sdmx.genericdata+xml; version=2.0; charset=utf-8");
                request.Headers.Remove("Accept");
                request.Headers.Add("Accept", "application/vnd.sdmx.genericdata+xml; version=2.0; charset=utf-8");
                request.Headers.Remove("Range");
                request.Headers.Add("Range", $"values=0-{currentMaxObs - 1}");
            }
            else if (requestType == SdmxEndPointCostant.RequestType.DataGenericV21Xml)
            {
                _logger.LogDebug("Accept Header: application/vnd.sdmx.genericdata+xml; version=2.0; charset=utf-8");
                request.Headers.Remove("Accept");
                request.Headers.Add("Accept", "application/vnd.sdmx.genericdata+xml; version=2.0; charset=utf-8");
                request.Headers.Remove("Range");
                request.Headers.Add("Range", $"values=0-{currentMaxObs - 1}");
            }
            else if (requestType == SdmxEndPointCostant.RequestType.DataCompactXml)
            {
                _logger.LogDebug("Accept Header: application/vnd.sdmx.compactdata+xml");
                request.Headers.Remove("Accept");
                request.Headers.Add("Accept", "application/vnd.sdmx.compactdata+xml");
                request.Headers.Remove("Range");
                request.Headers.Add("Range", $"values=0-{currentMaxObs - 1}");
            }
            else if (requestType == SdmxEndPointCostant.RequestType.DataSectionalCompactXml)
            {
                _logger.LogDebug("Accept Header: application/vnd.sdmx.crosssectionaldata+xml");
                request.Headers.Remove("Accept");
                request.Headers.Add("Accept", "application/vnd.sdmx.crosssectionaldata+xml");
                request.Headers.Remove("Range");
                request.Headers.Add("Range", $"values=0-{currentMaxObs - 1}");
            }
            else if (requestType == SdmxEndPointCostant.RequestType.DataEdiXml)
            {
                _logger.LogDebug("Accept Header: application/vnd.sdmx.edidata");
                request.Headers.Remove("Accept");
                request.Headers.Add("Accept", "application/vnd.sdmx.edidata");
                request.Headers.Remove("Range");
                request.Headers.Add("Range", $"values=0-{currentMaxObs - 1}");
            }
            else if (requestType == SdmxEndPointCostant.RequestType.DataCountString)
            {
                _logger.LogDebug(
                    "Accept Header (DataCount): application/vnd.sdmx.data+json; charset=utf-8; version=1.0");
                request.Headers.Remove("Accept");
                request.Headers.Add("Accept", "application/vnd.sdmx.data+json; charset=utf-8; version=1.0");
                request.Headers.Remove("Range");
                request.Headers.Add("Range", "values=0-0");
            }
            else if (requestType == SdmxEndPointCostant.RequestType.DataSdmxJson)
            {
                _logger.LogDebug("Accept Header: application/vnd.sdmx.data+json; charset=utf-8; version=1.0");
                request.Headers.Remove("Accept");
                request.Headers.Add("Accept", "application/vnd.sdmx.data+json; charset=utf-8; version=1.0");
                request.Headers.Remove("Range");
                request.Headers.Add("Range", $"values=0-{currentMaxObs - 1}");
            }
            else if (requestType == SdmxEndPointCostant.RequestType.DataCsvSdmxString)
            {
                _logger.LogDebug("Accept Header: application/vnd.sdmx.data+csv; charset=utf-8");
                request.Headers.Remove("Accept");
                request.Headers.Add("Accept", "application/vnd.sdmx.data+csv; charset=utf-8");
                request.Headers.Remove("Range");
                request.Headers.Add("Range", $"values=0-{currentMaxObs - 1}");
            }
            else if (requestType == SdmxEndPointCostant.RequestType.StructureJson)
            //else if (requestType == SdmxEndPointCostant.RequestType.StructureJson ||
            //            (requestType == SdmxEndPointCostant.RequestType.Structure &&
            //                !_endPointSDMXNodeConfig.RestDataResponseXml))
            {
                _logger.LogDebug("Accept Header: application/vnd.sdmx.structure+json; version=1.0; charset=utf-8");
                request.Headers.Remove("Accept");
                request.Headers.Add("Accept", "application/vnd.sdmx.structure+json; version=1.0; charset=utf-8");
                requestType = SdmxEndPointCostant.RequestType.StructureJson;
            }
            else //StructureXml
            {
                _logger.LogDebug("Accept Header: application/vnd.sdmx.structure+xml;version=2.1");
                request.Headers.Remove("Accept");
                request.Headers.Add("Accept", "application/vnd.sdmx.structure+xml;version=2.1");
                requestType = SdmxEndPointCostant.RequestType.StructureXml;
            }

            if (!string.IsNullOrWhiteSpace(contentType))
            {
                if (keys != null &&
                    contentType.Equals("application/x-www-form-urlencoded",
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug(
                            $"ContentType application/x-www-form-urlencoded: data: {JsonConvert.SerializeObject(keys)}");

                    request.Content = new FormUrlEncodedContent(keys);
                }
                else if (contentData != null &&
                         contentType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogDebug("application/json");
                    request.Content = new StringContent(JsonConvert.SerializeObject(contentData),
                        Encoding.UTF8,
                        "application/json");
                }
            }


            request.Headers.Remove("Accept-Language");
            var langs = _endPointSDMXNodeConfig.AcceptedLanguages.ToLowerInvariant();
            request.Headers.Add("Accept-Language", langs);
            request.Headers.Remove("Accept-Encoding");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");

            request.RequestType = requestType;
            request.XmlNeedFix = fixXmlResponse;
            request.IncludeCrossReference = includeCrossReference;
            _logger.LogDebug(
                $"XmlNeedFix: {request.XmlNeedFix}\tIncludeCrossReference: {request.IncludeCrossReference}\tAccept-Language:{langs}");
            _logger.LogDebug($"END {MethodBase.GetCurrentMethod().Name}");
            return request;
        }


        public SdmxHttpRequestMessage CreateRequest(XmlDocument xml, SdmxStructureEnumType structureType,
            bool withEnvelope, bool includeCrossReference = true, bool fixXmlResponse = false)
        {
            var actionType = SDMXUtils.GetQuerySoapAction(structureType);
            return CreateRequest(xml, actionType, withEnvelope, includeCrossReference, fixXmlResponse);
        }

        public SdmxHttpRequestMessage CreateRequest(XmlDocument xml, SdmxEndPointCostant.SDMXWSFunction sdmxFunction,
            bool withEnvelope, bool includeCrossReference = true, bool fixXmlResponse = false)
        {
            var actionType = sdmxFunction.ToString();
            return CreateRequest(xml, actionType, withEnvelope, includeCrossReference, fixXmlResponse);
        }

        public SdmxHttpRequestMessage CreateRequest(XmlDocument xml, SdmxEndPointCostant.SDMXWSFunctionV21 sdmxFunction,
            bool withEnvelope, bool includeCrossReference = true, bool fixXmlResponse = false)
        {
            var actionType = sdmxFunction.ToString();
            return CreateRequest(xml, actionType, withEnvelope, includeCrossReference, fixXmlResponse);
        }

        public async Task<SdmxHttpResponseMessage> SendRequestAsync(SdmxHttpRequestMessage httpRequest,
            bool saveResultOnFile = false, string specificFileToSave = null)
        {
            _logger.LogDebug("START SendRequestAsync");
            if (!_nameSpaceIsConfigured) throw new ArgumentNullException("call ConfigureNameSpace");

            var sdmxHttpResponseMessage = new SdmxHttpResponseMessage();
            var responseTotalTimeWatch = Stopwatch.StartNew();
            var responseWatch = Stopwatch.StartNew();
            _logger.LogInformation(
                $"Try call NSI Url: {httpRequest.RequestUri}\t Verb:{httpRequest.Method}\t Type:{httpRequest.RequestType}");
            using (var httpClient = _endPointHttp.CreateClientHttp())
            {
                using (var response = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead))
                {
                    sdmxHttpResponseMessage.ResponseType = httpRequest.RequestType;
                    sdmxHttpResponseMessage.ResponseTime = responseWatch.ElapsedMilliseconds;

                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        await WriteTraceAsync(requestBody: "",
                            requestUrl: httpRequest.RequestUri.ToString(),
                            operationName: OperationTypeEnum.NsiResponseHeader.ToString(),
                            requestOperation: httpRequest.RequestType.ToString(),
                            logDateTime: GetServerLogTime(),
                            operationId: _endPointSDMXNodeConfig.UserOperationGuid,
                            userGuid: _endPointSDMXNodeConfig.UserGuid,
                            responseTime: sdmxHttpResponseMessage.ResponseTime);
                    }

                    _logger.LogDebug($"Respone StatusCode: {response.StatusCode}");

                    

                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        //throw new Exception("SDMX WS: No artefacts found.");
                        sdmxHttpResponseMessage.NoResultFound = true;
                        return sdmxHttpResponseMessage;
                    }

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                        throw new AuthenticationException("UNAUTHORIZED");
                    if (response.StatusCode == HttpStatusCode.Forbidden)
                        throw new UnauthorizedAccessException("FORBIDDEN");

                    sdmxHttpResponseMessage.XmlResponse = new XmlDocument();
                    var tmpFilePath = "";
                    try
                    {
                        //await Tracing.WriteTraceAsync(requestBody: "",
                        //                    requestUrl: httpRequest.RequestUri.ToString(),
                        //                    operationName: OperationTypeEnum.NsiResponseMessage.ToString(),
                        //                    requestOperation: httpRequest.RequestType.ToString(),
                        //                    logDateTime: Tracing.GetServerLogTime(),
                        //                    operationId: _connectorContext.UserOperationGuid,
                        //                    userGuid: _connectorContext.UserGuid,
                        //                    responseTime: (int)responseWatch.ElapsedMilliseconds);
                        responseWatch.Restart();
                        if (response.IsSuccessStatusCode)
                        {
                            sdmxHttpResponseMessage.ResponseContentRange = response.Content.Headers.ContentRange;

                            if (sdmxHttpResponseMessage.ResponseTime > 300000 || //5min
                                saveResultOnFile ||
                                httpRequest.RequestType ==
                                SdmxEndPointCostant.RequestType.DataStructureSpecificV21Xml ||
                                httpRequest.RequestType == SdmxEndPointCostant.RequestType.DataSdmxJson ||
                                httpRequest.RequestType == SdmxEndPointCostant.RequestType.DataCsvSdmxString ||
                                response?.Content?.Headers?.ContentLength != null &&
                                response.Content.Headers.ContentLength.HasValue &&
                                response.Content.Headers.ContentLength.Value >= 100000000)
                                //Response size >=100Mb or response data (save response in tmp file)
                                tmpFilePath = await saveResponseOnFile(httpRequest, saveResultOnFile,
                                    specificFileToSave, sdmxHttpResponseMessage, responseWatch, response, tmpFilePath);
                            else
                                //Small response (save response in memory)
                                await saveResponseOnMemory(httpRequest, sdmxHttpResponseMessage, responseWatch,
                                    response);
                        }
                        else
                        {
                            await manageErrorResponse(sdmxHttpResponseMessage, responseWatch, response);
                        }
                    }
                    catch (SdmxLimitDataException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"SDMX WS [{_endPointSDMXNodeConfig.Code}]: {ex.Message}", ex);
                        if (!string.IsNullOrWhiteSpace(sdmxHttpResponseMessage?.TextResponse))
                            _logger.LogError("SDMX WS Error Message:" + sdmxHttpResponseMessage.TextResponse);
                        if (!string.IsNullOrWhiteSpace(sdmxHttpResponseMessage?.XmlResponse?.OuterXml))
                            _logger.LogError("SDMX WS Error Message:" + sdmxHttpResponseMessage.XmlResponse.OuterXml);
                        throw new Exception("SDMX WS: " + response.StatusCode + " - " + response.ReasonPhrase);
                    }
                    finally
                    {
                        if (!saveResultOnFile && !string.IsNullOrWhiteSpace(tmpFilePath) && File.Exists(tmpFilePath))
                            try
                            {
                                sdmxHttpResponseMessage.FileResponse = null;
                                File.Delete(tmpFilePath);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning($"Error during remove tmp xml file: {tmpFilePath}", ex);
                            }
                    }

                    if (httpRequest.XmlNeedFix && (httpRequest.RequestType ==
                                                   SdmxEndPointCostant.RequestType.DataStructureSpecificV21Xml ||
                                                   httpRequest.RequestType ==
                                                   SdmxEndPointCostant.RequestType.StructureXml))
                    {
                        var xmlDocumentFix = _sdmxParser.XmlDocumentFixed(sdmxHttpResponseMessage.XmlResponse,
                            SdmxSchemaEnumType.VersionTwoPointOne);
                        if (saveResultOnFile)
                            xmlDocumentFix.Save(sdmxHttpResponseMessage.FileResponse);
                        else
                            sdmxHttpResponseMessage.XmlResponse = xmlDocumentFix;
                    }

                    sdmxHttpResponseMessage.ObjectCompletedTime = responseWatch.ElapsedMilliseconds;


                    responseTotalTimeWatch.Stop();
                    //await Tracing.WriteTraceAsync(requestBody: "",
                    //                        requestUrl: "INTERNALUSE",
                    //                        operationName: "INTERNALUSE_NsiSendProcess",
                    //                        requestOperation: httpRequest.RequestType.ToString(),
                    //                        logDateTime: Tracing.GetServerLogTime(),
                    //                        operationId: _connectorContext.UserOperationGuid,
                    //                        userGuid: _connectorContext.UserGuid,
                    //                        responseTime: responseTotalTimeWatch.ElapsedMilliseconds);
                    //await Tracing.WriteTraceAsync(requestBody: "",
                    //                        requestUrl: httpRequest.RequestUri.ToString(),
                    //                        operationName: OperationTypeEnum.NsiDataMessageProcessed.ToString(),
                    //                        requestOperation: httpRequest.RequestType.ToString(),
                    //                        logDateTime: Tracing.GetServerLogTime(),
                    //                        operationId: _connectorContext.UserOperationGuid,
                    //                        userGuid: _connectorContext.UserGuid,
                    //                        responseTime: sdmxHttpResponseMessage.ObjectCompletedTime);
                    //await Tracing.WriteTraceAsync(requestBody: "",
                    //                        requestUrl: httpRequest.RequestUri.ToString(),
                    //                        operationName: OperationTypeEnum.NsiRequestEnd.ToString(),
                    //                        requestOperation: httpRequest.RequestType.ToString(),
                    //                        logDateTime: Tracing.GetServerLogTime(),
                    //                        operationId: _connectorContext.UserOperationGuid,
                    //                        userGuid: _connectorContext.UserGuid,
                    //                        responseTime: sdmxHttpResponseMessage.ObjectCompletedTime + 
                    //                                        sdmxHttpResponseMessage.DownloadResponseTime + 
                    //                                        sdmxHttpResponseMessage.ResponseTime);

                    _logger.LogDebug("END SendRequestAsync");
                    return sdmxHttpResponseMessage;
                }
            }
        }

        public SdmxHttpRequestMessage CreateRequest(XmlDocument xml, string actionType, bool withEnvelope,
            bool includeCrossReference, bool fixXmlResponse)
        {
            _logger.LogDebug($"START {MethodBase.GetCurrentMethod().Name}");
            if (!_nameSpaceIsConfigured)
            {
                _logger.LogWarning("Before of CreateRequest need to call ConfigureNameSpace()");
                throw new ArgumentNullException("call ConfigureNameSpace");
            }

            if (xml == null)
            {
                _logger.LogWarning("Null xml, invalid request");
                throw new ArgumentNullException("xml");
            }

            var doc = new XmlDocument();
            if (withEnvelope)
            {
                _logger.LogDebug("with envelope");
                var sb = new StringBuilder();
                sb.AppendFormat(SdmxSoapConstants.SoapRequest, _endPointSDMXNodeConfig.Prefix,
                    _endPointSDMXNodeConfig.Namespace);

                doc.LoadXml(sb.ToString());

                _logger.LogDebug("GetElementsByTagName");
                var nodes = doc.GetElementsByTagName(SdmxSoapConstants.Body, SdmxSoapConstants.Soap11Ns);
                _logger.LogDebug("CreateElement");
                var operation = doc.CreateElement(_endPointSDMXNodeConfig.Prefix, actionType,
                    _endPointSDMXNodeConfig.Namespace);
                //string parameterName = this._wsdlConfig.GetParameterName(operationName);
                //return always null
                string parameterName = null;
                var queryParent = operation;
                _logger.LogDebug("check parameterName");
                if (!string.IsNullOrEmpty(parameterName))
                {
                    _logger.LogDebug($"parameterName: {parameterName}");
                    queryParent = doc.CreateElement(_endPointSDMXNodeConfig.Prefix, parameterName,
                        _endPointSDMXNodeConfig.Namespace);
                    operation.AppendChild(queryParent);
                }

                if (xml.DocumentElement != null)
                {
                    _logger.LogDebug("DocumentElement not null");
                    var sdmxQueryNode = doc.ImportNode(xml.DocumentElement, true);
                    queryParent.AppendChild(sdmxQueryNode);
                }

                nodes[0].AppendChild(operation);
            }
            else
            {
                doc = xml;
            }

            //string soapAction = this._wsdlConfig.GetSoapAction(operationName);
            //GetCompactData  => ""
            //GetGenericData  => ""
            //GetUtilityData  => ""
            //GetCrossSectionalData  => ""
            //QueryStructure  => ""
            var soapAction = "";

            var httpRequestMessage =
                new SdmxHttpRequestMessage(HttpMethod.Post, _endPointSDMXNodeConfig.EndPointUrl.Trim());
            httpRequestMessage.Headers.Add("SOAPAction", soapAction);
            httpRequestMessage.Headers.Add("ContentType", "text/xml;charset=\"utf-8\"");
            httpRequestMessage.Headers.Add("Accept", "text/xml");
            httpRequestMessage.Headers.Add("Accept-Encoding", "gzip, deflate");
            httpRequestMessage.Headers.Add("Accept-Language",
                _endPointSDMXNodeConfig.AcceptedLanguages.ToLowerInvariant());
            httpRequestMessage.Content = new StringContent(doc.OuterXml, Encoding.UTF8, "text/xml");

            httpRequestMessage.RequestType =
                actionType == SdmxEndPointCostant.SDMXWSFunctionV21.GetCompactData.ToString() ||
                actionType == SdmxEndPointCostant.SDMXWSFunctionV21.GetStructureSpecificData.ToString() ||
                actionType == SdmxEndPointCostant.SDMXWSFunctionV21.GetCrossSectionalData.ToString()
                    ? SdmxEndPointCostant.RequestType.DataStructureSpecificV21Xml
                    : SdmxEndPointCostant.RequestType.StructureXml;
            httpRequestMessage.XmlNeedFix = fixXmlResponse;
            httpRequestMessage.IncludeCrossReference = includeCrossReference;
            _logger.LogDebug(
                $"XmlNeedFix: {httpRequestMessage.XmlNeedFix}\tIncludeCrossReference: {httpRequestMessage.IncludeCrossReference}\tSOAPAction: {soapAction}");

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("SOAP REQUEST: \n" + doc.OuterXml);
            }

            _logger.LogDebug($"END {MethodBase.GetCurrentMethod().Name}");
            return httpRequestMessage;
        }

        private async Task manageErrorResponse(SdmxHttpResponseMessage sdmxHttpResponseMessage, Stopwatch responseWatch,
            HttpResponseMessage response)
        {
            var messageResult = await response.Content.ReadAsStringAsync();
            sdmxHttpResponseMessage.DownloadResponseTime = responseWatch.ElapsedMilliseconds;
            responseWatch.Restart();

            sdmxHttpResponseMessage.ResponseContentType = response.Content.Headers.ContentType.MediaType;
            if (response.Content.Headers.ContentType.MediaType == "text/plain" ||
                response.Content.Headers.ContentType.MediaType == "text/html")
            {
                //TEXT
                sdmxHttpResponseMessage.TextResponse = messageResult;
                if (sdmxHttpResponseMessage.TextResponse.ToLowerInvariant().Contains("no available data found") ||
                    response.StatusCode == HttpStatusCode.NotFound)
                {
                    sdmxHttpResponseMessage.NoResultFound = true;
                    _logger.LogInformation($"404 not found: {sdmxHttpResponseMessage.TextResponse}");
                }
                else if (sdmxHttpResponseMessage.TextResponse.ToLowerInvariant().Contains("acceptable:") ||
                         response.StatusCode == HttpStatusCode.NotAcceptable)
                {
                    sdmxHttpResponseMessage.ResponseDataTypeNotAcceptable = true;
                    _logger.LogInformation($"406 not acceptable: {sdmxHttpResponseMessage.TextResponse}");
                }
                else
                {
                    sdmxHttpResponseMessage.NsiErrorResponse = true;
                    _logger.LogError(
                        $"NSI {sdmxHttpResponseMessage.StatusCode}: {sdmxHttpResponseMessage.TextResponse}");
                    throw new Exception(
                        $"NSI {sdmxHttpResponseMessage.StatusCode}: {sdmxHttpResponseMessage.TextResponse}");
                }
            }
            else
            {
                //XML
                sdmxHttpResponseMessage.XmlResponse = new XmlDocument();
                if (!string.IsNullOrWhiteSpace(messageResult))
                {
                    sdmxHttpResponseMessage.XmlResponse.LoadXml(messageResult);
                    sdmxHttpResponseMessage.NsiErrorResponse = true;
                    _logger.LogInformation(
                        $"NSI {sdmxHttpResponseMessage.StatusCode}: {sdmxHttpResponseMessage.XmlResponse.OuterXml}");
                    throw new Exception(
                        $"NSI {sdmxHttpResponseMessage.StatusCode}: {sdmxHttpResponseMessage.XmlResponse.OuterXml}");
                }
            }
        }

        private async Task saveResponseOnMemory(SdmxHttpRequestMessage httpRequest,
            SdmxHttpResponseMessage sdmxHttpResponseMessage, Stopwatch responseWatch, HttpResponseMessage response)
        {
            _logger.LogDebug("Respone small ContentLength, save in memory");
            if (httpRequest.RequestType != SdmxEndPointCostant.RequestType.DataCountString &&
                !responseIsXmlDataType(httpRequest.RequestType))
            {
                sdmxHttpResponseMessage.XmlResponse = null;
                sdmxHttpResponseMessage.TextResponse = await response.Content.ReadAsStringAsync();
                if (_logger.IsEnabled(LogLevel.Debug))
                    sdmxHttpResponseMessage.DownloadResponseSize =
                        Encoding.Unicode.GetByteCount(sdmxHttpResponseMessage.TextResponse) / 1024;
                sdmxHttpResponseMessage.DownloadResponseTime = responseWatch.ElapsedMilliseconds;
                responseWatch.Restart();
                logResponseFromText(sdmxHttpResponseMessage.TextResponse);
            }
            else if (httpRequest.RequestType == SdmxEndPointCostant.RequestType.DataCountString)
            {
                sdmxHttpResponseMessage.XmlResponse = null;
                sdmxHttpResponseMessage.TextResponse = null;
                sdmxHttpResponseMessage.DownloadResponseTime = responseWatch.ElapsedMilliseconds;
                responseWatch.Restart();
            }
            else
            {
                //Xml
                using (var strWriter = new StringWriterUTF8())
                {
                    using (var xmlWriter = XmlWriter.Create(strWriter))
                    {
                        SoapUtils.ExtractSdmxMessage(await response.Content.ReadAsStreamAsync(), xmlWriter);
                    }

                    sdmxHttpResponseMessage.DownloadResponseTime = responseWatch.ElapsedMilliseconds;
                    if (_logger.IsEnabled(LogLevel.Debug))
                        sdmxHttpResponseMessage.DownloadResponseSize =
                            Encoding.Unicode.GetByteCount(sdmxHttpResponseMessage.XmlResponse.OuterXml) / 1024;

                    sdmxHttpResponseMessage.XmlResponse.LoadXml(strWriter.ToString());
                    responseWatch.Restart();
                    logResponseFromXml(sdmxHttpResponseMessage.XmlResponse);
                }
            }

            _logger.LogDebug("Respone small DONE");
        }

        private async Task<string> saveResponseOnFile(SdmxHttpRequestMessage httpRequest, bool saveResultOnFile,
            string specificFileToSave, SdmxHttpResponseMessage sdmxHttpResponseMessage, Stopwatch responseWatch,
            HttpResponseMessage response, string tmpFilePath)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"Respone big ContentLength: {response.Content?.Headers?.ContentLength ?? -1}");
            _logger.LogDebug("Respone big Start");

            using (var streamToReadFrom = await response.Content.ReadAsStreamAsync())
            {
                tmpFilePath = string.IsNullOrWhiteSpace(specificFileToSave)
                    ? SDMXUtils.GetTempSdmxFileName()
                    : specificFileToSave;
                sdmxHttpResponseMessage.FileResponse = tmpFilePath;
                sdmxHttpResponseMessage.TextResponse = null;
                sdmxHttpResponseMessage.XmlResponse = null;


                if (saveResultOnFile && responseIsXmlDataType(httpRequest.RequestType))
                {
                    //Save Xml on File
                    var settings = new XmlWriterSettings();
                    settings.Indent = false;
                    using (var writer = XmlWriter.Create(tmpFilePath, settings))
                    {
                        SoapUtils.ExtractSdmxMessage(streamToReadFrom, writer);
                    }

                    sdmxHttpResponseMessage.DownloadResponseTime = responseWatch.ElapsedMilliseconds;
                    responseWatch.Restart();
                }
                else if (saveResultOnFile && !responseIsXmlDataType(httpRequest.RequestType))
                {
                    //Save String/Json on File
                    using (Stream streamToWriteTo = File.Open(tmpFilePath, FileMode.Create))
                    {
                        await streamToReadFrom.CopyToAsync(streamToWriteTo);
                    }

                    sdmxHttpResponseMessage.DownloadResponseTime = responseWatch.ElapsedMilliseconds;
                    responseWatch.Restart();
                }
                else if (!saveResultOnFile && responseIsXmlDataType(httpRequest.RequestType))
                {
                    //Save Xml on Memory
                    var settings = new XmlWriterSettings();
                    settings.Indent = false;
                    using (var writer = XmlWriter.Create(tmpFilePath, settings))
                    {
                        SoapUtils.ExtractSdmxMessage(streamToReadFrom, writer);
                    }

                    sdmxHttpResponseMessage.DownloadResponseTime = responseWatch.ElapsedMilliseconds;
                    responseWatch.Restart();

                    sdmxHttpResponseMessage.XmlResponse = new XmlDocument();
                    sdmxHttpResponseMessage.XmlResponse.LoadXml(await File.ReadAllTextAsync(tmpFilePath));
                    sdmxHttpResponseMessage.TextResponse = null;
                    sdmxHttpResponseMessage.FileResponse = null;
                }
                else if (!saveResultOnFile && !responseIsXmlDataType(httpRequest.RequestType))
                {
                    //Save String/Json on Memory
                    using (Stream streamToWriteTo = File.Open(tmpFilePath, FileMode.Create))
                    {
                        await streamToReadFrom.CopyToAsync(streamToWriteTo);
                    }

                    sdmxHttpResponseMessage.DownloadResponseTime = responseWatch.ElapsedMilliseconds;
                    sdmxHttpResponseMessage.XmlResponse = null;
                    sdmxHttpResponseMessage.TextResponse = await File.ReadAllTextAsync(tmpFilePath);
                    sdmxHttpResponseMessage.FileResponse = null;
                }

                if (_logger.IsEnabled(LogLevel.Debug))
                    sdmxHttpResponseMessage.DownloadResponseSize = new FileInfo(tmpFilePath).Length / 1024;
                logResponseFromFile(tmpFilePath);
            }

            _logger.LogDebug("Respone big DONE");
            return tmpFilePath;
        }

        private bool responseIsXmlDataType(SdmxEndPointCostant.RequestType requestResponseType)
        {
            return requestResponseType == SdmxEndPointCostant.RequestType.DataStructureSpecificV21Xml ||
                   requestResponseType == SdmxEndPointCostant.RequestType.StructureXml ||
                   requestResponseType == SdmxEndPointCostant.RequestType.DataGenericV20Xml ||
                   requestResponseType == SdmxEndPointCostant.RequestType.DataGenericV21Xml ||
                   requestResponseType == SdmxEndPointCostant.RequestType.DataCompactXml ||
                   requestResponseType == SdmxEndPointCostant.RequestType.DataSectionalCompactXml;
        }

        public async Task<INsiEndPointHttpRequest> ConfigureNameSpaceAsync()
        {
            _logger.LogDebug("START ConfigureNameSpaceAsync");
            var memoryKey = $"{_endPointSDMXNodeConfig.Code}EndPoint:{_endPointSDMXNodeConfig.EndPointType}:Namespace";
            var sdmxKey = $"EndPoint:{_endPointSDMXNodeConfig.EndPointType}:Namespace";

            if (_nameSpaceIsConfigured)
            {
                _logger.LogDebug("Namespace already configured");
                return this;
            }

            _nameSpaceIsConfigured = true;
            if (_endPointSDMXNodeConfig.EndPointType != SdmxEndPointCostant.ConnectorType.SoapV20 &&
                _endPointSDMXNodeConfig.EndPointType != SdmxEndPointCostant.ConnectorType.SoapV21)
            {
                _logger.LogDebug($"{_endPointSDMXNodeConfig.EndPointType} no need configuration");
                return this;
            }

            //Memory Cache
            if (_memoryCache != null)
            {
                _endPointSDMXNodeConfig.Namespace = _memoryCache.Get<string>(memoryKey);
                if (!string.IsNullOrWhiteSpace(_endPointSDMXNodeConfig.Namespace))
                {
                    _logger.LogDebug("END FROM MEMORY CACHE");
                    return this;
                }
            }

            //Sdmx Cache
            if (_sdmxCache != null && !_sdmxCache.DisableNamespace)
            {
                _endPointSDMXNodeConfig.Namespace = await _sdmxCache.GetGlobalGenericAsync(sdmxKey);
                if (!string.IsNullOrWhiteSpace(_endPointSDMXNodeConfig.Namespace))
                {
                    if (_memoryCache != null) _memoryCache.Set(memoryKey, _endPointSDMXNodeConfig.Namespace);
                    _logger.LogDebug("END FROM SDMX CACHE");
                    return this;
                }
            }

            //Request for get namespace
            using (var request = new HttpRequestMessage())
            {
                request.RequestUri = new Uri(string.IsNullOrWhiteSpace(_endPointSDMXNodeConfig.InitialWSDL)
                    ? _endPointSDMXNodeConfig.EndPointUrl.Trim() + "?wsdl"
                    : _endPointSDMXNodeConfig.InitialWSDL.Trim());
                request.Method = HttpMethod.Get;
                request.Headers.Clear();
                _logger.LogDebug($"request uri for namespace {request.RequestUri}");
                using (var httpCLient = _endPointHttp.CreateClientHttp())
                {
                    using (var soapResponse = await httpCLient.SendAsync(request))
                    {
                        if (soapResponse.StatusCode != HttpStatusCode.OK)
                        {
                            _logger.LogError($"SDMX WS: {soapResponse.StatusCode} - {soapResponse.ReasonPhrase}");
                            throw new Exception($"SDMX WS: {soapResponse.StatusCode} - {soapResponse.ReasonPhrase}");
                        }

                        var soapString = await soapResponse.Content.ReadAsStringAsync();

                        var indexFirst = soapString.IndexOf("targetNamespace=\"",
                            StringComparison.InvariantCultureIgnoreCase);
                        if (indexFirst >= 0)
                        {
                            var startString = soapString.Substring(indexFirst).Replace("targetNamespace=\"", "");
                            var indexLast = startString.IndexOf('"');
                            if (indexLast >= 0)
                            {
                                _endPointSDMXNodeConfig.Namespace = startString.Substring(0, indexLast);
                                if (_memoryCache != null)
                                    _memoryCache.Set(memoryKey, _endPointSDMXNodeConfig.Namespace);
                                if (_sdmxCache != null && !_sdmxCache.DisableNamespace)
                                    _sdmxCache.SetGlobalGenericAsync(sdmxKey, _endPointSDMXNodeConfig.Namespace);
                            }
                            else
                            {
                                _logger.LogError("WSDL not found");
                            }
                        }
                        else
                        {
                            _logger.LogError("WSDL not found");
                        }
                    }
                }
            }

            _logger.LogDebug("END ConfigureNameSpaceAsync");
            return this;
        }

        private void logResponseFromText(string responseMessage)
        {
            try
            {
                if (_logger.IsEnabled(LogLevel.Debug) &&
                    _endPointSDMXNodeConfig.EndPointResponseLogForDebug)
                    _logger.LogDebug("NSI RESPONSE TEXT" + responseMessage);
            }
            catch (Exception)
            {
            }
        }

        private void logResponseFromXml(XmlDocument xmlDocument)
        {
            try
            {
                if (_logger.IsEnabled(LogLevel.Debug) &&
                    _endPointSDMXNodeConfig.EndPointResponseLogForDebug)
                    _logger.LogDebug("NSI RESPONSE TEXT" + xmlDocument.InnerText);
            }
            catch (Exception)
            {
            }
        }

        private void logResponseFromFile(string filePath)
        {
            try
            {
                if (_logger.IsEnabled(LogLevel.Debug) &&
                    _endPointSDMXNodeConfig.EndPointResponseLogForDebug &&
                    !string.IsNullOrWhiteSpace(filePath))
                    _logger.LogDebug("NSI RESPONSE TEXT" + File.ReadAllText(filePath));
            }
            catch (Exception)
            {
            }
        }

        public class StringWriterUTF8 : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}