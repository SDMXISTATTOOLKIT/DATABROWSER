import {initRequest, RequestMethod} from "../../middlewares/request/requestActions";
import {
    getClearNodeCatalogMemoryCacheUrl,
    getDataflowsCacheListUrl,
    getDataflowsCacheUpdateUrl,
    getDataflowsClearAllCacheUrl,
    getDataflowsClearCacheUrl,
    getDataflowsCreateCacheUrl
} from "../../serverApi/urls";

export const DATAFLOW_CACHE_FETCH = "cache/fetch";
export const DATAFLOW_CACHE_CLEAR = "cache/clear";
export const DATAFLOW_CACHE_UPDATE = "cache/update";
export const DATAFLOW_CACHE_CREATE = "cache/create";
export const DATAFLOW_CACHE_DELETE = "cache/delete";
export const DATAFLOW_CACHE_DELETE_ALL = "cache/deleteAll";
export const DATAFLOW_CACHE_DELETE_CATALOG = "cache/deleteCatalog";


export const fetchDataflowCache = (nodeId: number) => initRequest(
  DATAFLOW_CACHE_FETCH,
  getDataflowsCacheListUrl(nodeId),
  undefined,
  undefined,
  t => ({
    onStart: t("scenes.nodesSettings.cacheSettings.messages.fetchDataflowCache.start")
  })
);

export const updateDataflowCache = (nodeId: number, cacheId: string, ttl: number) => initRequest(
  DATAFLOW_CACHE_UPDATE,
  getDataflowsCacheUpdateUrl(nodeId, cacheId),
  RequestMethod.PUT,
  {ttl: ttl},
  t => ({
      onStart: t("scenes.nodesSettings.cacheSettings.messages.updateDataflowCache.start")
  }),
  {
    ttl: ttl,
    nodeId: nodeId,
    cacheId: cacheId
  }
);

export const createDataflowCache = (nodeId: number, data:any) => initRequest(
    DATAFLOW_CACHE_CREATE,
    getDataflowsCreateCacheUrl(nodeId),
    RequestMethod.POST,
    data,
    t => ({
        onStart: t("scenes.nodesSettings.cacheSettings.messages.createDataflowCache.start")
    }),
    {
      oldData: data,
    }
);

export const deleteDataflowCache = (nodeId: number, cacheId: string) => initRequest(
  DATAFLOW_CACHE_DELETE,
  getDataflowsClearCacheUrl(nodeId, cacheId),
  RequestMethod.POST,
  {},
  t => ({
      onStart: t("scenes.nodesSettings.cacheSettings.messages.deleteDataflowCache.start")
  }),
  {
    nodeId: nodeId,
    cacheId: cacheId
  }
);

export const clearDataflowCache = () => ({
  type: DATAFLOW_CACHE_CLEAR
});

export const deleteAllDataflowCache = (nodeId: number) => initRequest(
  DATAFLOW_CACHE_DELETE_ALL,
  getDataflowsClearAllCacheUrl(nodeId),
  RequestMethod.POST,
  undefined,
  t => ({
      onStart: t("scenes.nodesSettings.cacheSettings.messages.deleteAllDataflowCache.start")
  }),
  {
    nodeId
  }
);

export const deleteCatalogCache = (nodeId: number) => initRequest(
  DATAFLOW_CACHE_DELETE_CATALOG,
  getClearNodeCatalogMemoryCacheUrl(nodeId),
  RequestMethod.POST,
  undefined,
  t => ({
      onStart: t("scenes.nodesSettings.cacheSettings.messages.deleteCatalogCache.start")
  }),
  {
    nodeId
  }
);
