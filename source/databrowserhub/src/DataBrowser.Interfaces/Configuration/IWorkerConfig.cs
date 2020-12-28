using System;
using System.Collections.Generic;

namespace DataBrowser.Interfaces.Configuration
{
    public interface IWorkerConfig
    {
        bool IsEnable { get; set; }
        DateTime StartTime { get; set; }
        List<int> Days { get; set; }
    }
}