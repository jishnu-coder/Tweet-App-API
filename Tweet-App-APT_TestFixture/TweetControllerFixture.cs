using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Tweet_App_API.Controllers;
using Tweet_App_API.Services;
using Tweet_App_API.Model;
using System.Threading.Tasks;

namespace Tweet_App_APT_TestFixture
{
    public class TweetControllerFixture
    {
        Mock<ILogger<TweetsController>> _logger;
        Mock<IUserServices> _userService;
        Mock<ITweetService> _tweetService;
        [SetUp]
        public void Setup()
        {
             _logger = new Mock<ILogger<TweetsController>>();
             _userService = new Mock<IUserServices>();
             _tweetService = new Mock<ITweetService>();

            

        }

        [Test]
        public async Task UserRegisterTest()
        {
            _userService.Setup(x => x.Register(It.IsAny<User>())).ReturnsAsync(new UserResponse()
            { Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IktpcmFuMTIzIiwiZW1haWwiuYmYiOjE2NTI2MjA5Mz" });
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = await tweetController.Register(new User() { });

            
        }
    }
}