using System.Threading.Tasks;

namespace DataBrowser.Interfaces.Database
{
    public interface IDatabaseBackup
    {
        Task BackupAsync();
    }
}