using System.Threading.Tasks;

namespace DataBrowser.Interfaces.Workers
{
    public interface IDashboardDataCacheGenerator
    {
        Task RefreshAllViewStaticCrossNodeAsync();
    }
}