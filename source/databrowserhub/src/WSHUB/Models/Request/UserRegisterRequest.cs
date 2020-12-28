using System.ComponentModel.DataAnnotations;

namespace WSHUB.Models.Request
{
    public class UserRegisterRequest
    {
        [Required] public string FirstName { get; set; }

        [Required] public string LastName { get; set; }

        [Required] public string Email { get; set; }

        [Required] public string Password { get; set; }

        public string Organization { get; set; }
        public string Type { get; set; }
    }
}