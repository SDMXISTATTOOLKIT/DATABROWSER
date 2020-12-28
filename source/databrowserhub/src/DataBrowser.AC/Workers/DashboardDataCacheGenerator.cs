using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Utility;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Interfaces.Workers;
using DataBrowser.Services.Interfaces;
using EndPointConnector.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DataBrowser.AC.Workers
{
    public class DashboardDataCacheGenerator : IDashboardDataCacheGenerator
    {
        private readonly IDataflowDataCache _dataflowDataCache;
        private readonly ILogger<DashboardDataCacheGenerator> _logger;
        private readonly IMapper _mapper;
        private readonly IMediatorService _mediatorService;
        private readonly IRepository<Dashboard> _repositoryDashboard;
        private readonly IRepository<Hub> _repositoryHub;
        private readonly IRepository<ViewTemplate> _repositoryViewTemplate;
        private readonly IRequestContext _requestContext;

        public DashboardDataCacheGenerator(IDataflowDataCache dataflowDataCache,
            IRepository<Dashboard> repositoryDashboard,
            IRepository<ViewTemplate> repositoryViewTemplate,
            IRepository<Hub> repositoryHub,
            IRequestContext requestContext,
            IMediatorService mediatorService,
            ILogger<DashboardDataCacheGenerator> logger,
            IMapper mapper)
        {
            _dataflowDataCache = dataflowDataCache;
            _repositoryDashboard = repositoryDashboard;
            _repositoryViewTemplate = repositoryViewTemplate;
            _repositoryHub = repositoryHub;
            _requestContext = requestContext;
            _mediatorService = mediatorService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task RefreshAllViewStaticCrossNodeAsync()
        {
            _logger.LogDebug("START RefreshAllViewStaticCrossNodeAsync");

            var dashboards = await _repositoryDashboard.ListAllAsync();

            _logger.LogDebug("get static view from all dashboards");
            var views = new List<DashboardDto.DashboardConfigItem>();
            foreach (var item in dashboards)
            {
                var dashDto = item.ConvertToDashboardDto(_mapper);
                if (dashDto.DashboardConfig == null) continue;

                var staticView = dashDto.DashboardConfig.SelectMany(i => i).Where(i =>
                    string.IsNullOrWhiteSpace(i.FilterDimension) &&
                    i.Type != null && i.Type.Equals("view", StringComparison.InvariantCultureIgnoreCase));
                if (staticView.Any()) views.AddRange(staticView);
            }

            var hub = await _repositoryHub.GetByIdAsync(1);
            var langs = new List<string> {"it", "en"};
            if (hub != null) langs = hub.ConvertToHubDto(_mapper)?.SupportedLanguages;

            await refreshViewDataAsync(views, langs);

            _logger.LogDebug("END RefreshAllViewStaticCrossNodeAsync");
        }

        private async Task refreshViewDataAsync(List<DashboardDto.DashboardConfigItem> views, List<string> langs)
        {
            var viewId = -1;
            foreach (var item in views)
            {
                string dataflowId = null;
                List<FilterCriteria> dataCriterias = null;
                try
                {
                    if (int.TryParse(Convert.ToString(item.Value), out viewId) && viewId <= 0) continue;


                    var view = await _repositoryViewTemplate.GetByIdAsync(viewId);
                    if (view == null)
                    {
                        _logger.LogDebug($"view {viewId} not found");
                        continue;
                    }

                    var viewDto = view.ConvertToViewTemplateDto(_mapper);
                    dataflowId = RequestAdapter.ConvertDataflowUriToDataflowId(viewDto.DatasetId);
                    dataCriterias = viewDto.Criteria;

                    foreach (var lang in langs)
                    {
                        _requestContext.OverwriteNodeId(viewDto.NodeId);
                        _requestContext.OverwriteUserLang(lang);

                        await _dataflowDataCache.InvalidateKeyForCurrentNodeAndLanguagesHAVEBUG(_requestContext.NodeId,
                            viewDto.DatasetId,
                            dataCriterias);

                        if (_logger.IsEnabled(LogLevel.Debug))
                            _logger.LogDebug(
                                $"Call dataflow {viewDto.DatasetId} data for with filter dataCriterias {JsonConvert.SerializeObject(dataCriterias)}\tnodeid:{viewDto.NodeId}\tlang:{lang}");
                        var dataflowRequest = new DataFromDataflowRequest
                            {DataflowId = viewDto.DatasetId, DataCriterias = dataCriterias};
                        await _mediatorService.Send(dataflowRequest);
                    }
                }
                catch (Exception ex)
                {
                    if (dataCriterias != null)
                        _logger.LogError(ex,
                            $"Some error on refreshViewDataAsync view:{viewId}\tdataflow:{dataflowId}\tdataCriterias:{JsonConvert.SerializeObject(dataCriterias)}");
                    else
                        _logger.LogError(ex,
                            $"Some error on refreshViewDataAsync view:{viewId}\tdataflow:{dataflowId}\t");
                }
            }
        }
    }
}