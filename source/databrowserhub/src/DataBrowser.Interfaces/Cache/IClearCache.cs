using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataBrowser.Interfaces.Cache
{
    public interface IClearCache
    {
        Task ClearNodeCacheAsync(int nodeId);
    }
}
