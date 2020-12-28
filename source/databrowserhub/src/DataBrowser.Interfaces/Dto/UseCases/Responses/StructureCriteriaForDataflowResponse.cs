using System.Collections.Generic;
using DataBrowser.Domain.Dtos;
using EndPointConnector.Models;

namespace DataBrowser.Interfaces.Dto.UseCases.Responses
{
    public class StructureCriteriaForDataflowResponse
    {
        public List<Criteria> Criterias { get; set; }
        public string CriteriaMode { get; set; }
        public List<string> LayoutRows { get; set; }
        public List<string> LayoutColumns { get; set; }
        public List<string> LayoutRowSelections { get; set; }
        public List<string> LayoutChartPrimaryDim { get; set; }
        public List<string> LayoutChartSecondaryDim { get; set; }
        public List<string> LayoutChartFilter { get; set; }
        public int DecimalNumber { get; set; }
        public string DecimalSeparator { get; set; }
        public long? MaxCell { get; set; }
        public List<string> GeoIds { get; set; }
        public List<FilterCriteria> DefaultCodeSelected { get; set; }
        public string EmptyCellPlaceHolder { get; set; }
        public List<string> TerritorialDimensions { get; set; }
        public ViewTemplateDto Template { get; set; }
        public ViewTemplateDto View { get; set; }
        public string DefaultView { get; set; }
    }
}