using System.Collections.Generic;

namespace DataBrowser.Interfaces.Configuration
{
    public class GeneralConfig
    {
        public CORSConfig CORS { get; set; }
        public ValidationRulesConfig ValidationRules { get; set; }
        public bool EndPointResponseLogForDebug { get; set; }
        public string InternalRestUrl { get; set; }
        public string ExternalRestUrl { get; set; }
        public string ExternalClientUrl { get; set; }
    }

    public class CORSConfig
    {
        public bool Enable { get; set; }
    }

    public class ValidationRulesConfig
    {
        public string[] View { get; set; }
        public string[] Template { get; set; }
        public string[] Node { get; set; }
    }


}