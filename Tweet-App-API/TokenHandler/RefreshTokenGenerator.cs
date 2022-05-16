using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Tweet_App_API.TokenHandler
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
         public string GenerateRegreshToken()
        {
            var randomNumber = new byte[32];
            using (var randonmNUmberGeneratir = RandomNumberGenerator.Create())
            {
                randonmNUmberGeneratir.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);

            }
        }
    }
}
