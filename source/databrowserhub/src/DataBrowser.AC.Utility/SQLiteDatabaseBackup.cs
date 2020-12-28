using System.Threading.Tasks;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Database;
using Microsoft.Extensions.Options; //using Microsoft.Data.Sqlite;

namespace DataBrowser.AC.Utility
{
    public class SQLiteDatabaseBackup : IDatabaseBackup
    {
        private readonly DatabaseConfig _optionsSnapshot;

        public SQLiteDatabaseBackup(IOptionsSnapshot<DatabaseConfig> optionsSnapshot)
        {
            _optionsSnapshot = optionsSnapshot.Value;
        }

        public async Task BackupAsync()
        {
            //using (var location = new SqliteConnection(@"Data Source=C:\activeDb.db; Version=3;"))
            //using (var destination = new SqliteConnection(string.Format(@"Data Source={0}:\backupDb.db; Version=3;", strDestination)))
            //{
            //    location.Open();
            //    destination.Open();
            //    location.BackupDatabase(destination);
            //}
        }
    }
}