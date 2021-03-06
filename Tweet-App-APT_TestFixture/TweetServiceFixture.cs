using FluentAssertions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tweet_App_API.DataBaseLayer;
using Tweet_App_API.Kafka;
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
        Mock<IUserServices> _userService;
        Mock<IKafkaProducer> _kafkaProducer;



        [SetUp]
        public void Setup()
        {
            _tweet = new Mock<IMongoCollection<Tweet>>();
            _guid = new Mock<IGuidService>();
            _Client = new Mock<IDBClient>();
            _mongoQueryableMock = new Mock<IMongoQueryable<Tweet>>();
            _userService = new Mock<IUserServices>();
            _kafkaProducer = new Mock<IKafkaProducer>();

            _kafkaProducer.Setup(x => x.KafkaProducerConfig(It.IsAny<Tweet>())).ReturnsAsync(true);

        }

        [Test]
        public void PostTweetTest()
        {
            var tweetList = new List<Tweet>() { };

            tweetList.Add(new Tweet()
            {
                TweetId = new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709").ToString(),
                Creator = new Creator { CreatorId = "jishnu123" },
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
            _userService.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new UserViewModel() { });

            _guid.Setup(x => x.NewGuid()).Returns(new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709"));

            _Client.Setup(x => x.GetTweetCollection()).Returns(_tweet.Object);

            var tweetService = new TweetService(_Client.Object, _guid.Object, _userService.Object, _kafkaProducer.Object);

            var response = tweetService.PostTweet(new Tweet() { Content = "New Content", Creator = new Creator() { CreatorId = "jishnu123" } });

            response.Result.TweetId.Should().Be(new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709").ToString());

        }

        [Test]
        public void GetByUserId()
        {
            var tweetList = new List<Tweet>() { };

            tweetList.Add(new Tweet()
            {
                TweetId = new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709").ToString(),
                Creator = new Creator { CreatorId = "jishnu123" },
                Content = "New Content",
                Tags = new List<string>(),
                Likes = new List<string>(),
                Replys = new List<TweetReply>()
            }); ;

            Mock<IAsyncCursor<Tweet>> _tweetCursor = new Mock<IAsyncCursor<Tweet>>();

            //mock movenext
            _tweetCursor.Setup(_ => _.Current).Returns(tweetList);
           

            _tweet.Setup(op => op.FindAsync<Tweet>(It.IsAny<FilterDefinition<Tweet>>(),
                            It.IsAny<FindOptions<Tweet, Tweet>>(),
                            It.IsAny<CancellationToken>())).ReturnsAsync(_tweetCursor.Object);



            _guid.Setup(x => x.NewGuid()).Returns(new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709"));
            _userService.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new UserViewModel() { });

            _Client.Setup(x => x.GetTweetCollection()).Returns(_tweet.Object);

            var tweetService = new TweetService(_Client.Object, _guid.Object, _userService.Object, _kafkaProducer.Object);

            var response = tweetService.GetTweetsByUserId("jishnu123");

            response.Exception.Should().BeNull();
        }

        [Test]
        public void UpdateTweetTest()
        {
            var tweetList = new List<Tweet>() { };

            tweetList.Add(new Tweet()
            {
                TweetId = new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709").ToString(),
                Creator = new Creator { CreatorId = "jishnu123" },
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

            _userService.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new UserViewModel() { });

            _Client.Setup(x => x.GetTweetCollection()).Returns(_tweet.Object);

            var tweetService = new TweetService(_Client.Object, _guid.Object, _userService.Object, _kafkaProducer.Object);

            var response = tweetService.UpdateTweet("test", "9D2B0228-4D0D-4C23-8B49-01A698857709", new Tweet()
            {
                TweetId = new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709").ToString(),
                Creator = new Creator { CreatorId = "jishnu123" },
                Content = "New Content",
                Tags = new List<string>(),
                Likes = new List<string>(),
                Replys = new List<TweetReply>()
            });

            response.Exception.Should().BeNull();

        }

        [Test]
        public void LikeTweetTest()
        {
            var tweetList = new List<Tweet>() { };

            tweetList.Add(new Tweet()
            {
                TweetId = new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709").ToString(),
                Creator = new Creator { CreatorId = "jishnu123" },
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

            var tweetService = new TweetService(_Client.Object, _guid.Object, _userService.Object, _kafkaProducer.Object);

            var response = tweetService.LikeTweet("jishnu@gmail.com", "9D2B0228-4D0D-4C23-8B49-01A698857709");

            response.Exception.Should().BeNull();
        }

        [Test]
        public void ReplyTweetTest()
        {
            var tweetList = new List<Tweet>() { };

            tweetList.Add(new Tweet()
            {
                TweetId = new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709").ToString(),
                Creator = new Creator { CreatorId = "jishnu123" },
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

            _userService.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(new UserViewModel() { });

            _Client.Setup(x => x.GetTweetCollection()).Returns(_tweet.Object);

            var tweetService = new TweetService(_Client.Object, _guid.Object, _userService.Object, _kafkaProducer.Object);

            var response = tweetService.ReplyTweet("jishnu@gmail.com", "9D2B0228-4D0D-4C23-8B49-01A698857709", new TweetReply() { Replied_userId = "jishnu@gmail.com", ReplyMessage = "Super..." });

            response.Exception.Should().BeNull();
        }

        [Test]
        public void DeleteTweetTest()
        {
            var deleteResult = new Mock<DeleteResult>();
            _tweet.Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Tweet>>(), It.IsAny<CancellationToken>())).ReturnsAsync(deleteResult.Object);
            _Client.Setup(x => x.GetTweetCollection()).Returns(_tweet.Object);

            var tweetService = new TweetService(_Client.Object, _guid.Object, _userService.Object, _kafkaProducer.Object);

            var result = tweetService.DeleteTweet("12344");

            result.Exception.Should().BeNull();
        }

        [Test]
        public void IsValidUserTestWithException()
        {
            var tweetService = new TweetService(_Client.Object, _guid.Object, _userService.Object, _kafkaProducer.Object);
            var result = tweetService.isValidUser("test");
            result.Exception.Should().NotBeNull();

        }

        [Test]
        public void IsValidTweetTestWithException()
        {
            var tweetService = new TweetService(_Client.Object, _guid.Object, _userService.Object, _kafkaProducer.Object);
            var result = tweetService.isValidTweet("1234");
            result.Exception.Should().NotBeNull();

        }

        [Test]
        public void TweetTimeStampTest()
        {
            var result1 = TweetService.TweetTimeStamp(DateTime.Now);

            result1.Should().Be("Just Now");

            var result2 = TweetService.TweetTimeStamp(DateTime.Now.AddMinutes(-30));

            result2.Should().Be("30 Minutes ago");

            var result3 = TweetService.TweetTimeStamp(DateTime.Now.AddMinutes(-65));

            result3.Should().Be("1 Hours ago");

            var result4 = TweetService.TweetTimeStamp(DateTime.Now.AddDays(-1));

            result4.Should().Be("1 Days ago");


        }

        [Test]
        public void ReformTweetObjectTest()
        {
            var result = TweetService.ReformTweetObject(new List<Tweet>() { new Tweet() {
                      CreateTime=DateTime.UtcNow,
                      Replys=new List<TweetReply>(){
                        new TweetReply()
                        {
                            Reply_Time=DateTime.UtcNow
                        }
                      }
            } });

            result.Should().HaveCount(1);


        }

        [Test]
        public void TweetLengthAndTagLengthValidationFixtureWithException()
        {
            try
            {
                string content = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                var tags = new List<string>() { "Test tag" };

                var result = TweetService.TweetLengthAndTagLengthValidation(content, tags);
            }
            catch(Exception ex)
            {
                ex.Message.Should().Be("Tweet length should be less than 50 charactor");
            }

            

            
        }

        [Test]
        public void isValidTweetFixtureWithException()
        {
            var tweetList = new List<Tweet>() { };

           

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
            _Client.Setup(x => x.GetTweetCollection()).Returns(_tweet.Object);


            var tweetService = new TweetService(_Client.Object, _guid.Object, _userService.Object, _kafkaProducer.Object);

            var result = tweetService.isValidTweet("1234");

            result.Exception.Should().NotBeNull();
         
        }
    }

   
}

