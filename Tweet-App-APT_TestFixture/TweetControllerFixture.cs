using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using Tweet_App_API.Controllers;
using Tweet_App_API.DataBaseLayer;
using Tweet_App_API.Model;
using Tweet_App_API.Services;
using Tweet_App_API.TokenHandler;

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
        public void UserRegisterTest()
        {
            var userObj = new UserResponse()
            {
                Email = "test@gmail.com",
                LoginId = "Test123",
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IktpcmFuMTIzIiwiZW1haWwiuYmYiOjE2NTI2MjA5Mz"
            };
            _userService.Setup(x => x.Register(It.IsAny<User>())).ReturnsAsync(userObj);
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.Register(new User() { });

            response.Result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Test]
        public void UserLoginTest()
        {
            var userObj = new UserResponse()
            {
                Email = "test@gmail.com",
                LoginId = "Test123",
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IktpcmFuMTIzIiwiZW1haWwiuYmYiOjE2NTI2MjA5Mz"
            };
            _userService.Setup(x => x.LoginUser(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(userObj);
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.Login("test123", "password");

            response.Result.Should().BeOfType(typeof(OkObjectResult));

            var content = (OkObjectResult)response.Result;
            content.Value.Should().BeOfType(typeof(UserResponse));
        }

        [Test]
        public void ResetPasswordTestSuccess()
        {

            _userService.Setup(x => x.ResetPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.ResetPassWord("test123", "password");

            response.Should().Be("Password Successfully Changed");
        }

        [Test]
        public void ResetPasswordTestFail()
        {

            _userService.Setup(x => x.ResetPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.ResetPassWord("test123", "password");

            response.Should().Be("Login Id Incorrect");
        }

        [Test]
        public void GetAllUserTest()
        {
            var collectionMock = new Mock<IMongoCollection<User>>();
            var user = new User()
            {
                FirstName = "jishnu",
                LoginId = "jishnu123"

            };

            var userList = new List<User>()
            {
                new User()
                {
                    FirstName="jishnu",
                    LoginId="jishnu123"

                }
            };


            Mock<IAsyncCursor<User>> _userCursor = new Mock<IAsyncCursor<User>>();

            //mock movenext
            _userCursor.Setup(_ => _.Current).Returns(userList);
            _userCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            collectionMock.Setup(op => op.FindSync(It.IsAny<FilterDefinition<User>>(),
                            It.IsAny<FindOptions<User, User>>(),
                            It.IsAny<CancellationToken>())).Returns(_userCursor.Object);


            var DbClient = new Mock<IDBClient>();
            DbClient.Setup(x => x.GetUserCollection()).Returns(collectionMock.Object);
            collectionMock.Object.InsertOneAsync(user);
            collectionMock.Object.InsertOneAsync(user);
            var jwtmanager = new Mock<IJwtAuthenticationManager>();
            var userService = new UserServices(DbClient.Object, jwtmanager.Object);
            var tweetController = new TweetsController(_logger.Object, userService, _tweetService.Object);

            var response = tweetController.GetAll();
            //var response = userService.Get();

            response.Result.Should().BeOfType(typeof(OkObjectResult));
        }
    }
}