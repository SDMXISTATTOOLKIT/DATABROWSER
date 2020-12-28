using DataBrowser.DB.EFCore.Context;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Domain.Serialization;
using EndPointConnector.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataBrowser.Updater.Actions
{
    public class Updater_1_1_0_0 : IActionUpdater
    {
        public Version UpgraderVersion => new Version(1, 1, 0, 0);

        public Task ExecuteAfterDatabaseAsync(Version dataBrowserCurrentVerions, Version upgradeToVersion, IServiceProvider serviceProvider)
        {
            if (!UpgraderUtility.CanUpgrade(dataBrowserCurrentVerions, upgradeToVersion, UpgraderVersion))
            {
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }

        public async Task ExecuteBeforeDatabaseAsync(Version dataBrowserCurrentVerions, Version upgradeToVersion, IServiceProvider serviceProvider)
        {
            if (!UpgraderUtility.CanUpgrade(dataBrowserCurrentVerions, upgradeToVersion, UpgraderVersion))
            {
                return;
            }
            using (var scope = serviceProvider.CreateScope())
            {
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var dataBrowserUpdaterContext = scope.ServiceProvider.GetRequiredService<DataBrowserUpdaterContext>();

                await viewTemplateConvertJsonCriteriaAsync(scope, loggerFactory, dataBrowserUpdaterContext);
            }
        }

        public async Task viewTemplateConvertJsonCriteriaAsync(IServiceScope scope, ILoggerFactory loggerFactory, DataBrowserUpdaterContext dataBrowserUpdaterContext)
        {
            var logger = loggerFactory.CreateLogger("viewTemplateConvertJsonCriteriaAsync");
            logger.LogDebug("START viewTemplateConvertJsonCriteriaAsync");

            try
            {
                var repository = scope.ServiceProvider.GetRequiredService<IRepository<ViewTemplate>>();

                var viewTemplates = await repository.ListAllAsync();

                foreach (var item in viewTemplates)
                {
                    if (item.Criteria == null) continue;

                    logger.LogDebug($"Process viewtemplate id {item.ViewTemplateId}");
                    try
                    {
                        var oldJsonCriteria = DataBrowserJsonSerializer.DeserializeObject<Dictionary<string, List<string>>>(item.Criteria);
                        var newStructureObject = new List<FilterCriteria>();
                        foreach (var itemCriteria in oldJsonCriteria)
                        {

                            if (!itemCriteria.Key.Equals("TIME_PERIOD", StringComparison.InvariantCultureIgnoreCase))
                            {
                                newStructureObject.Add(new FilterCriteria { Id = itemCriteria.Key, FilterValues = itemCriteria.Value, Type = FilterType.CodeValues });
                                continue;
                            }

                            DateTime startDate = DateTime.Now;
                            var haveStartDate = true;
                            if (itemCriteria.Value.Count > 0)
                            {
                                haveStartDate = DateTime.TryParse(itemCriteria.Value[0], out startDate);
                            }
                            DateTime endDate = DateTime.Now;
                            var haveEndDate = true;
                            if (itemCriteria.Value.Count > 1)
                            {
                                haveEndDate = DateTime.TryParse(itemCriteria.Value[1], out endDate);
                            }

                            newStructureObject.Add(new FilterCriteria
                            {
                                Id = itemCriteria.Key,
                                From = haveStartDate ? startDate : (DateTime?)null,
                                To = haveEndDate ? endDate : (DateTime?)null,
                                Type = FilterType.TimeRange
                            });
                        }

                        item.Criteria = DataBrowserJsonSerializer.SerializeObject(newStructureObject);

                        repository.Update(item);

                        logger.LogDebug($"Parse complete for viewtemplate id {item.ViewTemplateId}");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Some error in parsing viewTemplate Id {item.ViewTemplateId} \t Error: {ex.Message}", ex);
                    }
                }

                logger.LogDebug("call save change");
                await repository.UnitOfWork.SaveChangesAsync(dispatchDomainEvent: false);
            }
            catch (Exception ex)
            {
                logger.LogError($"Some error in viewTemplateConvertJsonCriteriaAsync: {ex.Message}", ex);
            }
            logger.LogDebug("END viewTemplateConvertJsonCriteriaAsync");

            await UpgraderUtility.RegisterActionAsync(dataBrowserUpdaterContext, "Updater_1_1_0_0.viewTemplateConvertJsonCriteriaAsync", true, "");
        }

    }
}
