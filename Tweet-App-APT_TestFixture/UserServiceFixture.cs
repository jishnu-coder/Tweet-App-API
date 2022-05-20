using FluentAssertions;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using Tweet_App_API.DataBaseLayer;
using Tweet_App_API.Model;
using Tweet_App_API.Services;
using Tweet_App_API.TokenHandler;

namespace Tweet_App_APT_TestFixture
{
    class UserServiceFixture
    {
        Mock<IMongoCollection<User>> _users;
        Mock<IJwtAuthenticationManager> jwtAuthenticationManager;

        [SetUp]
        public void Setup()
        {
            _users = new Mock<IMongoCollection<User>>();
            jwtAuthenticationManager = new Mock<IJwtAuthenticationManager>();
        }

        [Test]
        public void GetUserByEmailTest()
        {
            var user = new User()
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@gmail.com",
                LoginId = "test123"

            };
            var userList = new List<User>();
            userList.Add(new User()
            {
                FirstName = "Test1",
                LastName = "User",
                Email = "test1@gmail.com",
                LoginId = "test1"

            });

            userList.Add(new User()
            {
                FirstName = "Test2",
                LastName = "User",
                Email = "test2@gmail.com",
                LoginId = "test2"

            });

            Mock<IAsyncCursor<User>> _userCursor = new Mock<IAsyncCursor<User>>();

            //mock movenext
            _userCursor.Setup(_ => _.Current).Returns(userList);
            _userCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _users.Setup(op => op.FindAsync<User>(It.IsAny<FilterDefinition<User>>(),
                            It.IsAny<FindOptions<User, User>>(),
                            It.IsAny<CancellationToken>())).ReturnsAsync(_userCursor.Object);

            var DbClient = new Mock<IDBClient>();
            DbClient.Setup(x => x.GetUserCollection()).Returns(_users.Object);

            var userService = new UserServices(DbClient.Object, jwtAuthenticationManager.Object);

            var result = userService.GetUserByEmail("test1");

            result.Result.Email.Should().BeEquivalentTo("test1@gmail.com");
        }

        [Test]
        public void UserRegisterTest()
        {
            var user = new User()
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@gmail.com",
                LoginId = "test123",
                Password = "password"

            };


            jwtAuthenticationManager.Setup(x => x.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                  .Returns(new TokenResponse()
                  {
                      Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6",
                      RefreshToken = "rtyfwhikooloGTUIKK"
                  });

            var DbClient = new Mock<IDBClient>();
            DbClient.Setup(x => x.GetUserCollection()).Returns(_users.Object);

            var userService = new UserServices(DbClient.Object, jwtAuthenticationManager.Object);

            var result = userService.Register(user);

            result.Result.Email.Should().BeEquivalentTo("test@gmail.com");
        }

        [Test]
        public void UserRegisterFailedTest()
        {
            var user = new User()
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@gmail.com",
                LoginId = "test123",
                Password = "password"

            };


