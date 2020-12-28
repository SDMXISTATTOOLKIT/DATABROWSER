using System;
using System.Threading.Tasks;
using DataBrowser.DomainServices.Interfaces;
using EndPointConnector.Interfaces;
using EndPointConnector.Interfaces.Spod;

namespace Sister.EndPointConnector.Spod
{
    public class SpodEndPointManager : ISpodEndPointFactory
    {
        public Task<IEndPointConnector> CreateConnector(INodeConfiguration endPointConfig,
            SpodEndPointCostant.ConnectorType endPointType)
        {
            throw new NotImplementedException();
        }
    }
}