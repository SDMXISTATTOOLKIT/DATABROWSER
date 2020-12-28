using System.Net.Http;

namespace EndPointConnector.Interfaces.Sdmx
{
    public interface ISdmxEndPointHttp
    {
        /// <summary>
        ///     Create a HttpClient with authentication and proxy configured
        /// </summary>
        HttpClient CreateClientHttp();
    }
}