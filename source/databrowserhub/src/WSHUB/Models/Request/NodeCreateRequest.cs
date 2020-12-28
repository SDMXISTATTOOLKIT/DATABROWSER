using DataBrowser.Domain.Dtos;

namespace WSHUB.Models.Request
{
    public class NodeCreateRequest : NodeDto
    {
        public NodeCreateRequest()
        {
            Active = true;
        }
    }
}