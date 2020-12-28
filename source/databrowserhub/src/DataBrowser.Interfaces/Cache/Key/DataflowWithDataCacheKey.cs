using EndPointConnector.Models;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Interfaces.Cache.Key
{
    public class DataflowWithDataCacheKey : ICacheKey<ISdmxObjects>
    {
        public static string KeyName = "DataflowWithData";
        private readonly string _lang;
        private readonly int _nodeId;
        private readonly int _userId;
        private readonly string _dataflowId;

        public DataflowWithDataCacheKey(int nodeId, string dataflowId)
        {
            _userId = -1;
            _nodeId = nodeId;
            _lang = ConstraintKey.AllLanguages;
            _dataflowId = dataflowId;
        }

        public string CacheKey => $"{KeyName}:Node{_nodeId}:Lang{_lang}:User{_userId}:{_dataflowId}";
    }
}
