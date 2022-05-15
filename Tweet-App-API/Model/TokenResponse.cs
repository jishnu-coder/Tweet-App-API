using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet_App_API.Model
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string  RefreshToken { get; set; }
    }
}
