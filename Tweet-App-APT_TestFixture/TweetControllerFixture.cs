using AutoMapper;
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
using Tweet_App_API.Exceptions;
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
        Mock<IMapper> _mapper;
        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<TweetsController>>();
            _userService = new Mock<IUserServices>();
            _tweetService = new Mock<ITweetService>();
            _mapper = new Mock<IMapper>();
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
        public void UserRegisterTest_ModelState_NotValid()
        {

            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            tweetController.ModelState.AddModelError("test", "test");

            var response = tweetController.Register(new User() { });

            response.Result.Should().BeOfType(typeof(BadRequestResult));
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

            var response = tweetController.Login(new UserLoginModel() { UserName = "test@gmail.com", Password = "password" });

            response.Result.Should().BeOfType(typeof(OkObjectResult));

            var content = (OkObjectResult)response.Result;
            content.Value.Should().BeOfType(typeof(UserResponse));
        }

        [Test]
        public void UserLoginTest_NullInput()
        {

            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            tweetController.ModelState.AddModelError("test", "test");

            var response = tweetController.Login(new UserLoginModel() { UserName = "test@gmail.com" });

            response.Result.Should().BeOfType(typeof(BadRequestResult));

        }

        [Test]
        public void ResetPasswordTestSuccess()
        {

            _userService.Setup(x => x.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.ResetPassWord("test1230", new UserLoginModel() { UserName = "test123", Password = "password" });

            response.Result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Test]
        public void ResetPasswordTestFail()
        {

            _userService.Setup(x => x.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.ResetPassWord("test@123", new UserLoginModel() { UserName = "test123", Password = "password" });


            response.Result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Test]
        public void ResetPasswordTestFail_ModelState_Invalid()
        {

            _userService.Setup(x => x.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            tweetController.ModelState.AddModelError("test", "test");

            var response = tweetController.ResetPassWord("test@123", new UserLoginModel() { UserName = "test123", Password = "password" });

            response.Result.Should().BeOfType(typeof(BadRequestObjectResult));
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
            var userService = new UserServices(DbClient.Object, jwtmanager.Object, _mapper.Object);
            var tweetController = new TweetsController(_logger.Object, userService, _tweetService.Object);

            var response = tweetController.GetAllUsers();
            //var response = userService.Get();

            response.Result.Should().BeNull();
        }

        [Test]
        public void GeUserByNameTest()
        {

            _userService.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new UserViewModel() { Email = "test@123" });
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.GetUserByName("test@123");

            response.Result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Test]
        public void CreateTweetTest()
        {

            _tweetService.Setup(x => x.PostTweet(It.IsAny<Tweet>())).ReturnsAsync(new Tweet() { TweetId = "123456890" });
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.CreateTweet("test", new Tweet() { TweetId = "123456" });
            response.Result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Test]
        public void CreateTweetTestException()
        {

            _tweetService.Setup(x => x.PostTweet(It.IsAny<Tweet>())).ThrowsAsync(new System.Exception());
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.CreateTweet("test", new Tweet() { TweetId = "123456" });
            response.Result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Test]
        public void CreateTweetTest_ModelStateInvalid()
        {

            _tweetService.Setup(x => x.PostTweet(It.IsAny<Tweet>())).ReturnsAsync(new Tweet() { TweetId = "123456890" });
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            tweetController.ModelState.AddModelError("test", "test");

            var response = tweetController.CreateTweet("test@123", new Tweet() { TweetId = "123456" });
            response.Result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Test]
        public void GetAllTweetTest()
        {

            _tweetService.Setup(x => x.GetAll()).Returns(new List<Tweet>() { new Tweet() { TweetId = "1234567" } });
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.GetAllTweet();
            response.Should().HaveCount(1);
        }

        [Test]
        public void GetTweetByIdTest()
        {

            _tweetService.Setup(x => x.GetTweetsByUserId(It.IsAny<string>())).ReturnsAsync(new List<Tweet>() { new Tweet() { TweetId = "1234567" } });
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.GetTweetById("12345");
            response.Result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Test]
        public void GetTweetByIdTestWithException()
        {

            _tweetService.Setup(x => x.GetTweetsByUserId(It.IsAny<string>())).ThrowsAsync(new InvalidUserNameException("Exception"));
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.GetTweetById("12345");
            response.Result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Test]
        public void GetTweetByTweetIdTest()
        {

            _tweetService.Setup(x => x.GetTweetByTweetId(It.IsAny<string>())).ReturnsAsync( new Tweet() { TweetId = "1234567" } );
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.GetTweetByTweetId("12345");
            response.Result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Test]
        public void UpdateTweetTest()
        {

            _tweetService.Setup(x => x.UpdateTweet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tweet>())).ReturnsAsync(new Tweet() { TweetId = "1234567" });
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.UpdateTweet("test@gmail.com", "12344", new Tweet() { TweetId = "12345" });
            response.Result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Test]
        public void UpdateTweetTestWithException()
        {

            _tweetService.Setup(x => x.UpdateTweet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Tweet>())).ThrowsAsync(new System.Exception());
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.UpdateTweet("test@gmail.com", "12344", new Tweet() { TweetId = "12345" });
            response.Result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Test]
        public void DeleteTweetTest()
        {
            var deleteResult = new Mock<DeleteResult>();
            _tweetService.Setup(x => x.DeleteTweet(It.IsAny<string>())).ReturnsAsync(deleteResult.Object);
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.DeleteTweet("test", "12344");
            response.Result.Should().Be(deleteResult.Object);
        }

        [Test]
        public void LikeTweetTest()
        {

            _tweetService.Setup(x => x.LikeTweet(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Tweet() { TweetId = "123456" });
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.LikeTweet("test", "123456");
            response.Result.Should().BeOfType(typeof(Tweet));
        }

        [Test]
        public void ReplyTweetTest()
        {

            _tweetService.Setup(x => x.ReplyTweet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TweetReply>())).ReturnsAsync(new Tweet() { TweetId = "123456" });
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.ReplyTweet("test", "12345", new TweetReply() { Replied_userId = "test1", ReplyMessage = "new Reply" });
            response.Result.Should().BeOfType(typeof(OkObjectResult));
        }


        [Test]
        public void ReplyTweetTestWithException()
        {

            _tweetService.Setup(x => x.ReplyTweet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TweetReply>())).ThrowsAsync(new System.Exception());
            var tweetController = new TweetsController(_logger.Object, _userService.Object, _tweetService.Object);

            var response = tweetController.ReplyTweet("test", "12345", new TweetReply() { Replied_userId = "test1", ReplyMessage = "new Reply" });
            response.Result.Should().BeOfType(typeof(BadRequestObjectResult));
        }
    }
}