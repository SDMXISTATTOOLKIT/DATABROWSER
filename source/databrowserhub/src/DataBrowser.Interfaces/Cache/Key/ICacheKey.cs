namespace DataBrowser.Interfaces.Cache.Key
{
    public interface ICacheKey<TItem>
    {
        string CacheKey { get; }
    }
}