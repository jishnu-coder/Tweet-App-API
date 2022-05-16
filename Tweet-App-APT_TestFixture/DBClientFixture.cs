using FluentAssertions;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweet_App_API.DataBaseLayer;
using Tweet_App_API.Model;

namespace Tweet_App_APT_TestFixture
{
    class DBClientFixture
    {
        [Test]
        public void GetUserCollectionTest()
        {
            var user = new User()
            {
                FirstName = "jishnu",
                LoginId = "jishnu123"

            };

            var userList = new List<User>();
            userList.Add(user);
            //  var userCollection = new Mock<IMongoCollection<User>>();
            var dbMock = new Mock<IMongoDatabase>();

            Mock<IAsyncCursor<string>> _collectionCursor = new Mock<IAsyncCursor<string>>();

            _collectionCursor.Setup(_ => _.Current).Returns(new List<string>() { "Users","Tweets" });
            _collectionCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            Mock<IAsyncCursor<User>> _userCursor = new Mock<IAsyncCursor<User>>();

            _userCursor.Setup(_ => _.Current).Returns(userList);
            _userCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            var collectionClient = new Mock<IMongoCollection<User>>();
            var tweetColl = new Mock<IMongoCollection<Tweet>>();


            dbMock.Setup(x => x.GetCollection<User>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>())).Returns(collectionClient.Object);
            dbMock.Setup(x => x.GetCollection<Tweet>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>())).Returns(tweetColl.Object);

            dbMock.Setup(x => x.ListCollectionNames(It.IsAny<ListCollectionNamesOptions>(),It.IsAny<CancellationToken>())).Returns(_collectionCursor.Object);

            var mongoClient = new Mock<IMongoClient>();

            mongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>())).Returns(dbMock.Object);

            var DbClent = new DBClient(new TweetAppDBSettings()
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName= "Tweet-App",
                UserCollectionName="Users",
                TweetCollectionName="Tweets"
            },mongoClient.Object);

           var userT =  DbClent.GetUserCollection();

            user.Should().BeOfType(typeof(User));

            var tweetT = DbClent.GetTweetCollection();

            tweetT.Should().NotBeNull();

        }
    }
}
