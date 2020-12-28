using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DataBrowser.AC.Responses.Services;
using DataBrowser.AC.Utility.Helpers.ExtMethod;
using DataBrowser.Domain.Dtos;
using DataBrowser.Query.Dashboards.ModelView;
using WSHUB.Models.Response;

namespace WSHUB.Models
{
    public class APIMapperConfiguration : Profile
    {
        public APIMapperConfiguration()
        {
            CreateMap<NodeMinimalInfoDto, NodeModelView>()
                .ForMember(x => x.Description, opt => opt.Ignore())
                .ForMember(x => x.Slogan, opt => opt.Ignore())
                .ForMember(x => x.Name, opt => opt.Ignore())
                .ForMember(x => x.Extras, opt => opt.Ignore());
            CreateMap<NodeModelView, NodeMinimalInfoDto>()
                .ForMember(x => x.Description, opt => opt.Ignore())
                .ForMember(x => x.Slogan, opt => opt.Ignore())
                .ForMember(x => x.Title, opt => opt.Ignore())
                .ForMember(x => x.Extras, opt => opt.Ignore());
        }
    }

    public static class MappingClass
    {
        public static NodeModelView ConvertToNodeUserModelView(this NodeDto nodeDto, string lang,
            List<DashboardViewModel> dashBoardViewModels)
        {
            //var nodeModelView = mapper.Map<NodeModelView>(nodeDto);
            if (nodeDto == null) return null;

            var nodeModelView = new NodeModelView();
            nodeModelView.NodeId = nodeDto.NodeId;
            nodeModelView.Default = nodeDto.Default;
            nodeModelView.Code = nodeDto.Code;
            nodeModelView.LogoURL = nodeDto.Logo;
            nodeModelView.Order = nodeDto.Order;
            nodeModelView.BackgroundMediaURL = nodeDto.BackgroundMediaURL;
            nodeModelView.DecimalSeparator = nodeDto.DecimalSeparator?.GetTranslateItem(lang);
            nodeModelView.DecimalNumber = nodeDto.DecimalNumber;
            nodeModelView.BackgroundMediaURL = nodeDto.BackgroundMediaURL;
            nodeModelView.EmptyCellDefaultValue = nodeDto.EmptyCellDefaultValue;
            nodeModelView.DefaultView = nodeDto.DefaultView;
            nodeModelView.ShowDataflowUncategorized = nodeDto.ShowDataflowUncategorized;
            nodeModelView.CriteriaSelectionMode = nodeDto.CriteriaSelectionMode;
            nodeModelView.LabelDimensionTerritorials = nodeDto.LabelDimensionTerritorials;
            nodeModelView.LabelDimensionTemporals = nodeDto.LabelDimensionTemporals;
            nodeModelView.EndPointFormatSupported = nodeDto.EndPointFormatSupported;
            nodeModelView.CatalogNavigationMode = nodeDto.CatalogNavigationMode;
            nodeModelView.ShowCategoryLevels = nodeDto.ShowCategoryLevels;


            nodeModelView.Name = nodeDto.Title?.GetTranslateItem(lang);
            nodeModelView.Slogan = nodeDto.Slogan?.GetTranslateItem(lang);
            nodeModelView.Description = nodeDto.Description?.GetTranslateItem(lang);
            nodeModelView.Extras = nodeDto?.Extras?.Where(i => i.IsPublic).Select(i => new ExtraModelView
                {
                    Key = i.Key, Type = i.ValueType, Value = i.Value, Transalte = i?.Transaltes?.GetTranslateItem(lang)
                })
                .ToList();

            nodeModelView.Dashboards = dashBoardViewModels?.Select(i => new NodeModelView.Dashboard
                {Id = i.DashboardId, Title = i.Title.GetTranslateItem(lang)}).ToList();


            return nodeModelView;
        }

        public static NodeModelView ConvertToNodeMinimalInfoModelView(this NodeDto nodeDto, string lang)
        {
            //var nodeModelView = mapper.Map<NodeModelView>(nodeDto);
            if (nodeDto == null) return null;

            var nodeModelView = new NodeModelView();
            nodeModelView.Active = nodeDto.Active;
            nodeModelView.Default = nodeDto.Default;
            nodeModelView.NodeId = nodeDto.NodeId;
            nodeModelView.Code = nodeDto.Code;
            nodeModelView.LogoURL = nodeDto.Logo;
            nodeModelView.Order = nodeDto.Order;
            nodeModelView.BackgroundMediaURL = nodeDto.BackgroundMediaURL;
            nodeModelView.CatalogNavigationMode = nodeDto.CatalogNavigationMode;
            nodeModelView.ShowCategoryLevels = nodeDto.ShowCategoryLevels;

            nodeModelView.Name = nodeDto.Title?.GetTranslateItem(lang);
            nodeModelView.Slogan = nodeDto.Slogan?.GetTranslateItem(lang);
            nodeModelView.Description = nodeDto.Description?.GetTranslateItem(lang);
            nodeModelView.Extras = nodeDto?.Extras?.Where(i => i.IsPublic).Select(i => new ExtraModelView
                {
                    Key = i.Key, Type = i.ValueType, Value = i.Value, Transalte = i?.Transaltes?.GetTranslateItem(lang)
                })
                .ToList();

            return nodeModelView;
        }

        public static NodeModelView ConvertToNodeMinimalInfoModelView(this NodeMinimalInfoDto nodeDto, string lang)
        {
            //var nodeModelView = mapper.Map<NodeModelView>(nodeDto);
            if (nodeDto == null) return null;

            var nodeModelView = new NodeModelView();
            nodeModelView.NodeId = nodeDto.NodeId;
            nodeModelView.Default = nodeDto.Default;
            nodeModelView.Code = nodeDto.Code;
            nodeModelView.LogoURL = nodeDto.Logo;
            nodeModelView.Order = nodeDto.Order;
            nodeModelView.BackgroundMediaURL = nodeDto.BackgroundMediaURL;
            nodeModelView.CatalogNavigationMode = nodeDto.CatalogNavigationMode;
            nodeModelView.ShowCategoryLevels = nodeDto.ShowCategoryLevels;

            nodeModelView.Name = nodeDto.Title?.GetTranslateItem(lang);
            nodeModelView.Slogan = nodeDto.Slogan?.GetTranslateItem(lang);
            nodeModelView.Description = nodeDto.Description?.GetTranslateItem(lang);
            nodeModelView.Extras = nodeDto?.Extras?.Where(i => i.IsPublic).Select(i => new ExtraModelView
                {
                    Key = i.Key, Type = i.ValueType, Value = i.Value, Transalte = i?.Transaltes?.GetTranslateItem(lang)
                })
                .ToList();

            return nodeModelView;
        }
    }
}