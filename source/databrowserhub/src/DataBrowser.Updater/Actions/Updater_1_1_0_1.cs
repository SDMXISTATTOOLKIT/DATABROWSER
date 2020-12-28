using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataBrowser.Updater.Actions
{
    public class Updater_1_1_0_1 : IActionUpdater
    {
        public Version UpgraderVersion => new Version(1, 1, 0, 1);

        public Task ExecuteAfterDatabaseAsync(Version dataBrowserCurrentVerions, Version upgradeToVersion, IServiceProvider serviceProvider)
        {
            if (!UpgraderUtility.CanUpgrade(dataBrowserCurrentVerions, upgradeToVersion, UpgraderVersion))
            {
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }

        public Task ExecuteBeforeDatabaseAsync(Version dataBrowserCurrentVerions, Version upgradeToVersion, IServiceProvider serviceProvider)
        {
            if (!UpgraderUtility.CanUpgrade(dataBrowserCurrentVerions, upgradeToVersion, UpgraderVersion))
            {
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }

    }
}
