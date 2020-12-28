using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBrowser.Domain.Dtos
{
    public class HubDto
    {
        public int HubId { get; set; }
        public string LogoURL { get; set; }
        public string BackgroundMediaURL { get; set; }
        public List<string> SupportedLanguages { get; set; }
        public string DefaultLanguage { get; set; }
        public int MaxObservationsAfterCriteria { get; set; }
        public string DecimalSeparator { get; set; }
        public int DecimalNumber { get; set; }
        public long MaxCells { get; set; }
        public string EmptyCellDefaultValue { get; set; }
        public string DefaultView { get; set; }
        public string Extras { get; set; }
        public Dictionary<string, string> Title { get; set; }
        public Dictionary<string, string> Slogan { get; set; }
        public Dictionary<string, string> Description { get; set; }
        public Dictionary<string, string> Disclaimer { get; set; }
        public List<int> DashboardIds { get; set; }
    }
}
