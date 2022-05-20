using MongoDB.Driver;
using Tweet_App_API.Model;

namespace Tweet_App_API.DataBaseLayer
{
    public interface IDBClient
    {
        public IMongoCollection<User> GetUserCollection();

        public IMongoCollection<Tweet> GetTweetCollection();
    }
}
