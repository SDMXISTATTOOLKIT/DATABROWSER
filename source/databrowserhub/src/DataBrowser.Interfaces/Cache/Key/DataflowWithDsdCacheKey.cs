using EndPointConnector.Models;

namespace DataBrowser.Interfaces.Cache.Key
{
    public class DataflowWithDsdCacheKey : ICacheKey<ArtefactContainer>
    {
        public static string KeyName = "DataflowWithDsd";
        private readonly string _lang;
        private readonly int _nodeId;
        private readonly int _userId;

        public DataflowWithDsdCacheKey(int userId, int nodeId, string lang)
        {
            _userId = userId;
            _nodeId = nodeId;
            _lang = lang.ToLowerInvariant();
        }

        public DataflowWithDsdCacheKey(int nodeId)
        {
            _userId = -1;
            _nodeId = nodeId;
            _lang = ConstraintKey.AllLanguages;
        }

        public string CacheKey => $"{KeyName}:Node{_nodeId}:Lang{_lang}:User{_userId}";
    }
}