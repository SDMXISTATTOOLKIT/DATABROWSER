using System;

namespace TracertLOg
{
    public class ResponseTracing
    {
        public string RequestUrl { get; set; }
        public string RequestBody { get; set; }
        public string RequestOperation { get; set; }
        public string OperationName { get; set; }
        public DateTime LogDateTime { get; set; }
        public string OperationId { get; set; }
        public string UserGuid { get; set; }
        public long ResponseTime { get; set; }
    }
}