using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataBrowser.AC.Exceptions;
using DataBrowser.AC.Utility;
using DataBrowser.AC.Utility.Helpers;
using DataBrowser.Command.ViewTemplates.Model;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Mediator;
using DataBrowser.Specifications.Dashboards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBrowser.Command.ViewTemplates
{
    public class RemoveViewTemplateCommand : ViewTemplatesCommandBase, ICommand<RemoveViewTemplateResult>
    {
        public RemoveViewTemplateCommand(int viewTemplateId, int nodeId,
            bool filterByPermissionNodeView = false,
            bool filterByPermissionNodeTemplate = false,
            ClaimsPrincipal specificUser = null)
            : base(nodeId, filterByPermissionNodeView, filterByPermissionNodeTemplate, specificUser)
        {
            ViewTemplateId = viewTemplateId;
        }

        public int ViewTemplateId { get; }

        public class RemoveViewTemplateHandler : IRequestHandler<RemoveViewTemplateCommand, RemoveViewTemplateResult>
        {
            private readonly IFilterTemplate _filterTemplate;
            private readonly IFilterView _filterView;
            private readonly ILogger<RemoveViewTemplateHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<Dashboard> _repositoryDashboard;
            private readonly IRepository<ViewTemplate> _repositoryViewTemplate;

            public RemoveViewTemplateHandler(ILogger<RemoveViewTemplateHandler> logger,
                IRepository<ViewTemplate> repositoryViewTemplate,
                IRepository<Dashboard> repositoryDashboard,
                IMapper mapper,
                IFilterView filterView,
                IFilterTemplate filterTemplate)
            {
                _logger = logger;
                _repositoryViewTemplate = repositoryViewTemplate;
                _repositoryDashboard = repositoryDashboard;
                _mapper = mapper;
                _filterView = filterView;
                _filterTemplate = filterTemplate;
            }

            public async Task<RemoveViewTemplateResult> Handle(RemoveViewTemplateCommand request,
                CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");
                var entity = await _repositoryViewTemplate.GetByIdAsync(request.ViewTemplateId);
                if (entity != null)
                {
                    var viewTemplateDto = entity.ConvertToViewTemplateDto(_mapper);
                    var readerPermission = ViewTemplateHelper.HavePermission(
                        filterByPermissionNodeTemplate: request.FilterByPermissionNodeTemplate,
                        filterByPermissionNodeView: request.FilterByPermissionNodeView,
                        filterBySpecificNodeId: request.NodeId,
                        filterBySpecificUser: request.SpecificUser,
                        viewTemplate: viewTemplateDto,
                        filterTemplate: _filterTemplate,
                        filterView: _filterView,
                        logger: _logger);

                    if (!readerPermission)
                    {
                        _logger.LogDebug($"Haven't permission for current viewTemplate {request.ViewTemplateId}");
                        throw new InsufficentPermissionException(
                            $"Haven't permission for current viewTemplate {request.ViewTemplateId}");
                    }

                    if (viewTemplateDto.Type == ViewTemplateType.View)
                    {
                        var dashboards =
                            await _repositoryDashboard.FindAsync(
                                new DashboardContainsViewIdSpecification(request.ViewTemplateId));
                        if (dashboards != null &&
                            dashboards.Count > 0)
                            return new RemoveViewTemplateResult
                            {
                                Dashboards = dashboards.Select(i => i.ConvertToDashboardDto(_mapper)).ToList(),
                                Deleted = false
                            };
                    }

                    _repositoryViewTemplate.Delete(entity);

                    await _repositoryViewTemplate.UnitOfWork.SaveChangesAsync();

                    return new RemoveViewTemplateResult {Deleted = true};
                }

                _logger.LogDebug("END");
                return new RemoveViewTemplateResult {Deleted = false, NotFound = true};
            }
        }
    }
}