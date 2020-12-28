using System;
using System.Collections.Generic;
using System.Linq;
using DataBrowser.Domain.Dtos;
using EndPointConnector.Models;

namespace DataBrowser.Interfaces.ModelViews
{
    public class ViewTemplateViewModel
    {
        public int ViewTemplateId { get; set; }
        public string DatasetId { get; set; }
        public ViewTemplateType Type { get; set; }
        public string DefaultView { get; set; }
        public List<FilterCriteria> Criteria { get; set; }
        public string Layouts { get; set; }
        public string HiddenDimensions { get; set; }
        public bool BlockAxes { get; set; }
        public bool EnableLayout { get; set; }
        public bool EnableCriteria { get; set; }
        public bool EnableVariation { get; set; }
        public int DecimalNumber { get; set; }
        public int NodeId { get; set; }
        public int? UserId { get; set; }
        public DateTime ViewTemplateCreationDate { get; set; }
        public Dictionary<string, string> Title { get; set; }
        public Dictionary<string, string> DecimalSeparator { get; set; }
        public List<int> DashboardIds { get; set; }

        public ViewTemplateViewModel AssociatedTemplate;

        public static List<ViewTemplateViewModel> ConvertFromDto(IEnumerable<ViewTemplateDto> viewTemplateDto)
        {
            if (viewTemplateDto == null) return null;
            return viewTemplateDto.Select(i => ConvertFromDto(i)).ToList();
        }

        public static ViewTemplateViewModel ConvertFromDto(ViewTemplateDto viewTemplateDto)
        {
            if (viewTemplateDto == null) return null;

            return new ViewTemplateViewModel
            {
                ViewTemplateId = viewTemplateDto.ViewTemplateId,
                DatasetId = viewTemplateDto.DatasetId,
                Type = viewTemplateDto.Type,
                DefaultView = viewTemplateDto.DefaultView,
                Criteria = viewTemplateDto.Criteria,
                Layouts = viewTemplateDto.Layouts,
                HiddenDimensions = viewTemplateDto.HiddenDimensions,
                BlockAxes = viewTemplateDto.BlockAxes,
                EnableCriteria = viewTemplateDto.EnableCriteria,
                EnableVariation = viewTemplateDto.EnableVariation,
                DecimalNumber = viewTemplateDto.DecimalNumber,
                NodeId = viewTemplateDto.NodeId,
                UserId = viewTemplateDto.UserId,
                ViewTemplateCreationDate = viewTemplateDto.ViewTemplateCreationDate,
                Title = viewTemplateDto.Title,
                DecimalSeparator = viewTemplateDto.DecimalSeparator,
                DashboardIds = viewTemplateDto.DashboardIds,
                EnableLayout = viewTemplateDto.EnableLayout
            };
        }

        public static ViewTemplateDto ConvertToDto(ViewTemplateViewModel viewTemplateViewModel)
        {
            if (viewTemplateViewModel == null) return null;

            return new ViewTemplateDto
            {
                ViewTemplateId = viewTemplateViewModel.ViewTemplateId,
                DatasetId = viewTemplateViewModel.DatasetId,
                Type = viewTemplateViewModel.Type,
                DefaultView = viewTemplateViewModel.DefaultView,
                Criteria = viewTemplateViewModel.Criteria,
                Layouts = viewTemplateViewModel.Layouts,
                HiddenDimensions = viewTemplateViewModel.HiddenDimensions,
                BlockAxes = viewTemplateViewModel.BlockAxes,
                EnableCriteria = viewTemplateViewModel.EnableCriteria,
                EnableVariation = viewTemplateViewModel.EnableVariation,
                DecimalNumber = viewTemplateViewModel.DecimalNumber,
                NodeId = viewTemplateViewModel.NodeId,
                UserId = viewTemplateViewModel.UserId,
                ViewTemplateCreationDate = viewTemplateViewModel.ViewTemplateCreationDate,
                Title = viewTemplateViewModel.Title,
                DecimalSeparator = viewTemplateViewModel.DecimalSeparator,
                DashboardIds = viewTemplateViewModel.DashboardIds,
                EnableLayout = viewTemplateViewModel.EnableLayout
            };
        }
    }
}