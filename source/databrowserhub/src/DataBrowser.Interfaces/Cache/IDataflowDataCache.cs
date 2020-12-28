using DataBrowser.Interfaces.Dto;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using EndPointConnector.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataBrowser.Interfaces.Cache
{
    public interface IDataflowDataCache : IClearCache
    {
        Task<GetDataFromDataflowResponse> GetJsonStatForDataflowDataFromValidKey(
            DataFromDataflowRequest dataFromDataflowRequest, Dataset dataset);

        Task<GetDataFromDataflowResponse> GetJsonStatForDataflowDataFromValidKeyCompatible(
            DataFromDataflowRequest dataFromDataflowRequest, Dataset dataset);

        Task<DataflowDataCacheFile> GetOnlyCachedKeyInfoDataflowData(string dataflowId,
            List<FilterCriteria> dataCriterias, bool includeExpired, Dataset dataset);

        Task<DataflowDataCacheFile> GetOnlyCachedKeyInfoDataflowDataIfIsValid(string dataflowId,
            List<FilterCriteria> dataCriterias, Dataset dataset);

        Task SetJsonStatForDataflowData(DataFromDataflowRequest dataFromDataflowRequest,
            GetDataFromDataflowResponse cacheValue, Dataset dataset);

        Task ClearCacheDataflowDataAsync(int nodeId);
        Task ClearSingleDataflowCache(string dataflowId, int nodeId);
        Task ClearSingleItemCache(Guid cacheInfoId, int nodeId);
        Task<List<DataflowDataCacheInfo>> GetInfoFromNodeId(int nodeId);
        Task<bool> UpdateDataflowTTLFromNodeId(Guid id, int ttl);
        Task<DataflowDataCacheInfo> CreateDataflowDataCacheInfo(DataflowDataCacheInfo dataflowDataCacheInfo);
        Task<List<DataflowDataCacheExport>> ExportAccessNumber(int nodeId);

        Task InvalidateKeyForCurrentNodeAndLanguagesHAVEBUG(int nodeId, string dataflowId,
            List<FilterCriteria> dataCriterias);

        Task InvalidateAllKeysForCurrentNodeAndLanguages(string dataflowId);
        Task<List<DataflowDataCacheFile>> GetValidCacheFileIdForDataflowId(string dataflowId);
        bool DropDatabase(int nodeId);
    }
}