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
    public class ViewTemplateByType_Dataset_NodeIdQuery : ViewTemplatesQueryBase, IQuery<List<ViewTemplateDto>>
    {
        public ViewTemplateByType_Dataset_NodeIdQuery(int nodeId, string datasetId, ViewTemplateType viewTemplateType,
            bool filterByPermissionNodeView = false,
            bool filterByPermissionNodeTemplate = false,
            ClaimsPrincipal filterBySpecificUser = null) :
            base(nodeId, filterByPermissionNodeView, filterByPermissionNodeTemplate, filterBySpecificUser)
        {
            NodeId = nodeId;
            DatasetId = datasetId;
            ViewTemplateType = viewTemplateType;
        }

        public string DatasetId { get; }
        public ViewTemplateType ViewTemplateType { get; }

        public class
            ViewTemplateByTypeDataset_NodeIdHandler : IRequestHandler<ViewTemplateByType_Dataset_NodeIdQuery,
                List<ViewTemplateDto>>
        {
            private readonly IFilterTemplate _filterTemplate;
            private readonly IFilterView _filterView;
            private readonly ILogger<ViewTemplateByTypeDataset_NodeIdHandler> _logger;
            private readonly IMapper _mapper;
            private readonly IRepository<ViewTemplate> _repository;

            public ViewTemplateByTypeDataset_NodeIdHandler(ILogger<ViewTemplateByTypeDataset_NodeIdHandler> logger,
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

            public async Task<List<ViewTemplateDto>> Handle(ViewTemplateByType_Dataset_NodeIdQuery request,
                CancellationToken cancellationToken)
            {
                _logger.LogDebug("START");

                var viewTemplateEntities = await _repository.FindAsync(
                    new ViewTemplateByType_Dataset_NodeIdSpecification(request.NodeId, request.DatasetId,
                        request.ViewTemplateType));

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