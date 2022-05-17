using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.Model;

namespace Tweet_App_API.Services
{
    public interface ITweetService
    {
        public Task<Tweet> PostTweet(Tweet tweet);

        public List<Tweet> GetAll();

        public Task<List<Tweet>> GetByUserId(string id);

        public Task<Tweet> UpdateTweet(string userid, Tweet tweet);

        public  Task<DeleteResult> DeleteTweet(string tweetid);

        public  Task<Tweet> LikeTweet(string userId, string tweetId);

        public Task<Tweet> ReplyTweet(string userId, string tweetId, TweetReply replyTweet);

     //   public Tweet GetTweet();
    }
}
