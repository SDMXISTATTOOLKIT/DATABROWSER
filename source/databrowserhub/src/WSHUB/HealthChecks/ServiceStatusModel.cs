using System.Collections.Generic;

namespace WSHUB.HealthChecks
{
    public class ServiceStatusModel
    {
        public string Service { get; set; }
        public string Status { get; set; }
        public IEnumerable<KeyValuePair<string, object>> Data { get; set; }
    }
}