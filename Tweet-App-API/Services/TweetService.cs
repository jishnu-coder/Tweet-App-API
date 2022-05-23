using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.DataBaseLayer;
using Tweet_App_API.Exceptions;
using Tweet_App_API.Model;

namespace Tweet_App_API.Services
{
    public class TweetService : ITweetService
    {
        private readonly IMongoCollection<Tweet> _tweet;
        private readonly IGuidService guidService;
        private readonly IUserServices userServices;

        public TweetService(IDBClient client, IGuidService guidService, IUserServices userServices)
        {
            _tweet = client.GetTweetCollection();
            this.guidService = guidService;
            this.userServices = userServices;
        }

        public async Task<Tweet> PostTweet(Tweet tweet)
        {
            var isAValidCreator = await userServices.GetUserByEmail(tweet.CreatorId);
            if (isAValidCreator == null)
            {
                throw new InvalidUserNameException("Not a Valid creator");
            }

            tweet.Tags = tweet.Tags == null ? new List<string>() : tweet.Tags;

            if(TweetLengthAndTagLengthValidation(tweet.Content,tweet.Tags))
            {
                //Add Guid as tweet id
                tweet.TweetId = guidService.NewGuid().ToString();
                tweet.CreateTime = DateTime.Now;

                tweet.Likes = tweet.Likes == null ? new List<string>() { } : tweet.Likes;
               
                tweet.Replys = tweet.Replys == null ? new List<TweetReply>() { } : tweet.Replys;

                await _tweet.InsertOneAsync(tweet);
            }
            
            return await GetTweetByTweetId(tweet.TweetId);
        }

        public List<Tweet> GetAll()
        {
            return _tweet.AsQueryable().ToList();
        }

        public async Task<List<Tweet>> GetTweetsByUserId(string userName)
        {
            //Check the userName is valid , if not throw exception
            var isAValidCreator = await userServices.GetUserByEmail(userName);
            if (isAValidCreator == null)
            {
                throw new InvalidUserNameException("Not a Valid creator");
            }
            var result = await _tweet.FindAsync(x => x.CreatorId == userName);
            return result.ToList();
        }

        public async Task<Tweet> UpdateTweet(string userName, string tweetid, Tweet tweet)
        {
            tweet.Tags = tweet.Tags == null ? new List<string>() { } : tweet.Tags;
            //Check if the userName and  tweet id are valid 
            if (await isValidUser(tweet.CreatorId) && await isValidTweet(tweetid) && TweetLengthAndTagLengthValidation(tweet.Content, tweet.Tags))
            {
                tweet.CreateTime = DateTime.Now;                
                var filter = new BsonDocument("tweetId", tweet.TweetId);
                var update = Builders<Tweet>.Update.Set("content", tweet.Content).
                           Set("tags", tweet.Tags).Set("createTime", tweet.CreateTime);
                await _tweet.FindOneAndUpdateAsync<Tweet>(filter, update);
            }

            var updatedTweet = await GetTweetByTweetId(tweet.TweetId);

            return updatedTweet;
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
            //Check the tweet id is Valid and if the user already liked bypass the flow
            if (tweet != null && !tweet.Likes.Contains(userId))
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
            if (await isValidUser(userId) && await isValidTweet(tweetId)
                && TweetLengthAndTagLengthValidation(replyTweet.ReplyMessage, new List<string>()))
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

            throw new InvalidUserNameException("Invalid user Id");

        }

        public async Task<bool> isValidTweet(string tweetId)
        {
            var tweet = await GetTweetByTweetId(tweetId);

            if (tweet != null)
                return true;

            throw new InvalidTweetIdException("Invalid tweet Id");

        }

        private bool TweetLengthAndTagLengthValidation(string content,List<string> tags)
        {
           foreach(var tag in tags)
            {
                if(tag.Length > 50)
                {
                    throw new TweetLengthExceedException("Tag length should be less that 50 charactor");
                }
            }

           if(content.Length > 144)
            {
                throw new TweetLengthExceedException("Tweet length should be less that 50 charactor");
            }

            return true;
        }
    }
}
