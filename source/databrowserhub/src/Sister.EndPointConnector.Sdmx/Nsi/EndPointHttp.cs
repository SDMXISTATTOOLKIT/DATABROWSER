using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using EndPointConnector.Interfaces.Sdmx;
using EndPointConnector.Interfaces.Sdmx.Models;
using Microsoft.Extensions.Logging;

namespace Sister.EndPointConnector.Sdmx.Nsi
{
    public class EndPointHttp : ISdmxEndPointHttp
    {
        private readonly EndPointSdmxConfig _endPointSDMXNodeConfig;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EndPointHttp> _logger;

        public EndPointHttp(IHttpClientFactory httpClientFactory, EndPointSdmxConfig endPointSDMXNodeConfig,
            ILoggerFactory loggerFactory)
        {
            _endPointSDMXNodeConfig = endPointSDMXNodeConfig;
            _httpClientFactory = httpClientFactory;
            _logger = loggerFactory.CreateLogger<EndPointHttp>();
        }

        public HttpClient CreateClientHttp()
        {
            _logger.LogDebug($"START {MethodBase.GetCurrentMethod().Name}");
            HttpClient httpClient;
            var httpClientHandler = ConfigureProxy();
            if (httpClientHandler == null)
            {
                if (_httpClientFactory == null)
                {
                    _logger.LogWarning("HttpClientFactory is null, create new instance of HttpClient");
                    httpClientHandler = new HttpClientHandler
                    {
                        Proxy = WebRequest.DefaultWebProxy,
                        AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
                    };
                    httpClient = new HttpClient(httpClientHandler);
                    httpClient.Timeout = TimeSpan.FromMinutes(120);
                }
                else
                {
                    _logger.LogDebug("Create default HttpCLient for HttpClientFactory");
                    httpClient = _httpClientFactory.CreateClient("default");
                }
            }
            else
            {
                _logger.LogDebug("Create new instance of HttpCLient with custom proxy");
                httpClient = new HttpClient(httpClientHandler);
                httpClient.Timeout = TimeSpan.FromMinutes(120);
            }

            ConfigureAuthentication(httpClient);

            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");

            _logger.LogDebug($"END {MethodBase.GetCurrentMethod().Name}");
            return httpClient;
        }

        private void ConfigureAuthentication(HttpClient httpClient)
        {
            if (_endPointSDMXNodeConfig.EnableHttpAuth &&
                !string.IsNullOrWhiteSpace(_endPointSDMXNodeConfig.HttpAuthUsername))
            {
                _logger.LogDebug("configure basic auth for endpoint");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(
                        $"{_endPointSDMXNodeConfig.HttpAuthUsername}:{_endPointSDMXNodeConfig.HttpAuthPassword}")));
            }
        }

        private HttpClientHandler ConfigureProxy()
        {
            _logger.LogDebug($"START {MethodBase.GetCurrentMethod().Name}");
            HttpClientHandler httpClientHandler = null;
            if (_endPointSDMXNodeConfig.EnableProxy)
            {
                _logger.LogDebug("Proxy is enable");
                httpClientHandler = new HttpClientHandler
                {
                    UseProxy = true,
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
                };

                if (_endPointSDMXNodeConfig.UseProxySystem)
                {
                    _logger.LogDebug("use system proxy");
                    httpClientHandler.Proxy = WebRequest.DefaultWebProxy;
                }
                else
                {
                    _logger.LogDebug("create proxy confgiuration");
                    var proxy = new WebProxy(_endPointSDMXNodeConfig.ProxyAddress, _endPointSDMXNodeConfig.ProxyPort)
                        {BypassProxyOnLocal = false};
                    if (!string.IsNullOrWhiteSpace(_endPointSDMXNodeConfig.ProxyUsername))
                    {
                        //Set credentials
                        ICredentials credentials = new NetworkCredential(_endPointSDMXNodeConfig.ProxyUsername,
                            _endPointSDMXNodeConfig.ProxyPassword);
                        proxy.Credentials = credentials;
                    }

                    //Inizialize HttpClient with proxy
                    httpClientHandler.Proxy = proxy;
                }
            }

            _logger.LogDebug($"END {MethodBase.GetCurrentMethod().Name}");
            return httpClientHandler;
        }
    }
}