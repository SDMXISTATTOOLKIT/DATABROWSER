using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using DataBrowser.Interfaces.Cache;
using EndPointConnector.Interfaces;
using EndPointConnector.Interfaces.JsonStat;
using EndPointConnector.Interfaces.Sdmx;
using EndPointConnector.Interfaces.Sdmx.Dm;
using EndPointConnector.Interfaces.Sdmx.Ma;
using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.Interfaces.Sdmx.Nsi;
using EndPointConnector.Interfaces.Sdmx.Nsi.Get;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sister.EndPointConnector.Sdmx.Connector.Nsi;
using Sister.EndPointConnector.Sdmx.Nsi;
using Sister.EndPointConnector.Sdmx.Nsi.Rest;
using Sister.EndPointConnector.Sdmx.Nsi.Soap;
using Sister.EndPointConnector.Sdmx.Nsi.Soap.Get;

namespace Sister.EndPointConnector.Sdmx
{
    public class SdmxEndPointManager : ISdmxSpecificEndPointFactory, ISdmxEndPointFactory
    {
        private readonly EndPointAppConfig _endPointAppConfig;
        private readonly IFromSdmxXmlToJsonStatConverterFactory _fromSdmxXmlToJsonStatConverterFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IFromSdmxJsonToJsonStatConverterFactory _jsonStatConverterFactory;
        private readonly ILogger<SdmxEndPointManager> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ISdmxCache _sdmxCache;

        public SdmxEndPointManager(IOptionsSnapshot<EndPointAppConfig> endPointConfig,
            ILogger<SdmxEndPointManager> logger,
            IHttpClientFactory httpClientFactory,
            ILoggerFactory loggerFactory,
            IMemoryCache memoryCache,
            ISdmxCache sdmxCache,
            IFromSdmxJsonToJsonStatConverterFactory jsonStatConverterFactory,
            IFromSdmxXmlToJsonStatConverterFactory fromSdmxXmlToJsonStatConverterFactory)
        {
            _endPointAppConfig = endPointConfig.Value;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _loggerFactory = loggerFactory;
            _memoryCache = memoryCache;
            _sdmxCache = sdmxCache;
            _jsonStatConverterFactory = jsonStatConverterFactory;
            _fromSdmxXmlToJsonStatConverterFactory = fromSdmxXmlToJsonStatConverterFactory;
        }

        public async Task<IEndPointConnector> CreateConnector(EndPointConfig endPointConfig)
        {
            _logger.LogDebug("START CreateConnector");
            var nsiConnector =
                await CreateSdmxConnectorAsync(endPointConfig, SdmxEndPointCostant.ConnectorType.FromConfig, true);
            var result = new SdmxConnector(nsiConnector);
            _logger.LogDebug("END CreateConnector");
            return result;
        }

        public IDmConnector CreateDmConnector(EndPointConfig endPointConfig)
        {
            throw new NotImplementedException();
        }

        public IMaConnector CreateMaConnector(EndPointConfig nodeWS)
        {
            throw new NotImplementedException();
        }

