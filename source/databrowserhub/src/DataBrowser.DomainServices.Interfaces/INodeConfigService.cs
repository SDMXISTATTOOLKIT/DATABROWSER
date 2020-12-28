using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataBrowser.DomainServices.Interfaces
{
    public interface INodeConfigService
    {
        Task<INodeConfiguration> GenerateNodeConfigAsync(string nodeCode);
        Task<INodeConfiguration> GenerateNodeConfigAsync(int nodeId);
    }
}
