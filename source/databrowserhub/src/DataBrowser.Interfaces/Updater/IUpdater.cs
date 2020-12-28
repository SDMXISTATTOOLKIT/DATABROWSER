using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataBrowser.Interfaces.Updater
{
    public interface IUpdater
    {
        Task ExecutionBeforeDatabaseUpgrade();
        Task ExecutionAfterDatabaseUpgrade();
    }
}
