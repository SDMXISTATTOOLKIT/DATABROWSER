using System.Threading.Tasks;

namespace DataBrowser.Interfaces.Workers
{
    public interface IApplicationCleaner
    {
        Task DoWorkAsync();
    }
}