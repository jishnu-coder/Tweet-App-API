using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.DataBaseLayer;
using Tweet_App_API.Model;

namespace Tweet_App_API.Services
{
    public class UserServices : IUserServices
    {
        private readonly IMongoCollection<User> _users;
        //private readonly IMongoCollection<Test> _test;
        public UserServices(IDBClient client)
        {
            _users = client.GetUserCollection();

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
            //Hash the password
            usr.Password = CryptoGraphy.GetHash(usr.Password);
            _users.InsertOne(usr);
            return usr;
        }

        public User LoginUser(string loginId,string password)
        {
            var user =_users.Find<User>(emp => emp.LoginId == loginId).FirstOrDefault();

            var newHashValue = CryptoGraphy.GetHash(password);

            //Check the password match or Not

            if(CryptoGraphy.CompareHash(newHashValue, user.Password))
            {
                return user;
            }
            return null;
        }

    }
}

