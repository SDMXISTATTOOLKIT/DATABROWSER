
export interface IDataflowCache {
    id: string,
    nodeId: number,
    dataflowId: string,
    ttl: number,
    cacheSize: number,
    cachedDataflow: number,
    cachedDataAccess: number,
}