            _users.Setup(x => x.InsertOneAsync(It.IsAny<User>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("customEmail exception"));

            jwtAuthenticationManager.Setup(x => x.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                  .Returns(new TokenResponse()
                  {
                      Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6",
                      RefreshToken = "rtyfwhikooloGTUIKK"
                  });

            var DbClient = new Mock<IDBClient>();
            DbClient.Setup(x => x.GetUserCollection()).Returns(_users.Object);

            var userService = new UserServices(DbClient.Object, jwtAuthenticationManager.Object);

            var result = userService.Register(user);

            result.Result.Errors.Should().HaveCount(1);
        }

        [Test]
        public void LoginUserTest()
        {

            var userList = new List<User>();
            userList.Add(new User()
            {
                FirstName = "Test1",
                LastName = "User",
                Email = "test1@gmail.com",
                LoginId = "test1",
                Password = "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg="

            });

            userList.Add(new User()
            {
                FirstName = "Test2",
                LastName = "User",
                Email = "test2@gmail.com",
                LoginId = "test2"

            });

            Mock<IAsyncCursor<User>> _userCursor = new Mock<IAsyncCursor<User>>();

            //mock movenext
            _userCursor.Setup(_ => _.Current).Returns(userList);
            _userCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _users.Setup(op => op.FindAsync<User>(It.IsAny<FilterDefinition<User>>(),
                            It.IsAny<FindOptions<User, User>>(),
                            It.IsAny<CancellationToken>())).ReturnsAsync(_userCursor.Object);

            jwtAuthenticationManager.Setup(x => x.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                  .Returns(new TokenResponse()
                  {
                      Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6",
                      RefreshToken = "rtyfwhikooloGTUIKK"
                  });

            var DbClient = new Mock<IDBClient>();
            DbClient.Setup(x => x.GetUserCollection()).Returns(_users.Object);

            var userService = new UserServices(DbClient.Object, jwtAuthenticationManager.Object);

            var result = userService.LoginUser("test1", "password");

            result.Result.Email.Should().BeEquivalentTo("test1@gmail.com");
        }

        [Test]
        public void LoginUserWithIncorrectPasswordTest()
        {

            var userList = new List<User>();
            userList.Add(new User()
            {
                FirstName = "Test1",
                LastName = "User",
                Email = "test1@gmail.com",
                LoginId = "test1",
                Password = "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0"

            });

            userList.Add(new User()
            {
                FirstName = "Test2",
                LastName = "User",
                Email = "test2@gmail.com",
                LoginId = "test2"

            });

            Mock<IAsyncCursor<User>> _userCursor = new Mock<IAsyncCursor<User>>();

            //mock movenext
            _userCursor.Setup(_ => _.Current).Returns(userList);
            _userCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _users.Setup(op => op.FindAsync<User>(It.IsAny<FilterDefinition<User>>(),
                            It.IsAny<FindOptions<User, User>>(),
                            It.IsAny<CancellationToken>())).ReturnsAsync(_userCursor.Object);

            jwtAuthenticationManager.Setup(x => x.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                  .Returns(new TokenResponse()
                  {
                      Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6",
                      RefreshToken = "rtyfwhikooloGTUIKK"
                  });

            var DbClient = new Mock<IDBClient>();
            DbClient.Setup(x => x.GetUserCollection()).Returns(_users.Object);

            var userService = new UserServices(DbClient.Object, jwtAuthenticationManager.Object);

            var result = userService.LoginUser("test1", "password");

            result.Result.Errors.Should().HaveCount(1);
        }

        [Test]
        public void ResetPasswordTest()
        {
            var user = new User()
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@gmail.com",
                LoginId = "test123",
                Password = "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg="

            };
            var userList = new List<User>();
            userList.Add(new User()
            {
                FirstName = "Test1",
                LastName = "User",
                Email = "test1@gmail.com",
                LoginId = "test1"

            });

            userList.Add(new User()
            {
                FirstName = "Test2",
                LastName = "User",
                Email = "test2@gmail.com",
                LoginId = "test2"

            });

            Mock<IAsyncCursor<User>> _userCursor = new Mock<IAsyncCursor<User>>();

            //mock movenext
            _userCursor.Setup(_ => _.Current).Returns(userList);
            _userCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            _users.Setup(op => op.FindSync(It.IsAny<FilterDefinition<User>>(),
                            It.IsAny<FindOptions<User, User>>(),
                            It.IsAny<CancellationToken>())).Returns(_userCursor.Object);

            var DbClient = new Mock<IDBClient>();
            DbClient.Setup(x => x.GetUserCollection()).Returns(_users.Object);

            var userService = new UserServices(DbClient.Object, jwtAuthenticationManager.Object);

            var result = userService.ResetPassword("test1", "newPassword");

            result.Should().Be(true);
        }
    }
}
