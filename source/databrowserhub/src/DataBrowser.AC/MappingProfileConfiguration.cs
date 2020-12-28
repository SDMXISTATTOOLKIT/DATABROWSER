using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DataBrowser.AC.Responses.Services;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Serialization;
using EndPointConnector.Models;

namespace DataBrowser.AC
{
    public class MappingProfileConfiguration : Profile
    {
        public MappingProfileConfiguration()
        {
            CreateMap<Hub, HubDto>()
                .ForMember(dest => dest.Title,
                    opt => opt.MapFrom((src, dest) =>
                        src?.Title?.TransatableItemValues?.ToDictionary(k => k.Language, k => k.Value)))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom((src, dest) =>
                        src?.Description?.TransatableItemValues?.ToDictionary(k => k.Language, k => k.Value)))
                .ForMember(dest => dest.Slogan,
                    opt => opt.MapFrom((src, dest) =>
                        src?.Slogan?.TransatableItemValues?.ToDictionary(k => k.Language, k => k.Value)))
                .ForMember(dest => dest.Disclaimer,
                    opt => opt.MapFrom((src, dest) =>
                        src?.Disclaimer?.TransatableItemValues?.ToDictionary(k => k.Language, k => k.Value)))
                .ForMember(dest => dest.SupportedLanguages,
                    opt => opt.MapFrom((src, dest) =>
                        src?.SupportedLanguages?.Split(';')));

            CreateMap<Node, NodeDto>()
                .ForMember(dest => dest.Extras,
                    opt => opt.MapFrom((src, dest) =>
                        src?.Extras.Select(i => new ExtraDto
                        {
                            Key = i.Key,
                            ValueType = i.ValueType,
                            Value = i.Value,
                            IsPublic = i.IsPublic,
                            Transaltes =
                                i?.TransatableItem?.TransatableItemValues?.ToDictionary(k => k.Language, k => k.Value)
                        })))
                .ForMember(dest => dest.Title,
                    opt => opt.MapFrom((src, dest) =>
                        src?.Title?.TransatableItemValues?.ToDictionary(k => k.Language, k => k.Value)))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom((src, dest) =>
                        src?.Description?.TransatableItemValues?.ToDictionary(k => k.Language, k => k.Value)))
                .ForMember(dest => dest.Slogan,
                    opt => opt.MapFrom((src, dest) =>
                        src?.Slogan?.TransatableItemValues?.ToDictionary(k => k.Language, k => k.Value)))
                .ForMember(dest => dest.DecimalSeparator,
                    opt => opt.MapFrom((src, dest) =>
                        src?.DecimalSeparator?.TransatableItemValues?.ToDictionary(k => k.Language, k => k.Value)))
                .ForMember(dest => dest.CategorySchemaExcludes,
                    opt => opt.MapFrom((src, dest) =>
                        src?.CategorySchemaExcludes?.Split(';')))
                .ForMember(dest => dest.LabelDimensionTemporals,
                    opt => opt.MapFrom((src, dest) =>
                        src?.LabelDimensionTemporal?.Split(';')))
                .ForMember(dest => dest.LabelDimensionTerritorials,
                    opt => opt.MapFrom((src, dest) =>
                        src?.LabelDimensionTerritorial?.Split(';')));

            CreateMap<NodeDto, NodeMinimalInfoDto>();

            CreateMap<ViewTemplate, ViewTemplateDto>()
                .ForMember(dest => dest.Title,
                    opt => opt.MapFrom((src, dest) =>
                        src?.Title?.TransatableItemValues?.ToDictionary(k => k.Language, k => k.Value)))
                .ForMember(dest => dest.NodeId,
                    opt => opt.MapFrom((src, dest) =>
                        src?.NodeFK))
                .ForMember(dest => dest.UserId,
                    opt => opt.MapFrom((src, dest) =>
                        src?.UserFK))
                .ForMember(dest => dest.Criteria,
                    opt => opt.MapFrom((src, dest) =>
                        !string.IsNullOrWhiteSpace(src?.Criteria)
                            ? DataBrowserJsonSerializer.DeserializeObject<List<FilterCriteria>>(
                                src.Criteria)
                            : null))
                .ForMember(dest => dest.DecimalSeparator,
                    opt => opt.MapFrom((src, dest) =>
                        src?.DecimalSeparator?.TransatableItemValues?.ToDictionary(k => k.Language, k => k.Value)))
                .ForMember(dest => dest.Type,
                    opt => opt.MapFrom((src, dest) =>
                    {
                        if (src.Type.Equals("View", StringComparison.InvariantCultureIgnoreCase))
                            return ViewTemplateType.View;
                        return ViewTemplateType.Template;
                    }));

            CreateMap<Dashboard, DashboardDto>()
                .ForMember(dest => dest.Title,
                    opt => opt.MapFrom((src, dest) =>
                        src?.Title?.TransatableItemValues?.ToDictionary(k => k.Language, k => k.Value)))
                .ForMember(dest => dest.UserId,
                    opt => opt.MapFrom((src, dest) =>
                        src?.UserFk))
                .ForMember(dest => dest.ViewIds,
                    opt => opt.MapFrom((src, dest) =>
                        src?.Views?.Select(i => i.ViewTemplateId)))
                .ForMember(dest => dest.NodeIds,
                    opt => opt.MapFrom((src, dest) =>
                        src?.Nodes?.Select(i => i.NodeId)))
                .ForMember(dest => dest.DashboardConfig,
                    opt => opt.MapFrom((src, dest) =>
                        !string.IsNullOrWhiteSpace(src?.DashboardConfig)
                            ? DataBrowserJsonSerializer.DeserializeObject<DashboardDto.DashboardConfigItem[][]>(
                                src.DashboardConfig)
                            : null))
                .ForMember(dest => dest.HubId,
                    opt => opt.MapFrom((src, dest) =>
                        src?.HubFk));
        }
    }
}