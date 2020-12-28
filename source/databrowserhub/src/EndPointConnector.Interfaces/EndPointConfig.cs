using System.Collections.Generic;
using DataBrowser.DomainServices.Interfaces;
using DataBrowser.Interfaces;

namespace EndPointConnector.Interfaces
{
    public class EndPointConfig : INodeConfiguration
    {
        public EndPointConfig(INodeConfiguration node, IRequestContext requestContext)
        {
            NodeId = node.NodeId;
            Code = node.Code;
            EndPoint = node.EndPoint;
            Type = node.Type;
            EnableHttpAuth = node.EnableHttpAuth;
            HttpAuthUsername = node.HttpAuthUsername;
            HttpAuthPassword = node.HttpAuthPassword;
            EnableProxy = node.EnableProxy;
            UseProxySystem = node.UseProxySystem;
            ProxyAddress = node.ProxyAddress;
            ProxyPort = node.ProxyPort;
            ProxyUsername = node.ProxyUsername;
            ProxyPassword = node.ProxyPassword;
            Extras = node.Extras;
            ShowDataflowUncategorized = node.ShowDataflowUncategorized;
            LabelDimensionTerritorial = node.LabelDimensionTerritorial;
            EndPointFormatSupported = node.EndPointFormatSupported;
            EmptyCellDefaultValue = node.EmptyCellDefaultValue;
            MaxObservationsAfterCriteria = node.MaxObservationsAfterCriteria;
            ShowDataflowNotInProduction = node.ShowDataflowNotInProduction;
            CategorySchemaExcludes = node.CategorySchemaExcludes;

            //EndPointResponseLogForDebug = requestContext.CategorySchemaExcludes;
            UserGuid = requestContext.UserGuid;
            UserOperationGuid = requestContext.UserOperationGuid;
            UserLang = requestContext.UserLang;
            UserId = requestContext.LoggedUserId;
        }

        public bool EndPointResponseLogForDebug { get; set; }
        public string UserGuid { get; set; }
        public string UserOperationGuid { get; set; }
        public string UserLang { get; set; }
        public int UserId { get; set; }
        public int NodeId { get; set; }
        public string Code { get; set; }
        public string EndPoint { get; set; }
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
        public bool ShowDataflowUncategorized { get; set; }
        public string CriteriaSelectionMode { get; set; }
        public string LabelDimensionTerritorial { get; set; }
        public string LabelDimensionTemporal { get; set; }
        public List<string> CategorySchemaExcludes { get; set; }
        public string EndPointFormatSupported { get; set; }
        public string EmptyCellDefaultValue { get; set; }
        public int MaxObservationsAfterCriteria { get; set; }
        public bool ShowDataflowNotInProduction { get; set; }
    }
}