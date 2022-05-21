using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.DataBaseLayer;
using Tweet_App_API.Model;

namespace Tweet_App_API.Services
{
    public class TweetService : ITweetService
    {
        private readonly IMongoCollection<Tweet> _tweet;
        private readonly IGuidService guidService;
        private readonly IUserServices userServices;

        public TweetService(IDBClient client, IGuidService guidService , IUserServices userServices)
        {
            _tweet = client.GetTweetCollection();
            this.guidService = guidService;
            this.userServices = userServices;
        }

        public async Task<Tweet> PostTweet(Tweet tweet)
        {
            var isAValidCreator = await userServices.GetUserByEmail(tweet.CreatorId);
            if(isAValidCreator == null)
            {
                throw new Exception("Not a Valid creator");
            }
            //Add Guid as tweet id
            tweet.TweetId = guidService.NewGuid().ToString();
            tweet.CreateTime = DateTime.Now;

            tweet.Likes = tweet.Likes == null ? new List<string>() { } : tweet.Likes;
            tweet.Tags = tweet.Tags == null ? new List<string>() { } : tweet.Tags;
            tweet.Replys = tweet.Replys == null ? new List<TweetReply>() { } : tweet.Replys;

            await _tweet.InsertOneAsync(tweet);
            return await GetTweetByTweetId(tweet.TweetId);
        }

        public List<Tweet> GetAll()
        {
            return _tweet.AsQueryable().ToList();
        }

        public async Task<List<Tweet>> GetTweetsByUserId(string userName)
        {
            var isAValidCreator = await userServices.GetUserByEmail(userName);
            if (isAValidCreator == null)
            {
                throw new Exception("Not a Valid creator");
            }
            var result = await _tweet.FindAsync(x => x.CreatorId == userName);
            return result.ToList();
        }

        public async Task<Tweet> UpdateTweet(string username,string tweetid, Tweet tweet)
        {
            if (await isValidUser(username) && await isValidTweet(tweetid))
            {

                tweet.CreateTime = DateTime.Now;
                tweet.Tags = tweet.Tags == null ? new List<string>() { } : tweet.Tags;
                var filter = new BsonDocument("tweetId", tweet.TweetId);
                var update = Builders<Tweet>.Update.Set("content", tweet.Content).
                           Set("tags", tweet.Tags).Set("createTime", tweet.CreateTime);
                await _tweet.FindOneAndUpdateAsync<Tweet>(filter, update);
            }

            var data = await GetTweetByTweetId(tweet.TweetId);

            return data;
        }

        public async Task<DeleteResult> DeleteTweet(string tweetid)
        {

            var result = await _tweet.DeleteOneAsync<Tweet>(x => x.TweetId == tweetid);
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
            if ( tweet != null && !tweet.Likes.Contains(userId))
            {
                var filter = new BsonDocument("tweetId", tweetId);
                var update = Builders<Tweet>.Update.AddToSet("likes", userId);

                await _tweet.FindOneAndUpdateAsync<Tweet>(filter, update);

                return await GetTweetByTweetId(tweetId);
            }

            return tweet;

        }

        public async Task<Tweet> ReplyTweet(string userId, string tweetId, TweetReply replyTweet)
        {
            if (await isValidUser(userId) && await isValidTweet(tweetId))
            {

                replyTweet.Reply_Time = DateTime.Now;
                replyTweet.Replied_userId = userId;
                var filter = new BsonDocument("tweetId", tweetId);
                var update = Builders<Tweet>.Update.AddToSet("replys", replyTweet);

                await _tweet.FindOneAndUpdateAsync<Tweet>(filter, update);
               
            }
            return await GetTweetByTweetId(tweetId);
        }

        public async Task<bool> isValidUser(string userName)
        {
            var user = await userServices.GetUserByEmail(userName);

            if (user != null)
                return true;

            throw new Exception("Invalid user Id");

        }

        public async Task<bool> isValidTweet(string tweetId)
        {
            var tweet = await GetTweetByTweetId(tweetId);

            if (tweet != null)
                return true;

            throw new Exception("Invalid tweet Id");

        }
    }
}
