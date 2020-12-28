using DataBrowser.DB.EFCore.Context;
using DataBrowser.Domain.Entities.Versioning;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataBrowser.Updater.Actions
{
    static public class UpgraderUtility
    {
        static public bool CanUpgrade(Version dataBrowserCurrentVerions, Version upgradeToVersion, Version methodVersion)
        {
            if (methodVersion.CompareTo(dataBrowserCurrentVerions) > 0 &&
                methodVersion.CompareTo(upgradeToVersion) <= 0)
            {
                return true;
            }

            return false;
        }

        static public async Task RegisterActionAsync(DataBrowserUpdaterContext dataBrowserUpdaterContext, string upgraderName, bool success, string errors)
        {
            var item = new DataBrowserVersionActionUpgrader();
            item.ExecutionDate = DateTime.UtcNow;
            item.Name = upgraderName;
            item.Success = success;
            item.Errors = errors;
            dataBrowserUpdaterContext.DataBrowserVersionActionUpgraders.Add(item);

            await dataBrowserUpdaterContext.SaveChangesAsync();
        }
    }
}
