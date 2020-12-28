using EndPointConnector.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Interfaces.Cache.Key
{
    public class DsdWithCodelistlistStubCacheKey : ICacheKey<ArtefactContainer>
    {
        public static string KeyName = "DsdWithCodelistlistStub";
        private readonly string _lang;
        private readonly int _nodeId;
        private readonly int _userId;
        private readonly string _dsdId;

        public DsdWithCodelistlistStubCacheKey(int nodeId, string dsdId)
        {
            _userId = -1;
            _nodeId = nodeId;
            _lang = ConstraintKey.AllLanguages;
            _dsdId = dsdId;
        }

        public string CacheKey => $"{KeyName}:Node{_nodeId}:Lang{_lang}:User{_userId}:{_dsdId}";
    }
}
