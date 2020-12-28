using DataBrowser.Domain.Serialization;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Cache.Key;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WSHUB.Models.Response;

namespace WSHUB.Utils
{
    public static class GetTreeHelper
    {
        public static async Task<string> GetCatalogTreeAsync(IMediatorService mediatorService,
            IDataBrowserMemoryCache dataBrowserMemoryCache,
            IRequestContext requestContext,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider)
        {
            var logger = loggerFactory.CreateLogger(typeof(GetTreeHelper).FullName);
            var inputHanlde = new NodeCatalogTreeRequest
            { OrderLang = requestContext.UserLang, FilterByPermissionUser = false };
            var userId = ConstraintKey.AllUsers;
            var catalogTreeCacheKey = new CatalogTreeCacheKey(userId, requestContext.NodeId, requestContext.UserLang);
            var nodeCatalogJson = requestContext.IgnoreCache ? null : dataBrowserMemoryCache.Get(catalogTreeCacheKey);
            if (nodeCatalogJson == null)
            {
                logger.LogDebug("Key cache not found for CatalogTreeCacheKey");

                if (!requestContext.IgnoreCache)
                {
                    logger.LogDebug("begin process for GenerateDataflowAndDsdCacheAsync");
                    dataBrowserMemoryCache.GenerateDataflowDsdCodelistConceptschemeCacheAsync(serviceProvider, requestContext, true);
                }

                logger.LogDebug("begin process to generate catalogtree");

                var useCaseResult = await mediatorService.Send(inputHanlde);
                var nodeCatalogDto = useCaseResult?.NodeCatalogDto;

                if (nodeCatalogDto != null)
                {
                    nodeCatalogJson =
                        DataBrowserJsonSerializer.SerializeObject(
                            NodeCatalogModelView.ConvertFromDto(nodeCatalogDto, requestContext.UserLang, useCaseResult.NodeCode));
                    if (!requestContext.IgnoreCache)
                    {
                        logger.LogDebug("Save cache CatalogTreeCacheKey");
                        await dataBrowserMemoryCache.AddAsync(nodeCatalogJson, catalogTreeCacheKey);
                    }
                }
            }
            else
            {
                logger.LogDebug("Key cache found for CatalogTreeCacheKey");
            }

            return nodeCatalogJson;
        }
    }
}