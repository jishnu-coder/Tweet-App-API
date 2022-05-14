using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet_App_API.Model
{
    public class UserResponse
    {
        public string LoginId { get; set; }

        public string Email { get; set; }

        public List<string> Errors { get; set; }        
    }
}
