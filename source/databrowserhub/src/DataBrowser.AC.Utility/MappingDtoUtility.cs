using AutoMapper;
using DataBrowser.AC.Responses.Services;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Entities.Geometry;
using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Entities.ViewTemplates;

namespace DataBrowser.AC.Utility
{
    public static class MappingDtoUtility
    {
        public static HubDto ConvertToHubDto(this Hub hubEntity, IMapper mapper)
        {
            return mapper.Map<HubDto>(hubEntity);
        }

        public static NodeDto ConvertToNodeDto(this Node nodeEntity, IMapper mapper)
        {
            return mapper.Map<NodeDto>(nodeEntity);
        }

        public static NodeMinimalInfoDto ConvertToNodeDataView(this NodeDto nodeDto, IMapper mapper)
        {
            return mapper.Map<NodeMinimalInfoDto>(nodeDto);
        }

        public static NodeMinimalInfoDto ConvertToNodeDataView(this Node nodeEntity, IMapper mapper)
        {
            var nodeDto = mapper.Map<NodeDto>(nodeEntity);
            return mapper.Map<NodeMinimalInfoDto>(nodeDto);
        }

        public static ViewTemplateDto ConvertToViewTemplateDto(this ViewTemplate viewTemplateEntity, IMapper mapper)
        {
            return mapper.Map<ViewTemplateDto>(viewTemplateEntity);
        }

        public static DashboardDto ConvertToDashboardDto(this Dashboard dashboard, IMapper mapper)
        {
            return mapper.Map<DashboardDto>(dashboard);
        }

        public static GeometryDto ConvertToGeometryDto(this Geometry geometry, IMapper mapper)
        {
            return mapper.Map<GeometryDto>(geometry);
        }
    }
}