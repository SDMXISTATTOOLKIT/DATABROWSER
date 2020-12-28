using System.Threading.Tasks;
using DataBrowser.DomainServices.Interfaces;
using DataBrowser.Interfaces;

namespace EndPointConnector.Interfaces
{
    public interface IEndPointConnectorFactory
    {
        Task<IEndPointConnector> Create(INodeConfiguration endPointConfig, IRequestContext requestContext);
    }
}