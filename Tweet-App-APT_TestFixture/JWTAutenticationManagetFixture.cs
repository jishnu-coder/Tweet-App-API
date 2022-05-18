using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweet_App_API.TokenHandler;

namespace Tweet_App_APT_TestFixture
{
    class JWTAutenticationManagetFixture
    {

        [Test]
        public void AuthenticateTest()
        {
            var refreshTokenGenerator = new RefreshTokenGenerator();
            var config = new Mock<IConfiguration>();

            config.SetupGet(x => x[It.IsAny<string>()]).Returns("key used to create Token");

            var autenticateManager = new JwtAuthenticationManager(refreshTokenGenerator, config.Object);

            var result = autenticateManager.Authenticate("test@gmail.com", "password");

            result.Token.Should().NotBeNull();
            result.RefreshToken.Should().NotBeNull();
        }
    }
}
