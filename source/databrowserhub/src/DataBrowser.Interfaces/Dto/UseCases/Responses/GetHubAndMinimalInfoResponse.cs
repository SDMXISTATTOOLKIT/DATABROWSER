using System.Collections.Generic;
using DataBrowser.AC.Responses.Services;
using DataBrowser.Domain.Dtos;

namespace DataBrowser.Interfaces.Dto.UseCases.Responses
{
    public class GetHubAndMinimalInfoResponse
    {
        public HubDto Hub { get; set; }
        public List<Dashboard> HubDashboards { get; set; }
        public IReadOnlyList<NodeMinimalInfoDto> Nodes { get; set; }

        public class Dashboard
        {
            public int Id { get; set; }
            public Dictionary<string, string> Titles { get; set; }
        }
    }
}