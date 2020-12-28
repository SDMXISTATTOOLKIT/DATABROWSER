using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using EndPointConnector.Interfaces.Sdmx.Models;
using Org.Sdmxsource.Sdmx.Api.Constants;

namespace EndPointConnector.Interfaces.Sdmx.Nsi
{
    public interface INsiEndPointHttpRequest
    {
        /// <summary>
        ///     Create a HttpRequestMessage for SDMX request with query string
        /// </summary>
        SdmxHttpRequestMessage CreateRequest(string queryString,
            SdmxEndPointCostant.RequestType requestType,
            HttpMethod httpMethod,
            bool includeCrossReference = true,
            bool fixXmlResponse = false,
            int? maxObservations = null,
            string contentType = null,
            Dictionary<string, string> keys = null,
            object contentData = null);

        /// <summary>
        ///     Create a HttpRequestMessage for SDMX request with XML message
        /// </summary>
        SdmxHttpRequestMessage CreateRequest(XmlDocument xml, SdmxEndPointCostant.SDMXWSFunction sdmxFunction,
            bool withEnvelope, bool includeCrossReference = true, bool fixXmlResponse = false);

        SdmxHttpRequestMessage CreateRequest(XmlDocument xml, SdmxEndPointCostant.SDMXWSFunctionV21 sdmxFunction,
            bool withEnvelope, bool includeCrossReference = true, bool fixXmlResponse = false);

        SdmxHttpRequestMessage CreateRequest(XmlDocument xml, SdmxStructureEnumType structureType, bool withEnvelope,
            bool includeCrossReference = true, bool fixXmlResponse = false);

        /// <summary>
        ///     Sends the specified <paramref name="request" /> to the Web Service defined by <see cref="_config" />
        /// </summary>
        Task<SdmxHttpResponseMessage> SendRequestAsync(SdmxHttpRequestMessage httpRequest,
            bool saveResultOnFile = false, string specificFileToSave = null);
    }
}