using System;
using System.Net.Http;

namespace EndPointConnector.Interfaces.Sdmx.Models
{
    public class SdmxHttpRequestMessage : HttpRequestMessage
    {
        public SdmxHttpRequestMessage(HttpMethod method, Uri requestUri) : base(method, requestUri)
        {
        }

        public SdmxHttpRequestMessage(HttpMethod method, string requestUri) : base(method, requestUri)
        {
        }

        public SdmxEndPointCostant.RequestType RequestType { get; set; }
        public bool IncludeCrossReference { get; set; }
        public bool XmlNeedFix { get; set; }
    }
}