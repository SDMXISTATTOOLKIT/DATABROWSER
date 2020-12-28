using System;
using System.Collections.Generic;
using EndPointConnector.Models;

namespace DataBrowser.Interfaces.Dto
{
    public class DataflowDataCacheFile
    {
        public Guid CacheFileId { get; set; }
        public Guid CacheInfoId { get; set; }
        public List<FilterCriteria> Filter { get; set; }
        public string Language { get; set; }
        public string Annotations { get; set; }
        public DataType DataType { get; set; }
        public string Path { get; set; }
        public DateTime CreationDate { get; set; }
        public int FileSize { get; set; }
        public long Accesses { get; set; }
        public int Ttl { get; set; }
        public bool IsValid { get; set; }
    }

    public enum DataType
    {
        AllData,
        PartialData,
        UnKnow,
        NoData
    }
}