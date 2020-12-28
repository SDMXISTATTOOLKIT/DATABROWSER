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
    public class ViewTemplateListByTypeQuery : IQuery<List<ViewTemplateDto>>
    {
        public ViewTemplateListByTypeQuery(ViewTemplateType viewTemplateType,
            bool filterByPermissionNodeView = false,
            bool filterByPermissionNodeTemplate = false,
            ClaimsPrincipal specificUser = null)
        {
            ViewTemplateType = viewTemplateType;
            FilterByPermissionNodeView = filterByPermissionNodeView;
            FilterByPermissionNodeTemplate = filterByPermissionNodeTemplate;
            SpecificUser = specificUser;
        }

        public ViewTemplateType ViewTemplateType { get; }
        public bool FilterByPermissionNodeView { get; set; }
        public bool FilterByPermissionNodeTemplate { get; set; }
        public ClaimsPrincipal SpecificUser { get; set; }

        public class ViewTemplateListByTypeHandler : IRequestHandler<ViewTemplateListByTypeQuery, List<ViewTemplateDto>>
        {
            private readonly IFilterTemplate _filterTemplate;
            private readonly IFilterView _filterView;
            private readonly ILogger<ViewTemplateListByTypeHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<ViewTemplate> _repository;

            public ViewTemplateListByTypeHandler(ILogger<ViewTemplateListByTypeHandler> logger,
                IRepository<ViewTemplate> repository,
                IMapper mapper,
                IFilterView filterView,
                IFilterTemplate filterTemplate)
            {
                _logger = logger;
                _repository = repository;
                _mapper = mapper;
                _filterView = filterView;
                _filterTemplate = filterTemplate;
            }


            public async Task<List<ViewTemplateDto>> Handle(ViewTemplateListByTypeQuery request,
                CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var viewTemplateEntities =
                    await _repository.FindAsync(new ViewTemplateByTypeSpecification(-1, request.ViewTemplateType));

                var viewTemplatesDto = viewTemplateEntities.Select(x => x.ConvertToViewTemplateDto(_mapper)).ToList();

                var viewTemplatesDtoFiltered = new List<ViewTemplateDto>();
                foreach (var item in viewTemplatesDto)
                {
                    var readerPermission = ViewTemplateHelper.HavePermission(
                        filterByPermissionNodeTemplate: request.FilterByPermissionNodeTemplate,
                        filterByPermissionNodeView: request.FilterByPermissionNodeView,
                        filterBySpecificNodeId: -1,
                        filterBySpecificUser: request.SpecificUser,
                        viewTemplate: item,
                        filterTemplate: _filterTemplate,
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