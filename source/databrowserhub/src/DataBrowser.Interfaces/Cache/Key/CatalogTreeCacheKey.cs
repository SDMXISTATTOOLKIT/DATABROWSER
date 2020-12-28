namespace DataBrowser.Interfaces.Cache.Key
{
    public class CatalogTreeCacheKey : ICacheKey<string>
    {
        public static string KeyName = "CatalogTree";
        private readonly string _lang;
        private readonly int _nodeId;
        private readonly int _userId;

        public CatalogTreeCacheKey(int userId, int nodeId, string lang)
        {
            _userId = userId;
            _nodeId = nodeId;
            _lang = lang.ToLowerInvariant();
        }

        public string CacheKey => $"{KeyName}:Node{_nodeId}:Lang{_lang}:User{_userId}";
    }
}