using System.Threading.Tasks;
using System.Xml;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;

namespace EndPointConnector.Interfaces.Sdmx
{
    public interface ISdmxCache : IEndPointCache
    {
        int ExpiredTime { get; }
        bool DisableSdmxCache { get; }
        bool DisableGlobalCache { get; }
        bool DisableNamespace { get; }

        Task SetSdmxObjectsAsync(string key, ISdmxObjects sdmxObjects);
        Task<ISdmxObjects> GetSdmxObjectsAsync(string key);

        Task SetSdmxXmlAsync(string key, XmlDocument sdmxXml);
        Task<XmlDocument> GetSdmxXmlAsync(string key);

        Task SetJsonAsync(string key, object obj);
        Task<T> GetJsonAsync<T>(string key);

        Task<bool> InvalidSdmxDataAsync(bool onlyCurrentNode = false, bool onlyCurrentUser = false);

        /// <summary>
        ///     Get data from cache
        /// </summary>
        /// <param name="key">
        ///     The key value
        /// </param>
        /// <param name="crossDomain">
        ///     false in case the key is valid only for current node
        /// </param>
        /// <returns>
        ///     The value of key or null in case of crossDomain is false but the current NodeCode is null
        /// </returns>
        Task<string> GetGlobalGenericAsync(string key, bool crossDomain = false);

        /// <summary>
        ///     Set data in cache
        /// </summary>
        /// <param name="key">
        ///     The key value
        /// </param>
        /// <param name="value">
        ///     The value associated at key
        /// </param>
        /// <param name="crossDomain">
        ///     false in case the key is valid only for current node
        /// </param>
        /// <returns>
        ///     False in case of crossDomain is false but the current NodeCode is null
        /// </returns>
        Task<bool> SetGlobalGenericAsync(string key, string value, bool crossDomain = false);

        /// <summary>
        ///     Reset all cache data
        /// </summary>
        Task<bool> InvalidGlobalGenericAsync(bool crossDomain = false);

        string CreateKey(params string[] elements);
    }
}