using System;

namespace DataBrowser.Interfaces.Dto
{
    public class DataflowDataCacheInfo
    {
        public Guid Id { get; set; }
        public int NodeId { get; set; }
        public string DataflowId { get; set; }
        public int TTL { get; set; }
        public long CacheSize { get; set; }
        public long CachedDataflow { get; set; }
        public long CachedDataAccess { get; set; }
        public string Title { get; set; }
    }
}