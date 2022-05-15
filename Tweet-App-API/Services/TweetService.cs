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

        public TweetService(IDBClient client)
        {
            _tweet = client.GetTweetCollection();
        }

        public Tweet PostTweet(Tweet tweet)
        {
            //Add Guid as tweet id
            tweet.TweetId = Guid.NewGuid().ToString();
            tweet.CreateTime = DateTime.Now;

            tweet.Likes = tweet.Likes == null ? new List<string>() { } : tweet.Likes;
            tweet.Tags = tweet.Tags == null ? new List<string>() { } : tweet.Tags;
            tweet.Replys = tweet.Replys == null ? new List<TweetReply>() { } : tweet.Replys;

            _tweet.InsertOne(tweet);
            return tweet;
        }

        public List<Tweet> GetAll()
        {
            return _tweet.AsQueryable().ToList();
        }

        public List<Tweet> GetByUserId(string id)
        {
            return _tweet.Find(x => x.CreatorId == id).ToList();
        }

        public async  Task<Tweet> UpdateTweet(string tweetid, Tweet tweet)
        {
            tweet.CreateTime = DateTime.Now;
            var filter = new BsonDocument("tweetId", tweet.TweetId);
            var update = Builders<Tweet>.Update.Set("content", tweet.Content).
                       Set("tags", tweet.Tags).Set("createTime", tweet.CreateTime);
            return await _tweet.FindOneAndUpdateAsync<Tweet>(filter, update);
        }

        public async Task<DeleteResult> DeleteTweet(string tweetid)
        {
          
            var result = await _tweet.DeleteOneAsync<Tweet>( x => x.TweetId ==  tweetid);
            return result;
           
        }

        public Tweet GetTweetByTweetId(string id)
        {
            return _tweet.Find(x => x.TweetId == id).FirstOrDefault();
        }

        public async Task<Tweet> LikeTweet(string userId,string tweetId)
        {
            var tweet = GetTweetByTweetId(tweetId);
            if(!tweet.Likes.Contains(userId))
            {
                var filter = new BsonDocument("tweetId", tweetId);
                var update = Builders<Tweet>.Update.AddToSet("likes", userId);

                return await _tweet.FindOneAndUpdateAsync<Tweet>(filter, update);
            }

            return tweet;
           
        }

        public async Task<Tweet> ReplyTweet(string userId, string tweetId , TweetReply replyTweet)
        {
            replyTweet.Reply_Time = DateTime.Now;
            var filter = new BsonDocument("tweetId", tweetId);
            var update = Builders<Tweet>.Update.AddToSet("replys", replyTweet);

            return await _tweet.FindOneAndUpdateAsync<Tweet>(filter, update);
        }
    }
}
