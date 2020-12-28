using System.Collections.Generic;

namespace DataBrowser.Interfaces.Configuration
{
    public class MemoryCacheConfig
    {
        public Dictionary<string, string> ExpirationKeys { get; set; }
    }
}