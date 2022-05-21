using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tweet_App_API.Model;

namespace Tweet_App_API.TokenHandler
{
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        private readonly string Key;

        private readonly IRefreshTokenGenerator refreshTokenGenerator;

        public JwtAuthenticationManager(IRefreshTokenGenerator refreshTokenGenerator, IConfiguration configuration)
        {

            Key = configuration["JWT:Key"];
            this.refreshTokenGenerator = refreshTokenGenerator;

        }

        //Return the Jwt token and refresh toke for the Authenticated User
        public TokenResponse Authenticate(string email, string password)
        {
            var tokenHandler = new JwtSecurityTokenHandler();// install System.IdentityModel.Tokens.Jwt
            var tokenKey = Encoding.ASCII.GetBytes(Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                       new Claim(ClaimTypes.Name, email),

                }
                ),
                //Add expiry of 30 minutes
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = refreshTokenGenerator.GenerateRegreshToken();
            return new TokenResponse() { Token = tokenHandler.WriteToken(token), RefreshToken = refreshToken };

        }
    }
}
