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
    public class ViewTemplateById_Type_NodeIdQuery : ViewTemplatesQueryBase, IQuery<ViewTemplateDto>
    {
        public ViewTemplateById_Type_NodeIdQuery(int viewTemplateId, int nodeId, ViewTemplateType type,
            bool filterByPermissionNodeView = false,
            bool filterByPermissionNodeTemplate = false,
            ClaimsPrincipal filterBySpecificUser = null) :
            base(nodeId, filterByPermissionNodeView, filterByPermissionNodeTemplate, filterBySpecificUser)
        {
            ViewTemplateId = viewTemplateId;
            Type = type;
        }

        public int ViewTemplateId { get; }
        public ViewTemplateType Type { get; }

        public class
            ViewTemplateById_Type_NodeIdHandler : IRequestHandler<ViewTemplateById_Type_NodeIdQuery, ViewTemplateDto>
        {
            private readonly IFilterTemplate _filterTemplate;
            private readonly IFilterView _filterView;
            private readonly ILogger<ViewTemplateById_Type_NodeIdHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<ViewTemplate> _repository;

            public ViewTemplateById_Type_NodeIdHandler(ILogger<ViewTemplateById_Type_NodeIdHandler> logger,
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

            public async Task<ViewTemplateDto> Handle(ViewTemplateById_Type_NodeIdQuery request,
                CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var viewTemplateEntity = (await _repository.FindAsync(
                    new ViewTemplateById_Type_NodeIdSpecification(request.ViewTemplateId, request.NodeId,
                        request.Type)))?.FirstOrDefault();

                if (viewTemplateEntity == null) return null;

                var result = viewTemplateEntity.ConvertToViewTemplateDto(_mapper);

                var readerPermission = ViewTemplateHelper.HavePermission(
                    filterByPermissionNodeTemplate: request.FilterByPermissionNodeTemplate,
                    filterByPermissionNodeView: request.FilterByPermissionNodeView,
                    filterBySpecificNodeId: request.NodeId,
                    filterBySpecificUser: request.SpecificUser,
                    viewTemplate: result,
                    filterTemplate: _filterTemplate,
                    filterView: _filterView,
                    logger: _logger);
                if (!readerPermission)
                {
                    _logger.LogDebug("Check user haven't permission");
                    return null;
                }

                _logger.LogDebug("END");
                return result;
            }
        }
    }
}