using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataBrowser.Updater.Actions
{
    public interface IActionUpdater
    {
        Version UpgraderVersion { get; }
        Task ExecuteBeforeDatabaseAsync(Version dataBrowserCurrentVerions, Version upgradeToVersion, IServiceProvider serviceProvider);
        Task ExecuteAfterDatabaseAsync(Version dataBrowserCurrentVerions, Version upgradeToVersion, IServiceProvider serviceProvider);
    }
}
