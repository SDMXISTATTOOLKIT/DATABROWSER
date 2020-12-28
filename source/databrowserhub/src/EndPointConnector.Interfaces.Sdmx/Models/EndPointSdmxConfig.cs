using System;
using System.Collections.Generic;

namespace EndPointConnector.Interfaces.Sdmx.Models
{
    public class EndPointSdmxConfig
    {
        private readonly List<string> avaiablesLanguage = new List<string> {"en", "it"};

        public EndPointSdmxConfig()
        {
        }

        public EndPointSdmxConfig(EndPointConfig endPointConfig)
        {
            NodeId = endPointConfig.NodeId;
            Code = endPointConfig.Code;
            EndPointUrl = endPointConfig.EndPoint;
            Type = endPointConfig.Type;
            EnableHttpAuth = endPointConfig.EnableHttpAuth;
            HttpAuthUsername = endPointConfig.HttpAuthUsername;
            HttpAuthPassword = endPointConfig.HttpAuthPassword;
            EnableProxy = endPointConfig.EnableProxy;
            UseProxySystem = endPointConfig.UseProxySystem;
            ProxyAddress = endPointConfig.ProxyAddress;
            ProxyPort = endPointConfig.ProxyPort;
            ProxyUsername = endPointConfig.ProxyUsername;
            ProxyPassword = endPointConfig.ProxyPassword;
            Extras = endPointConfig.Extras;

            CategorySchemaExcludes = endPointConfig.CategorySchemaExcludes;
            MaxObservationsAfterCriteria = endPointConfig.MaxObservationsAfterCriteria;
            CategorySchemaExcludes = endPointConfig.CategorySchemaExcludes;
            ShowDataflowUncategorized = endPointConfig.ShowDataflowUncategorized;
            LabelDimensionTerritorial = endPointConfig.LabelDimensionTerritorial;
            LabelDimensionTemporal = endPointConfig.LabelDimensionTemporal;
            ShowDataflowNotInProduction = endPointConfig.ShowDataflowNotInProduction;

            EndPointResponseLogForDebug = endPointConfig.EndPointResponseLogForDebug;
            UserGuid = endPointConfig.UserGuid;
            UserOperationGuid = endPointConfig.UserOperationGuid;
            UserLang = endPointConfig.UserLang;
            UserId = endPointConfig.UserId;
        }


        public SdmxEndPointCostant.ConnectorType EndPointType { get; set; }
        public EndPointAppConfig EndPointAppConfig { get; set; }

        public bool OptimizeCallWithSoap { get; set; }

        //From Config with Extra
        public string EndPointV20 { get; set; }
        public string EndPointV21 { get; set; }
        public string Prefix { get; set; }
        public string InitialWSDL { get; set; }
        public string InitialWSDLV20 { get; set; }
        public string Namespace { get; set; }
        public string NamespaceV20 { get; set; }
        public bool SupportAllCompleteStubs { get; set; }
        public bool XmlResultNeedFix { get; set; }
        public bool ShowDataflowUncategorized { get; set; }
        public bool RestDataResponseXml { get; set; }
        public string LabelDimensionTerritorial { get; set; }
        public string LabelDimensionTemporal { get; set; }
        public bool SupportPostFilters { get; set; }
        public bool CallDataflowWithoutPartial { get; set; }
        public bool EnableEndPointV20 { get; set; }
        

        public string AcceptedLanguages
        {
            get
            {
                var allItems = UserLang.ToLowerInvariant();
                foreach (var item in avaiablesLanguage)
                {
                    if (UserLang.Equals(item, StringComparison.InvariantCultureIgnoreCase)) continue;
                    allItems += $",{item.ToLowerInvariant()}";
                }

                return allItems;
            }
        }

        //From Config
        public int NodeId { get; set; }
        public string Code { get; set; }
        public string EndPointUrl { get; set; }
        public string Type { get; set; }
        public bool EnableHttpAuth { get; set; }
        public string HttpAuthUsername { get; set; }
        public string HttpAuthPassword { get; set; }
        public bool EnableProxy { get; set; }
        public bool UseProxySystem { get; set; }
        public string ProxyAddress { get; set; }
        public int ProxyPort { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
        public Dictionary<string, object> Extras { get; set; }
        public List<string> CategorySchemaExcludes { get; set; }
        public int MaxObservationsAfterCriteria { get; set; }
        public bool ShowDataflowNotInProduction { get; set; }
        public EndPointCustomAnnotationConfig AnnotationConfig { get; set; }
        public bool EndPointResponseLogForDebug { get; set; }
        public string UserGuid { get; set; }
        public string UserOperationGuid { get; set; }
        public string UserLang { get; set; }
        public int UserId { get; set; }
    }
}