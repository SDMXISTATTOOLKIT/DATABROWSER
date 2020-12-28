namespace EndPointConnector.Interfaces.Sdmx.Models
{
    public class SdmxCacheConfig
    {
        public string ConnectionString { get; set; }
        public int ExpiredTime { get; set; }
        public bool DisableSdmxCache { get; set; }
        public bool DisableGlobalCache { get; set; }
        public bool DisableNamespace { get; set; }
    }
}