using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.DataBaseLayer;
using Tweet_App_API.Exceptions;
using Tweet_App_API.Kafka;
using Tweet_App_API.Model;

namespace Tweet_App_API.Services
{
    public class TweetService : ITweetService
    {
        private readonly IMongoCollection<Tweet> _tweet;
        private readonly IGuidService guidService;
        private readonly IUserServices userServices;
        private readonly IKafkaProducer _kafkaProducer;

        public TweetService(IDBClient client, IGuidService guidService, IUserServices userServices , IKafkaProducer kafkaProducer)
        {
            _tweet = client.GetTweetCollection();
            this.guidService = guidService;
            this.userServices = userServices;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<Tweet> PostTweet(Tweet tweet)
        {
            var isAValidCreator = await userServices.GetUserByEmail(tweet.Creator.CreatorId);
            if (isAValidCreator == null)
            {
                throw new InvalidUserNameException("Not a Valid creator");
            }

            tweet.Tags = tweet.Tags == null ? new List<string>() : tweet.Tags;
            tweet.Creator.Seq = isAValidCreator.Seq;
            if (TweetLengthAndTagLengthValidation(tweet.Content, tweet.Tags))
            {
                //Add Guid as tweet id
                tweet.TweetId = guidService.NewGuid().ToString();
                tweet.CreateTime = DateTime.Now;

                tweet.Likes = tweet.Likes == null ? new List<string>() { } : tweet.Likes;

                tweet.Replys = tweet.Replys == null ? new List<TweetReply>() { } : tweet.Replys;

                await _tweet.InsertOneAsync(tweet);

                await _kafkaProducer.KafkaProducerConfig(new Tweet() { Content = tweet.Content });
            }

            return await GetTweetByTweetId(tweet.TweetId);
        }

        public List<Tweet> GetAll()
        {
           
            var tweetList = _tweet.AsQueryable().OrderByDescending(x => x.CreateTime).ToList();

            tweetList = tweetList.Any() ? ReformTweetObject(tweetList) : tweetList;

            return tweetList;
        }

        public async Task<List<Tweet>> GetTweetsByUserId(string userName)
        {
            //Check the userName is valid , if not throw exception
            var isAValidCreator = await userServices.GetUserByEmail(userName);
            if (isAValidCreator == null)
            {
                throw new InvalidUserNameException("Not a Valid creator");
            }
            var result = await _tweet.FindAsync(x => x.Creator.CreatorId == userName);

            var data = result.ToList();

            data = data.OrderByDescending(x => x.CreateTime).ToList();

            data = data.Any() ? ReformTweetObject(data) : data;

          
            return data;

        }

        public async Task<Tweet> UpdateTweet(string userName, string tweetid, Tweet tweet)
        {
            tweet.Tags = tweet.Tags == null ? new List<string>() { } : tweet.Tags;
            //Check if the userName and  tweet id are valid 
            if (await isValidUser(tweet.Creator.CreatorId) && await isValidTweet(tweetid) && TweetLengthAndTagLengthValidation(tweet.Content, tweet.Tags))
            {
                tweet.CreateTime = DateTime.Now;
                var filter = new BsonDocument("tweetId", tweet.TweetId);
                var update = Builders<Tweet>.Update.Set("content", tweet.Content).
                           Set("tags", tweet.Tags).Set("createTime", tweet.CreateTime);
                await _tweet.FindOneAndUpdateAsync<Tweet>(filter, update);

                await _kafkaProducer.KafkaProducerConfig(new Tweet() { Content = $"{userName} updat the tweet with content {tweet.Content}" });
            }

            var updatedTweet = await GetTweetByTweetId(tweet.TweetId);

            return updatedTweet;
        }

        public async Task<DeleteResult> DeleteTweet(string tweetid)
        {

            var result = await _tweet.DeleteOneAsync<Tweet>(x => x.TweetId == tweetid);

            await _kafkaProducer.KafkaProducerConfig(new Tweet() { Content = $"Delete the tweet {tweetid}" });
            return result;

        }

        public async Task<Tweet> GetTweetByTweetId(string id)
        {
            var result = await _tweet.FindAsync(x => x.TweetId == id);

            return await result.FirstOrDefaultAsync();
        }

        public async Task<Tweet> LikeTweet(string userId, string tweetId)
        {
            var tweet = await GetTweetByTweetId(tweetId);
            //Check the tweet id is Valid and if the user already liked bypass the flow
            if (tweet != null && !tweet.Likes.Contains(userId))
            {
                var filter = new BsonDocument("tweetId", tweetId);
                var update = Builders<Tweet>.Update.AddToSet("likes", userId);

                await _tweet.FindOneAndUpdateAsync<Tweet>(filter, update);

                await _kafkaProducer.KafkaProducerConfig(new Tweet { Content = $"{userId} Like the tweet {tweetId}" });

                return await GetTweetByTweetId(tweetId);
            }

            return tweet;

        }

        public async Task<Tweet> ReplyTweet(string userId, string tweetId, TweetReply replyTweet)
        {
            if (await isValidUser(userId) && await isValidTweet(tweetId)
                && TweetLengthAndTagLengthValidation(replyTweet.ReplyMessage, new List<string>()))
            {
                replyTweet.Reply_Time = DateTime.Now;
                replyTweet.Replied_userId = userId;
                var filter = new BsonDocument("tweetId", tweetId);
                var update = Builders<Tweet>.Update.AddToSet("replys", replyTweet);

                await _tweet.FindOneAndUpdateAsync<Tweet>(filter, update);

                await _kafkaProducer.KafkaProducerConfig( new Tweet() { Content = $"{userId} Reply the tweet {tweetId} with Reply message {replyTweet.ReplyMessage}" });

            }
            return await GetTweetByTweetId(tweetId);
        }

        public async Task<bool> isValidUser(string userName)
        {
            var user = await userServices.GetUserByEmail(userName);

            if (user != null)
                return true;

            throw new InvalidUserNameException("Invalid user Id");

        }

        public async Task<bool> isValidTweet(string tweetId)
        {
            var tweet = await GetTweetByTweetId(tweetId);

            if (tweet != null)
                return true;

            throw new InvalidTweetIdException("Invalid tweet Id");

        }

        public static bool TweetLengthAndTagLengthValidation(string content, List<string> tags)
        {
            foreach (var tag in tags)
            {
                if (tag.Length > 50)
                {
                    throw new TweetLengthExceedException("Tag length should be less than 50 charactor");
                }
            }

            if (content.Length > 144)
            {
                throw new TweetLengthExceedException("Tweet length should be less than 50 charactor");
            }

            return true;
        }

        public static List<Tweet> ReformTweetObject(List<Tweet> data)
        {
            foreach (var tweet in data)
            {
                tweet.DateTimeStamp = TweetTimeStamp(TimeZoneInfo.ConvertTimeFromUtc(tweet.CreateTime, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")));

                tweet.Replys = tweet.Replys.OrderByDescending(x => x.Reply_Time).ToList();

                foreach (var reply in tweet.Replys)
                {
                    reply.Reply_Time_Stamp = TweetTimeStamp(TimeZoneInfo.ConvertTimeFromUtc(reply.Reply_Time, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")));
                }


            }

            return data;
        }

        public static string TweetTimeStamp (DateTime createTime)
        {
            TimeSpan ts = DateTime.Now - createTime;

            var minutes = ts.TotalMinutes;

            if(minutes < 1)
            {
                return "Just Now";
            }

            if(minutes < 60)
            {
                return $"{(int)minutes} Minutes ago";
            }

            if(minutes < 1440)
            {
                var hours = (int)minutes / 60;

                return $"{hours} Hours ago";
            }

            return $"{(int)minutes / 1440} Days ago";
            
            
        }
    }
}
