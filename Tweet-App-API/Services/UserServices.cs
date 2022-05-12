using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.Model;

namespace Tweet_App_API.Services
{
    public class UserServices
    {
        private readonly IMongoCollection<User> _users;
        public UserServices(ITweetAppDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.BooksCollectionName);

        }

        public List<User> Get()
        {
            List<User> users;
            users = _users.Find(usr => true).ToList();
            return users;
        }

        public User Get(string id) =>
            _users.Find<User>(emp => emp.LoginId == id).FirstOrDefault();

        public User Post(User usr)
        {
            _users.InsertOne(usr);
            return usr;
        }

    }
}

