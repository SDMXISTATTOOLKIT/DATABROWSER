using System.Collections.Generic;
using System.Threading.Tasks;

namespace EndPointConnector.Interfaces
{
    public interface IEndPointCache
    {
        Task ClearCacheAsync(List<string> nodesCode);
    }
}