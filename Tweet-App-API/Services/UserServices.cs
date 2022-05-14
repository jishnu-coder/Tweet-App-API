using MongoDB.Bson;
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

        public List<User> GetUserById(string id) =>
            _users.Find<User>(emp => emp.LoginId.Contains(id)).ToList();

        public async Task<UserResponse> Register(User usr)
        {
            var responsse = new UserResponse() {Email=usr.Email, LoginId = usr.LoginId , Errors=new List<string>() };
           
            try
            {
                //Hash the password
                usr.Password = CryptoGraphy.GetHash(usr.Password);
                await _users.InsertOneAsync(usr);
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("customLoginId"))
                {
                    responsse.Errors.Add("Login ID is already exisit");
                }

                if (ex.Message.Contains("customEmail"))
                {
                    responsse.Errors.Add("Email is already exisit");
                }

            }

            return responsse;
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

        public User ResetPassword(string userId, string newPassword)
        {
            var user = GetUserById(userId);
            if (user[0] != null)
            {
                var hashPassword = CryptoGraphy.GetHash(newPassword);
                var filter = new BsonDocument("loginId", userId);
                var update = Builders<User>.Update.Set("password", hashPassword);
                var result = _users.FindOneAndUpdate(filter, update);
                return user[0];
            }

            return null;
        }
    }
}

