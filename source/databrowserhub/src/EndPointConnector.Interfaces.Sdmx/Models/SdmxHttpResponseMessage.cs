using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Xml;
using log4net;

namespace EndPointConnector.Interfaces.Sdmx.Models
{
    public class SdmxHttpResponseMessage : IDisposable
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SdmxHttpResponseMessage));

        public XmlDocument XmlResponse { get; set; }
        public string TextResponse { get; set; }
        public string FileResponse { get; set; }
        public string ResponseContentType { get; set; }
        public ContentRangeHeaderValue ResponseContentRange { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool NoResultFound { get; set; }
        public bool NsiErrorResponse { get; set; }
        public bool ResponseDataTypeNotAcceptable { get; set; }
        public long ResponseTime { get; set; }
        public long DownloadResponseTime { get; set; }
        public long DownloadResponseSize { get; set; }
        public long ObjectCompletedTime { get; set; }
        public SdmxEndPointCostant.RequestType ResponseType { get; set; }

        public void Dispose()
        {
            if (!string.IsNullOrWhiteSpace(FileResponse))
                try
                {
                    File.Delete(FileResponse);
                }
                catch (Exception ex)
                {
                    _logger.Warn($"Unable to remove file {FileResponse}\t Error:{ex.Message}", ex);
                }
        }
    }
}