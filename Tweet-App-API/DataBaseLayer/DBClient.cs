using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.Model;

namespace Tweet_App_API.DataBaseLayer
{
    public class DBClient : IDBClient
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Tweet> _tweets;
        private readonly IMongoClient client;

        public DBClient(ITweetAppDBSettings settings, IMongoClient client)
        {
            this.client = client;

            var database = client.GetDatabase(settings.DatabaseName);

            _users = GetDBCollection<User>(database, settings.UserCollectionName);

            _tweets = GetDBCollection<Tweet>(database, settings.TweetCollectionName);

           

            //_users.Indexes.CreateOne("{ title : 1 }", options);

        }

        public IMongoCollection<User> GetUserCollection()
        {
            return _users;
        }

        public IMongoCollection<Tweet> GetTweetCollection()
        {
            return _tweets;
        }

        public static IMongoCollection<T> GetDBCollection<T>(IMongoDatabase database,string collectionName)
        {
            var collectionList = database.ListCollectionNames()?.ToList();

            var isExisit = collectionList.Any(x => x == collectionName);

            if (!isExisit)
            {
                database.CreateCollection(collectionName);
            }

            return database.GetCollection<T>(collectionName);
        }

      
    }
}
