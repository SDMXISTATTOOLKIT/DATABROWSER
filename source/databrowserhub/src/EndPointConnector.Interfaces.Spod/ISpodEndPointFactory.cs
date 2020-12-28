using System.Threading.Tasks;
using DataBrowser.DomainServices.Interfaces;

namespace EndPointConnector.Interfaces.Spod
{
    public interface ISpodEndPointFactory
    {
        /// <summary>
        ///     Create an EndPoint connetor for spod
        /// </summary>
        Task<IEndPointConnector> CreateConnector(INodeConfiguration endPointConfig,
            SpodEndPointCostant.ConnectorType endPointType);
    }
}