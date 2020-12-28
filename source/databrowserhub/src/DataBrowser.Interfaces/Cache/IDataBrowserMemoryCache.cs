using DataBrowser.Interfaces.Cache.Key;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataBrowser.Interfaces.Cache
{
    public interface IDataBrowserMemoryCache : IClearCache
    {
        Task ClearCacheAsync(List<int> nodesId = null, List<string> applicationLangs = null, List<int> userIds = null,
            string key = null);

        Task AddAsync<TItem>(TItem item, ICacheKey<TItem> key);

        TItem Get<TItem>(ICacheKey<TItem> key) where TItem : class;

        Task GenerateDataflowDsdCodelistConceptschemeCacheAsync(IServiceProvider serviceProvider, IRequestContext requestContext, bool forceIfExist);
    }
}