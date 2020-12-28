using System.Collections.Generic;

namespace WSHUB.Models.Response
{
    public class HubInfoMinimalModelView
    {
        public MinimalHub Hub { get; set; }
        public List<NodeModelView> Nodes { get; set; }

        public class MinimalHub
        {
            public string Slogan { get; set; }
            public string Description { get; set; }
            public string Name { get; set; }
            public string Disclaimer { get; set; }
            public string BackgroundMediaURL { get; set; }
            public string LogoURL { get; set; }
            public List<string> SupportedLanguages { get; set; }
            public string DefaultLanguage { get; set; }
            public int MaxObservationsAfterCriteria { get; set; }
            public long MaxCells { get; set; }
            public string Extras { get; set; }
            public List<HubDashboard> Dashboards { get; set; }

            public class HubDashboard
            {
                public int Id { get; set; }
                public string Title { get; set; }
            }
        }
    }
}