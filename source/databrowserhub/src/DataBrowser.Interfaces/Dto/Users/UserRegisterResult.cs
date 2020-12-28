using System.Collections.Generic;

namespace DataBrowser.Interfaces.Dto.Users
{
    public class UserRegisterResult
    {
        public bool HaveError { get; set; }
        public List<string> Errors { get; set; }
        public string Username { get; set; }
        public int UserId { get; set; }
    }
}