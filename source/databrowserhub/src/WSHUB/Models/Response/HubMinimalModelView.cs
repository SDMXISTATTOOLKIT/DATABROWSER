using System.Collections.Generic;

namespace WSHUB.Models.Response
{
    public class HubMinimalModelView
    {
        public int HubId { get; set; }
        public string LogoURL { get; set; }
        public string BackgroundMediaURL { get; set; }
        public List<string> SupportedLanguages { get; set; }
        public string DefaultLanguage { get; set; }
        public int MaxObservationsAfterCriteria { get; set; }
        public string Title { get; set; }
        public string Slogan { get; set; }
        public string Description { get; set; }
        public string Disclaimer { get; set; }
        public List<ExtraModelView> Extras { get; set; }
        public List<DashboardModelView> Dashboards { get; set; }

        public class DashboardModelView
        {
            public int Id { get; set; }
            public string Title { get; set; }
        }
    }
}