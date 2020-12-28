using DataBrowser.Interfaces.EndPoint;

namespace DataBrowser.Interfaces.Dto.UseCases.Responses
{
    public class DownloadDataflowFromNodeEndPointResponse
    {
        public GenericResponseData<string> Response { get; set; }
        public string ExtensionFile { get; set; }
        public bool IsCompressed { get; set; }
    }
}