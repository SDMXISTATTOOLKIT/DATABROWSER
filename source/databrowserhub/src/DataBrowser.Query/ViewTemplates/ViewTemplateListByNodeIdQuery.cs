using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Exceptions;
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
    public class ViewTemplateListByNodeIdQuery : ViewTemplatesQueryBase, IQuery<List<ViewTemplateDto>>
    {
        public ViewTemplateListByNodeIdQuery(int nodeId,
            bool filterByPermissionNodeView = false,
            bool filterByPermissionNodeTemplate = false,
            ClaimsPrincipal filterBySpecificUser = null) :
            base(nodeId, filterByPermissionNodeView, filterByPermissionNodeTemplate, filterBySpecificUser)
        {
            NodeId = nodeId;
        }

        public class
            ViewTemplateListByNodeIdHandler : IRequestHandler<ViewTemplateListByNodeIdQuery, List<ViewTemplateDto>>
        {
            private readonly IFilterTemplate _filterTemplate;
            private readonly IFilterView _filterView;
            private readonly ILogger<ViewTemplateListByNodeIdHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<ViewTemplate> _repository;

            public ViewTemplateListByNodeIdHandler(ILogger<ViewTemplateListByNodeIdHandler> logger,
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


            public async Task<List<ViewTemplateDto>> Handle(ViewTemplateListByNodeIdQuery request,
                CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var nodeExist = await _repository.GetByIdAsync(request.NodeId);
                if (nodeExist == null)
                    throw new ClientErrorException("NODE_NOT_FOUND", $"Node with id '{request.NodeId}' is not present");

                var viewTemplateEntities =
                    await _repository.FindAsync(new ViewTemplateByNodeIdSpecification(request.NodeId));

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