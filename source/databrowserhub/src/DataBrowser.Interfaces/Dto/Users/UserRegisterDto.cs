namespace DataBrowser.Interfaces.Dto.Users
{
    public class UserRegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Organization { get; set; }
        public string Type { get; set; }
    }
}