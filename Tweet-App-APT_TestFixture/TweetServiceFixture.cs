using FluentAssertions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using  System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweet_App_API.DataBaseLayer;
using Tweet_App_API.Model;
using Tweet_App_API.Services;

namespace Tweet_App_APT_TestFixture
{
    class TweetServiceFixture
    {
        Mock<IMongoCollection<Tweet>> _tweet;
        Mock<IGuidService> _guid;
        Mock<IDBClient> _Client;
        Mock<IMongoQueryable<Tweet>> _mongoQueryableMock;

        

        [SetUp]
        public void Setup()
        {
            _tweet = new Mock<IMongoCollection<Tweet>>();
            _guid = new Mock<IGuidService>();
            _Client = new Mock<IDBClient>();
            _mongoQueryableMock = new Mock<IMongoQueryable<Tweet>>();

        }

        [Test]
        public void PostTweetTest()
        {
            var tweetList = new List<Tweet>() { };

            tweetList.Add(new Tweet()
            {
                TweetId= new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709").ToString(),
                CreatorId="jishnu123",
                Content ="New Content",
                Tags=new List<string>(),
                Likes= new List<string>(),
                Replys = new List<TweetReply>()
            });;

            Mock<IAsyncCursor<Tweet>> _tweetCursor = new Mock<IAsyncCursor<Tweet>>();

            //mock movenext
            _tweetCursor.Setup(_ => _.Current).Returns(tweetList);
            _tweetCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _tweet.Setup(op => op.FindAsync<Tweet>(It.IsAny<FilterDefinition<Tweet>>(),
                            It.IsAny<FindOptions<Tweet, Tweet>>(),
                            It.IsAny<CancellationToken>())).ReturnsAsync(_tweetCursor.Object);

            _guid.Setup(x => x.NewGuid()).Returns(new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709"));

            _Client.Setup(x => x.GetTweetCollection()).Returns(_tweet.Object);

            var tweetService = new TweetService(_Client.Object, _guid.Object);

            var response = tweetService.PostTweet(new Tweet() { Content = "New Content" ,  });

            response.Result.TweetId.Should().Be(new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709").ToString());

        }

        [Test]
        public void GetByUserId()
        {
            var tweetList = new List<Tweet>() { };

            tweetList.Add(new Tweet()
            {
                TweetId = new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709").ToString(),
                CreatorId = "jishnu123",
                Content = "New Content",
                Tags = new List<string>(),
                Likes = new List<string>(),
                Replys = new List<TweetReply>()
            }); ;

            Mock<IAsyncCursor<Tweet>> _tweetCursor = new Mock<IAsyncCursor<Tweet>>();

            //mock movenext
            _tweetCursor.Setup(_ => _.Current).Returns(tweetList);
            _tweetCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _tweet.Setup(op => op.FindAsync<Tweet>(It.IsAny<FilterDefinition<Tweet>>(),
                            It.IsAny<FindOptions<Tweet, Tweet>>(),
                            It.IsAny<CancellationToken>())).ReturnsAsync(_tweetCursor.Object);

            _guid.Setup(x => x.NewGuid()).Returns(new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709"));

            _Client.Setup(x => x.GetTweetCollection()).Returns(_tweet.Object);

            var tweetService = new TweetService(_Client.Object, _guid.Object);

            var response = tweetService.GetByUserId("jishnu123");


        }

        [Test]
        public void UpdateTweetTest()
        {
            var tweetList = new List<Tweet>() { };

            tweetList.Add(new Tweet()
            {
                TweetId = new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709").ToString(),
                CreatorId = "jishnu123",
                Content = "New Content",
                Tags = new List<string>(),
                Likes = new List<string>(),
                Replys = new List<TweetReply>()
            }); ;

            Mock<IAsyncCursor<Tweet>> _tweetCursor = new Mock<IAsyncCursor<Tweet>>();

            //mock movenext
            _tweetCursor.Setup(_ => _.Current).Returns(tweetList);
            _tweetCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _tweet.Setup(op => op.FindAsync<Tweet>(It.IsAny<FilterDefinition<Tweet>>(),
                            It.IsAny<FindOptions<Tweet, Tweet>>(),
                            It.IsAny<CancellationToken>())).ReturnsAsync(_tweetCursor.Object);

            _guid.Setup(x => x.NewGuid()).Returns(new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709"));

            _Client.Setup(x => x.GetTweetCollection()).Returns(_tweet.Object);

            var tweetService = new TweetService(_Client.Object, _guid.Object);

            var response = tweetService.UpdateTweet("9D2B0228-4D0D-4C23-8B49-01A698857709", new Tweet()
            {
                TweetId = new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709").ToString(),
                CreatorId = "jishnu123",
                Content = "New Content",
                Tags = new List<string>(),
                Likes = new List<string>(),
                Replys = new List<TweetReply>()
            });

            response.Result.TweetId.Should().Be(new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709").ToString());

        }

        [Test]
        public void LikeTweetTest()
        {
            var tweetList = new List<Tweet>() { };

            tweetList.Add(new Tweet()
            {
                TweetId = new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709").ToString(),
                CreatorId = "jishnu123",
                Content = "New Content",
                Tags = new List<string>(),
                Likes = new List<string>(),
                Replys = new List<TweetReply>()
            }); ;

            Mock<IAsyncCursor<Tweet>> _tweetCursor = new Mock<IAsyncCursor<Tweet>>();

            //mock movenext
            _tweetCursor.Setup(_ => _.Current).Returns(tweetList);
            _tweetCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _tweet.Setup(op => op.FindAsync<Tweet>(It.IsAny<FilterDefinition<Tweet>>(),
                            It.IsAny<FindOptions<Tweet, Tweet>>(),
                            It.IsAny<CancellationToken>())).ReturnsAsync(_tweetCursor.Object);

            _guid.Setup(x => x.NewGuid()).Returns(new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709"));

            _Client.Setup(x => x.GetTweetCollection()).Returns(_tweet.Object);

            var tweetService = new TweetService(_Client.Object, _guid.Object);

            var response = tweetService.LikeTweet( "jishnu@gmail.com", "9D2B0228-4D0D-4C23-8B49-01A698857709");

            response.Exception.Should().BeNull();
        }

    }
}
