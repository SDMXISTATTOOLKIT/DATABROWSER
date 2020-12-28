using System.Collections.Generic;
using System.Threading.Tasks;
using DataBrowser.Interfaces.Configuration;

namespace DataBrowser.Interfaces.Workers
{
    public interface IDataflowDataCacheGenerator
    {
        Task RefreshSingleDataflowAsync(string id, string nodeCode);
        Task RefreshAllDataflowAsync();
        Task RefreshAllDataflowCrossNodeAsync();
        Task RefreshAllDataflowInStaticDashboardCrossNodeAsync();
        List<DataflowDataCacheGeneratorWorkerConfig.Dataflowsrefresh> GetDataflowRefresh();
    }
}