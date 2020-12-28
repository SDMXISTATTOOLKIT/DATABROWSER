using DataBrowser.Interfaces.Cache;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataBrowser.AC.Caches
{
    public class DataBrowserCachesService : IDataBrowserCachesService
    {
        readonly IDataBrowserMemoryCache _dataBrowserMemoryCache;
        readonly IDataflowDataCache _dataflowDataCache;

        public DataBrowserCachesService(IDataBrowserMemoryCache dataBrowserMemoryCache,
                                        IDataflowDataCache dataflowDataCache)
        {
            _dataBrowserMemoryCache = dataBrowserMemoryCache;
            _dataflowDataCache = dataflowDataCache;
        }


        public async Task ClearNodeCacheAsync(int nodeId)
        {
            if (_dataBrowserMemoryCache != null)
            {
                await _dataBrowserMemoryCache.ClearNodeCacheAsync(nodeId);
            }

            if (_dataflowDataCache != null)
            {
                await _dataflowDataCache.ClearNodeCacheAsync(nodeId);
            }
        }
    }
}
