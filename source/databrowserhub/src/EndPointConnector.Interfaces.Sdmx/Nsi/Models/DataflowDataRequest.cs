using System.Collections.Generic;
using System.Net.Http;

namespace EndPointConnector.Interfaces.Sdmx.Nsi.Models
{
    public class DataflowDataRequest
    {
        public HttpMethod HttpMethod { get; set; }
        public string QueryString { get; set; }
        public Dictionary<string, string> Keys { get; set; }
        public string ContentType { get; set; }
    }
}