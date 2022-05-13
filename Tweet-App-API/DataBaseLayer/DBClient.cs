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

        public DBClient(ITweetAppDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            var collectionList = database.ListCollectionNames().ToList();

            var isExisit = collectionList.Any(x => x == settings.UserCollectionName);

            if (!isExisit)
            {
                database.CreateCollection(settings.UserCollectionName);
            }
             
            
            
            _users = database.GetCollection<User>(settings.UserCollectionName);

            var options = new CreateIndexOptions { Unique = true };

            //_users.Indexes.CreateOne("{ title : 1 }", options);
            
        }

        public IMongoCollection<User> GetUserCollection()
        {
            return _users;
        }
    }
}
