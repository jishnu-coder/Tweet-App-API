using System;
using System.Security.Cryptography;

namespace Tweet_App_API.TokenHandler
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        public string GenerateRegreshToken()
        {
            //Use any random number to create the token
            var randomNumber = new byte[32];
            using (var randonmNUmberGeneratir = RandomNumberGenerator.Create())
            {
                randonmNUmberGeneratir.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);

            }
        }
    }
}
