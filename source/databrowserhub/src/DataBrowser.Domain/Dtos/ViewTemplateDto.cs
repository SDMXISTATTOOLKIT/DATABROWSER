using DataBrowser.Domain.Serialization;
using EndPointConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataBrowser.Domain.Dtos
{
    public class ViewTemplateDto
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
        public Dictionary<string,string> Title { get; set; }
        public Dictionary<string, string> DecimalSeparator { get; set; }
        public List<int> DashboardIds { get; set; }

        public string ConvertCriteriaToText()
        {
            if (Criteria == null ||
                !Criteria.Any())
            {
                return "[]";
            }
            return DataBrowserJsonSerializer.SerializeObject(Criteria);
        }

    }
}
