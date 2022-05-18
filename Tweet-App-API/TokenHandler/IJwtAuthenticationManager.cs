using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.Model;

namespace Tweet_App_API.TokenHandler
{
    public interface IJwtAuthenticationManager
    {
        TokenResponse Authenticate(string email, string password);
    }
}