        public async Task<INsiConnector> CreateSdmxConnectorAsync(EndPointConfig endPointConfig,
            SdmxEndPointCostant.ConnectorType endPointType = SdmxEndPointCostant.ConnectorType.FromConfig,
            bool inizializeSoapV20 = false)
        {
            _logger.LogDebug("START CreateSdmxConnectorAsync");


            var endPointSDMXNodeConfig = createSdmxConfig(endPointConfig, endPointType);
            ISdmxEndPointHttp endPointHttp =
                new EndPointHttp(_httpClientFactory, endPointSDMXNodeConfig, _loggerFactory);


            var endPointHttpRequest =
                await new NsiEndPointHttpRequest(endPointSDMXNodeConfig, endPointHttp, _loggerFactory, _memoryCache,
                    _sdmxCache).ConfigureNameSpaceAsync();
            INsiGet sdmxGet = null;
            INsiGet sdmxGetSoap = null;
            INsiGetV20 sdmxGetV20 = null;
            
            if (endPointSDMXNodeConfig.EndPointType == SdmxEndPointCostant.ConnectorType.SoapV20)
            {
                sdmxGetV20 = new NsiGetV20Soap(endPointHttpRequest, _loggerFactory);
                sdmxGet = new NsiGetSoap(endPointHttpRequest, sdmxGetV20, endPointSDMXNodeConfig, _loggerFactory);
            }
            else if (endPointSDMXNodeConfig.EndPointType == SdmxEndPointCostant.ConnectorType.SoapV21 ||
                     endPointSDMXNodeConfig.OptimizeCallWithSoap)
            {
                if (inizializeSoapV20)
                {
                    //use the same IEndPointHttp, but specific EndPointSDMXNodeConfig foe endpoint SOAPV20 config
                    var configV20 = createSdmxConfig(endPointConfig, SdmxEndPointCostant.ConnectorType.SoapV20);
                    configV20.EndPointUrl = endPointSDMXNodeConfig.EndPointV20;
                    configV20.InitialWSDL = endPointSDMXNodeConfig.InitialWSDLV20;
                    configV20.Namespace = endPointSDMXNodeConfig.NamespaceV20;
                    var endPointHttpRequestV20 =
                        await new NsiEndPointHttpRequest(configV20, endPointHttp, _loggerFactory, _memoryCache,
                            _sdmxCache).ConfigureNameSpaceAsync();
                    sdmxGetV20 = new NsiGetV20Soap(endPointHttpRequestV20, _loggerFactory);
                }

                if (endPointSDMXNodeConfig.OptimizeCallWithSoap)
                {
                    var configV21 = createSdmxConfig(endPointConfig, SdmxEndPointCostant.ConnectorType.SoapV21);
                    configV21.EndPointUrl = endPointSDMXNodeConfig.EndPointV21;
                    configV21.InitialWSDL = endPointSDMXNodeConfig.InitialWSDL;
                    configV21.Namespace = endPointSDMXNodeConfig.Namespace;
                    var endPointHttpRequestV21 =
                        await new NsiEndPointHttpRequest(configV21, endPointHttp, _loggerFactory, _memoryCache,
                            _sdmxCache).ConfigureNameSpaceAsync();
                    sdmxGetSoap = new NsiGetSoap(endPointHttpRequestV21, sdmxGetV20, configV21, _loggerFactory);
                }
                else
                {
                    sdmxGet = new NsiGetSoap(endPointHttpRequest, sdmxGetV20, endPointSDMXNodeConfig, _loggerFactory);
                }
            }

            if (endPointSDMXNodeConfig.EndPointType == SdmxEndPointCostant.ConnectorType.Rest)
            {
                var provvisorio = true;
                if (provvisorio &&
                    !string.IsNullOrWhiteSpace(endPointSDMXNodeConfig.EndPointV20) &&
                    endPointSDMXNodeConfig.EnableEndPointV20)
                {
                    //endPointSDMXNodeConfig.EndPointV20 = "http://demost-mdm.sister.it/NSIWSST/NSIEstatV20Service";
                    var configV20 = createSdmxConfig(endPointConfig, SdmxEndPointCostant.ConnectorType.SoapV20);
                    configV20.EndPointUrl = endPointSDMXNodeConfig.EndPointV20;
                    configV20.InitialWSDL = endPointSDMXNodeConfig.InitialWSDLV20;
                    configV20.Namespace = endPointSDMXNodeConfig.NamespaceV20;
                    var endPointHttpRequestV20 =
                        await new NsiEndPointHttpRequest(configV20, endPointHttp, _loggerFactory, _memoryCache,
                            _sdmxCache).ConfigureNameSpaceAsync();
                    sdmxGetV20 = new NsiGetV20Soap(endPointHttpRequestV20, _loggerFactory);
                    //endPointSDMXNodeConfig.EndPointV21 = "http://demost-mdm.sister.it/NSIWSST/SdmxRegistryService";
                    var configV21 = createSdmxConfig(endPointConfig, SdmxEndPointCostant.ConnectorType.SoapV21);
                    configV21.EndPointUrl = endPointSDMXNodeConfig.EndPointV21;
                    configV21.InitialWSDL = endPointSDMXNodeConfig.InitialWSDL;
                    configV21.Namespace = endPointSDMXNodeConfig.Namespace;
                    //var endPointHttpRequestV21 = await new NsiEndPointHttpRequest(configV21, endPointHttp, _loggerFactory, _memoryCache, _sdmxCache);
                    sdmxGetSoap = new NsiGetSoap(null, sdmxGetV20, configV21, _loggerFactory);
                }

                //Rest 
                sdmxGet = new NsiGetRest(endPointHttpRequest, endPointSDMXNodeConfig, _loggerFactory, sdmxGetSoap);
            }

            INsiConnector endPoint = new NsiConnector(sdmxGet, endPointType, _loggerFactory, _sdmxCache, endPointSDMXNodeConfig,
                _fromSdmxXmlToJsonStatConverterFactory, _jsonStatConverterFactory);


            _logger.LogDebug("END CreateSdmxConnectorAsync");
            return endPoint;
        }


