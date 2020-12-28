using System;
using System.Collections.Generic;

namespace DataBrowser.Interfaces.Configuration
{
    public class DashboardDataCacheGeneratorWorkerConfig : IWorkerConfig
    {
        public bool IsEnable { get; set; }
        public DateTime StartTime { get; set; }
        public List<int> Days { get; set; }
    }
}