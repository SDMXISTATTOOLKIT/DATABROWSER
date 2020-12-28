using System.Collections.Generic;

namespace DataBrowser.Interfaces.Configuration
{
    public class DataflowDataCacheConfig
    {
        public string Type { get; set; }
        public string ConnectionString { get; set; }
        public bool IsEnable { get; set; }
        public bool SaveDataOnFile { get; set; }
        public string SavedDataFilePath { get; set; }
        public int MaxSize { get; set; }
        public int Expiration { get; set; }
        public List<string> ExclusionList { get; set; }
    }
}