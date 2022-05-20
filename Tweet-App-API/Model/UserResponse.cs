using System.Collections.Generic;

namespace Tweet_App_API.Model
{
    public class UserResponse
    {
        public string LoginId { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public List<string> Errors { get; set; }
    }
}
