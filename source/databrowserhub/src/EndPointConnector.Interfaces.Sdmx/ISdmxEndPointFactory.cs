using System.Threading.Tasks;

namespace EndPointConnector.Interfaces.Sdmx
{
    public interface ISdmxEndPointFactory
    {
        /// <summary>
        ///     Create an EndPoint connector
        /// </summary>
        Task<IEndPointConnector> CreateConnector(EndPointConfig endPointConfig);
    }
}