        private EndPointSdmxConfig createSdmxConfig(EndPointConfig endPointConfig,
            SdmxEndPointCostant.ConnectorType endPointType)
        {
            _logger.LogDebug($"START {MethodBase.GetCurrentMethod().Name}");
            if (endPointType == SdmxEndPointCostant.ConnectorType.FromConfig)
            {
                _logger.LogDebug($"Type value {endPointConfig.Type}");
                switch (endPointConfig.Type.ToUpperInvariant())
                {
                    case "SDMX-SOAPV20":
                        endPointType = SdmxEndPointCostant.ConnectorType.SoapV20;
                        break;
                    case "SDMX-SOAPV21":
                        endPointType = SdmxEndPointCostant.ConnectorType.SoapV21;
                        break;
                    case "SDMX-REST":
                        endPointType = SdmxEndPointCostant.ConnectorType.Rest;
                        break;
                    default:
                        throw new ArgumentException("EndPointType not supported");
                }
            }

            _logger.LogDebug($"Type enum {endPointType}");

            var endPointWSNodeConfig = new EndPointSdmxConfig(endPointConfig);

            _logger.LogDebug("EndPointSdmxConfig mapped");
            endPointWSNodeConfig.EndPointAppConfig = _endPointAppConfig;
            endPointConfig.Extras = endPointConfig.Extras ?? new Dictionary<string, object>();
            if (endPointType == SdmxEndPointCostant.ConnectorType.Rest)
            {
                endPointWSNodeConfig.EndPointType = endPointType;
                endPointWSNodeConfig.RestDataResponseXml =
                    Convert.ToBoolean(getExtraValue<string>(endPointConfig.Extras, "RestDataResponseXml"));
                endPointWSNodeConfig.SupportAllCompleteStubs =
                    Convert.ToBoolean(getExtraValue<string>(endPointConfig.Extras, "SupportAllCompleteStubs"));
                endPointWSNodeConfig.EndPointV20 = getExtraValue<string>(endPointConfig.Extras, "EndPointV20");
                endPointWSNodeConfig.EndPointV21 = getExtraValue<string>(endPointConfig.Extras, "EndPointV21");
                endPointWSNodeConfig.InitialWSDL = getExtraValue<string>(endPointConfig.Extras, "InitialWSDLV21");
                endPointWSNodeConfig.InitialWSDLV20 = getExtraValue<string>(endPointConfig.Extras, "InitialWSDLV20");
                endPointWSNodeConfig.Namespace = getExtraValue<string>(endPointConfig.Extras, "NamespaceV21");
                endPointWSNodeConfig.NamespaceV20 = getExtraValue<string>(endPointConfig.Extras, "NamespaceV20");
                endPointWSNodeConfig.OptimizeCallWithSoap =
                    Convert.ToBoolean(getExtraValue<string>(endPointConfig.Extras, "OptimizeCallWithSoap"));
                if (string.IsNullOrWhiteSpace(endPointWSNodeConfig.EndPointV20) ||
                    string.IsNullOrWhiteSpace(endPointWSNodeConfig.EndPointV21))
                    endPointWSNodeConfig.OptimizeCallWithSoap = false;
            }
            else if (endPointType == SdmxEndPointCostant.ConnectorType.SoapV20)
            {
                endPointWSNodeConfig.EndPointType = endPointType;
                endPointWSNodeConfig.EndPointV20 = getExtraValue<string>(endPointConfig.Extras, "EndPointV20");
                endPointWSNodeConfig.InitialWSDL = getExtraValue<string>(endPointConfig.Extras, "InitialWSDLV20");
                endPointWSNodeConfig.Namespace = getExtraValue<string>(endPointConfig.Extras, "NamespaceV20");
            }
            else if (endPointType == SdmxEndPointCostant.ConnectorType.SoapV21)
            {
                endPointWSNodeConfig.EndPointType = endPointType;
                endPointWSNodeConfig.EndPointV20 = getExtraValue<string>(endPointConfig.Extras, "EndPointV20");
                endPointWSNodeConfig.InitialWSDL = getExtraValue<string>(endPointConfig.Extras, "InitialWSDLV21");
                endPointWSNodeConfig.InitialWSDLV20 = getExtraValue<string>(endPointConfig.Extras, "NamespaceV20");
                endPointWSNodeConfig.Namespace = getExtraValue<string>(endPointConfig.Extras, "NamespaceV21");
            }

            endPointWSNodeConfig.XmlResultNeedFix = Convert.ToBoolean(getExtraValue<string>(endPointConfig.Extras, "XmlResultNeedFix"));
            endPointWSNodeConfig.Prefix = getExtraValue<string>(endPointConfig.Extras, "Prefix") ?? "web";
            endPointWSNodeConfig.SupportPostFilters = Convert.ToBoolean(getExtraValue<string>(endPointConfig.Extras, "SupportPostFilters"));
            endPointWSNodeConfig.CallDataflowWithoutPartial = Convert.ToBoolean(getExtraValue<string>(endPointConfig.Extras, "CallDataflowWithoutPartial"));
            endPointWSNodeConfig.EnableEndPointV20 = Convert.ToBoolean(getExtraValue<string>(endPointConfig.Extras, "EnableEndPointV20")); 

            var annotationConfig = getExtraValue<string>(endPointConfig.Extras, "AnnotationConfig");
            if (!string.IsNullOrWhiteSpace(annotationConfig))
                endPointWSNodeConfig.AnnotationConfig =
                    JsonConvert.DeserializeObject<EndPointCustomAnnotationConfig>(annotationConfig);

            _logger.LogDebug($"END {MethodBase.GetCurrentMethod().Name}");
            return endPointWSNodeConfig;
        }

        private T getExtraValue<T>(Dictionary<string, object> extras, string key)
        {
            object result;
            if (extras.TryGetValue(key, out result) && result is T) return (T) result;
            return default;
        }
    }
}