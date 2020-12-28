using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Exceptions;
using DataBrowser.AC.Utility;
using DataBrowser.AC.Utility.Helpers;
using DataBrowser.AuthenticationAuthorization;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Specifications.ViewTemplates;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Query.ViewTemplates
{
    public class ViewListByUserIdQuery : ViewTemplatesQueryBase, IQuery<List<ViewTemplateDto>>
    {
        public ViewListByUserIdQuery(int nodeId,
            bool filterByPermissionNodeView = false,
            bool filterByPermissionNodeTemplate = false,
            ClaimsPrincipal filterBySpecificUser = null) :
            base(nodeId, filterByPermissionNodeView, filterByPermissionNodeTemplate, filterBySpecificUser)
        {
            NodeId = nodeId;
        }

        public class
            ViewTemplateListByType_UserIdHandler : IRequestHandler<ViewListByUserIdQuery, List<ViewTemplateDto>>
        {
            private readonly IFilterTemplate _filterTemplate;
            private readonly IFilterView _filterView;
            private readonly ILogger<ViewTemplateListByType_UserIdHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<ViewTemplate> _viewTemplateRepository;
            private readonly IRepository<Node> _nodeRepository;

            public ViewTemplateListByType_UserIdHandler(ILogger<ViewTemplateListByType_UserIdHandler> logger,
                IRepository<ViewTemplate> viewTemplateRepository,
                IRepository<Node> nodeRepository,
                IMapper mapper,
                IFilterView filterView,
                IFilterTemplate filterTemplate)
            {
                _logger = logger;
                _viewTemplateRepository = viewTemplateRepository;
                _mapper = mapper;
                _filterView = filterView;
                _filterTemplate = filterTemplate;
                _nodeRepository = nodeRepository;
            }


            public async Task<List<ViewTemplateDto>> Handle(ViewListByUserIdQuery request,
                CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var nodeExist = await _nodeRepository.GetByIdAsync(request.NodeId);
                if (nodeExist == null)
                    throw new ClientErrorException("NODE_NOT_FOUND", $"Node with id '{request.NodeId}' is not present");

                var userId = UtilitySecurity.GetUserId(request.SpecificUser);
                var viewEntities =
                    await _viewTemplateRepository.FindAsync(new ViewByUserIdSpecification(userId));

                var viewTemplatesDto = viewEntities.Select(x => x.ConvertToViewTemplateDto(_mapper)).ToList();

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