using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tweet_App_API.Model;

namespace Tweet_App_API.Services
{
    public interface ITweetService
    {
        public Task<Tweet> PostTweet(Tweet tweet);

        public List<Tweet> GetAll();

        public Task<List<Tweet>> GetTweetsByUserId(string userName);

        public Task<Tweet> UpdateTweet(string userName, string tweetid, Tweet tweet);

        public Task<DeleteResult> DeleteTweet(string tweetid);

        public Task<Tweet> LikeTweet(string userId, string tweetId);

        public Task<Tweet> ReplyTweet(string userId, string tweetId, TweetReply replyTweet);

    }
}
