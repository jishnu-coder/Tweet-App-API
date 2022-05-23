using System.ComponentModel.DataAnnotations;

namespace Tweet_App_API.Model
{
    public class UserLoginModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public string ContactNumber { get; set; }
    }
}
