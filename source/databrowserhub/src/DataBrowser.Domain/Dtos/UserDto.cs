using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Settings { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<int> DashboardIds { get; set; }
    }
}
