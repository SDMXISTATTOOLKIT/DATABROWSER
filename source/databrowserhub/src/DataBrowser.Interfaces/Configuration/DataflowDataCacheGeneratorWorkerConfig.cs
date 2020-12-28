using System;
using System.Collections.Generic;

namespace DataBrowser.Interfaces.Configuration
{
    public class DataflowDataCacheGeneratorWorkerConfig : IWorkerConfig
    {
        public bool IsEnable { get; set; }
        public DateTime StartTime { get; set; }
        public List<int> Days { get; set; }
        public List<Dataflowsrefresh> DataflowsRefresh { get; set; }

        public class Dataflowsrefresh
        {
            public string Id { get; set; }
            public List<string> Dimensions { get; set; }
            public string NodeCode { get; set; }
            public int ObservationMax { get; set; }
            public GruopByNumberObj GruopByNumber { get; set; }
            public List<string> AloneIds { get; set; }

            public class GruopByNumberObj
            {
                public int GroupSize { get; set; }
                public List<string> ExceptionValues { get; set; }
            }
        }
    }
}