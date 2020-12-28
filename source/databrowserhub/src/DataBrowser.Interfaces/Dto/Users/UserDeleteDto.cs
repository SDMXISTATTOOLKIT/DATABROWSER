using System.Collections.Generic;

namespace DataBrowser.Interfaces.Dto.Users
{
    public class UserDeleteDto
    {
        public bool Deleted { get; set; }
        public List<EntityUseDto> Dashboards { get; set; }
        public bool NotFound { get; set; }
        public bool CanDeleteUserItself { get; set; }

        public class EntityUseDto
        {
            public int Id { get; set; }
            public Dictionary<string, string> Title { get; set; }
            public List<EntityUseDto> Nodes { get; set; }
            public List<EntityUseDto> Views { get; set; }
        }
    }
}