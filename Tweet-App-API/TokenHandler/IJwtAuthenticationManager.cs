using Tweet_App_API.Model;

namespace Tweet_App_API.TokenHandler
{
    public interface IJwtAuthenticationManager
    {
        TokenResponse Authenticate(string email, string password);
    }
}
