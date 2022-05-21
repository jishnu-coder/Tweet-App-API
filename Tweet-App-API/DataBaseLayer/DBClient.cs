using MongoDB.Driver;
using System.Linq;
using Tweet_App_API.Model;

namespace Tweet_App_API.DataBaseLayer
{
    public class DBClient : IDBClient
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Tweet> _tweets;


        public DBClient(ITweetAppDBSettings settings, IMongoClient client)
        {

            var database = client.GetDatabase(settings.DatabaseName);

            _users = GetDBCollection<User>(database, settings.UserCollectionName);

            _tweets = GetDBCollection<Tweet>(database, settings.TweetCollectionName);

        }

        public IMongoCollection<User> GetUserCollection()
        {
            return _users;
        }

        public IMongoCollection<Tweet> GetTweetCollection()
        {
            return _tweets;
        }

        public static IMongoCollection<T> GetDBCollection<T>(IMongoDatabase database, string collectionName)
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
