using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Utility;
using DataBrowser.AC.Utility.Helpers;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Specifications.ViewTemplates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Query.ViewTemplates
{
    public class TakeViewFromManyIdsQuery : IQuery<List<ViewTemplateDto>>
    {
        public TakeViewFromManyIdsQuery(List<int> views,
            bool filterByPermissionNodeView = false,
            ClaimsPrincipal filterBySpecificUser = null)
        {
            Views = views;
            FilterByPermissionNodeView = filterByPermissionNodeView;
            SpecificUser = filterBySpecificUser;
        }

        public int NodeId => -1;
        public bool FilterByPermissionNodeView { get; set; }
        public bool FilterByPermissionNodeTemplate => false;
        public ClaimsPrincipal SpecificUser { get; set; }
        public List<int> Views { get; }

        public class TakeViewFromManyIdsHandler : IRequestHandler<TakeViewFromManyIdsQuery, List<ViewTemplateDto>>
        {
            private readonly IFilterView _filterView;
            private readonly ILogger<TakeViewFromManyIdsHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<ViewTemplate> _viewTemplateRepository;

            public TakeViewFromManyIdsHandler(ILogger<TakeViewFromManyIdsHandler> logger,
                IMapper mapper,
                IFilterView filterView,
                IRepository<ViewTemplate> viewTemplateRepository)
            {
                _logger = logger;
                _mapper = mapper;
                _filterView = filterView;
                _viewTemplateRepository = viewTemplateRepository;
            }

            public async Task<List<ViewTemplateDto>> Handle(TakeViewFromManyIdsQuery request,
                CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var viewTemplateEntities =
                    await _viewTemplateRepository.FindAsync(new ViewTemplateByMultiIdsSpecification(request.Views));

                var viewTemplatesDto = viewTemplateEntities.Select(x => x.ConvertToViewTemplateDto(_mapper)).ToList();

                var viewTemplatesDtoFiltered = new List<ViewTemplateDto>();
                foreach (var item in viewTemplatesDto)
                {
                    var readerPermission = ViewTemplateHelper.HavePermission(
                        filterByPermissionNodeTemplate: request.FilterByPermissionNodeTemplate,
                        filterByPermissionNodeView: request.FilterByPermissionNodeView,
                        filterBySpecificNodeId: request.NodeId,
                        filterBySpecificUser: request.SpecificUser,
                        viewTemplate: item,
                        filterTemplate: null,
                        filterView: _filterView,
                        logger: _logger);
                    if (!readerPermission)
                    {
                        _logger.LogDebug($"Check user haven't permission for viewTemplate {item.ViewTemplateId}");
                        continue;
                    }

                    viewTemplatesDtoFiltered.Add(item);
                }

                _logger.LogDebug("END");
                return viewTemplatesDtoFiltered;
            }
        }
    }
}