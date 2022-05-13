using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.Model;

namespace Tweet_App_API.DataBaseLayer
{
    public interface IDBClient
    {
        public IMongoCollection<User> GetUserCollection();

        public IMongoCollection<Tweet> GetTweetCollection();
    }
}
