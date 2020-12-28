using DataBrowser.DB.EFCore.Context;
using DataBrowser.Domain.Entities.Versioning;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Updater;
using DataBrowser.Updater.Actions;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DataBrowser.Updater
{
    public class DataBrowserUpdater : IUpdater
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DataBrowserUpdaterContext _dataBrowserUpdaterContext;
        private readonly ILogger<DataBrowserUpdater> _logger;

        public DataBrowserUpdater(IServiceProvider serviceProvider,
                                DataBrowserUpdaterContext dataBrowserUpdaterContext,
                                ILogger<DataBrowserUpdater> logger)
        {
            _serviceProvider = serviceProvider;
            _dataBrowserUpdaterContext = dataBrowserUpdaterContext;
            _logger = logger;
        }

        public async Task ExecutionBeforeDatabaseUpgrade()
        {
            _logger.LogDebug("START ExecutionBeforeDatabaseUpgrade");

            await commonInvokeMethods("ExecuteBeforeDatabaseAsync");

            //This call is only for fix database data insert before versioning system (v.1.0 or before)
            await fixOldJsonViewTemplateWithoutDbVersion();

            _logger.LogDebug("END ExecutionBeforeDatabaseUpgrade");
        }

        public async Task ExecutionAfterDatabaseUpgrade()
        {
            _logger.LogDebug("START ExecutionAfterDatabaseUpgrade");

            await commonInvokeMethods("ExecuteAfterDatabaseAsync");

            await setCurrentVersionDbAsync();

            _logger.LogDebug("END ExecutionAfterDatabaseUpgrade");
        }

        private async Task setCurrentVersionDbAsync()
        {
            _logger.LogDebug("START setCurrentVersionDbAsync");

            var isCurrentVersion = await _dataBrowserUpdaterContext.DataBrowserVersion.FirstOrDefaultAsync(i => i.Major == VersionDataBrowser.Current.Major &&
                                                                                i.Minor == VersionDataBrowser.Current.Minor &&
                                                                                i.Build == VersionDataBrowser.Current.Build &&
                                                                                i.Revision == VersionDataBrowser.Current.Revision);
            if (isCurrentVersion != null && isCurrentVersion.IsCurrentVersion)
            {
                _logger.LogDebug("END with version is current");
                return;
            }
            else if (isCurrentVersion != null && !isCurrentVersion.IsCurrentVersion)
            {
                var allVersionsToUpd = await _dataBrowserUpdaterContext.DataBrowserVersion.ToListAsync();

                allVersionsToUpd.ForEach(i => i.IsCurrentVersion = false);

                isCurrentVersion.IsCurrentVersion = true;
                isCurrentVersion.From = DateTime.UtcNow;
                _dataBrowserUpdaterContext.Update(isCurrentVersion);
                await _dataBrowserUpdaterContext.SaveChangesAsync();

                _logger.LogDebug("END with version present and change in true");
                return;
            }

            var allVersions = await _dataBrowserUpdaterContext.DataBrowserVersion.ToListAsync();
            allVersions.ForEach(i => i.IsCurrentVersion = false);

            var item = new DataBrowserVersion
            {
                Major = VersionDataBrowser.Current.Major,
                Minor = VersionDataBrowser.Current.Minor,
                Build = VersionDataBrowser.Current.Build,
                Revision = VersionDataBrowser.Current.Revision,
                From = DateTime.UtcNow,
                IsCurrentVersion = true
            };
            _dataBrowserUpdaterContext.DataBrowserVersion.Add(item);

            await _dataBrowserUpdaterContext.SaveChangesAsync();

            _logger.LogDebug("END with new version created");
        }

        private async Task commonInvokeMethods(string methodName)
        {
            var versionDb = _dataBrowserUpdaterContext.DataBrowserVersion.Where(i => i.IsCurrentVersion).FirstOrDefault();

            if (versionDb == null)
            {
                return;
            }

            var type = typeof(IActionUpdater);
            var assemblies = new Assembly[] { typeof(DataBrowserUpdater).GetTypeInfo().Assembly };
            var types = assemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p))
                .OrderBy(i=> ((IActionUpdater)Activator.CreateInstance(i)).UpgraderVersion);

            foreach (var item in types)
            {
                if (!item.IsClass) continue;

                ConstructorInfo callUpdeiterConstructor = item.GetConstructor(Type.EmptyTypes);
                object classObject = callUpdeiterConstructor.Invoke(new object[] { });
                MethodInfo updeiterMethod = item.GetMethod(methodName);

                var versionAllowed = false;
                var valueFromInterface = item.GetProperty("UpgraderVersion").GetValue(classObject);
                if (valueFromInterface != null)
                {
                    versionAllowed = UpgraderUtility.CanUpgrade(new Version(versionDb.Major, versionDb.Minor, versionDb.Build, versionDb.Revision),
                                                                VersionDataBrowser.Current,
                                                                (Version)valueFromInterface);
                }

                if (!versionAllowed)
                    continue;

                var result = (Task)updeiterMethod.Invoke(classObject, new object[] { new Version(versionDb.Major, versionDb.Minor, versionDb.Build, versionDb.Revision),
                                                                                     VersionDataBrowser.Current,
                                                                                    _serviceProvider });
                await result;
            }
        }

        private async Task fixOldJsonViewTemplateWithoutDbVersion()
        {
            var haveJsonFix = await _dataBrowserUpdaterContext.DataBrowserVersionActionUpgraders.Where(i => i.Name == "Updater_1_1_0_0.viewTemplateConvertJsonCriteriaAsync").FirstOrDefaultAsync();
            if (haveJsonFix == null)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                    var classUpd = new Updater_1_1_0_0();
                    await classUpd.viewTemplateConvertJsonCriteriaAsync(scope, loggerFactory, _dataBrowserUpdaterContext);
                }
            }
        }

    }
}
