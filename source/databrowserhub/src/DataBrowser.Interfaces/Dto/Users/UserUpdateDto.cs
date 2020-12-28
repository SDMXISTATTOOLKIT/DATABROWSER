namespace DataBrowser.Interfaces.Dto.Users
{
    public class UserUpdateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Organization { get; set; }
        public bool IsActive { get; set; }
        public string Type { get; set; }
    }
}