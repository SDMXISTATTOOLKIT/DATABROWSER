using System.Collections.Generic;

namespace DataBrowser.Interfaces.Dto.Users
{
    public class UserList
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public string Organization { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Permission { get; set; }
        public bool IsActive { get; set; }
    }
}