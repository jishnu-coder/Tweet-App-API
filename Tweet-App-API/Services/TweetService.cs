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
            _tweet.InsertOne(tweet);
            return tweet;
        }
    }
}
