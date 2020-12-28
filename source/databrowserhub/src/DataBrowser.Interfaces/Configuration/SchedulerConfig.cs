using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Interfaces.Configuration
{
    public class SchedulerConfig
    {
        public bool IsEnable { get; set; }
        public TimeSpan Timer { get; set; }
    }
}
