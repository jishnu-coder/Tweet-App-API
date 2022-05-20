﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.DataBaseLayer;
using Tweet_App_API.Model;
using Tweet_App_API.TokenHandler;

namespace Tweet_App_API.Services
{
    public class UserServices : IUserServices
    {
        private readonly IMongoCollection<User> _users;
        private readonly IJwtAuthenticationManager jwtAuthenticationManager;
      
        public UserServices(IDBClient client, IJwtAuthenticationManager jwtAuthenticationManager)
        {
            _users = client.GetUserCollection();
            this.jwtAuthenticationManager = jwtAuthenticationManager;
        }

        public async Task<List<User>> Get()
        {
            var users = await _users.FindAsync(usr => true);
            return users?.ToList();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _users.FindAsync<User>(emp => emp.Email.Equals(email));

            return await user.FirstOrDefaultAsync();
        }


        public async Task<UserResponse> Register(User usr)
        {
            usr.LoginId = usr.FirstName + Guid.NewGuid().ToString();
            var responsse = new UserResponse() { Email = usr.Email, LoginId = usr.LoginId, Errors = new List<string>() };

            try
            {
                //Hash the password
                usr.Password = CryptoGraphy.GetHash(usr.Password);
                await _users.InsertOneAsync(usr);
                var tokenContainer = jwtAuthenticationManager.Authenticate(usr.Email, usr.Password);
                responsse.Token = tokenContainer.Token;
                responsse.RefreshToken = tokenContainer.RefreshToken;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("customLoginId"))
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

        public async Task<UserResponse> LoginUser(string email, string password)
        {
            var user = await _users.FindAsync<User>(emp => emp.Email == email).Result.FirstOrDefaultAsync();

            var userResponse = new UserResponse() { Errors = new List<string>() };

            if (user == null)
            {
                userResponse.Errors.Add("Invalid Log in ID");
                return userResponse;
            }

            userResponse.Email = user.Email;
            userResponse.LoginId = user.LoginId;

            var newHashValue = CryptoGraphy.GetHash(password);

            //Check the password match or Not

            if (CryptoGraphy.CompareHash(newHashValue, user.Password))
            {
                var tokenResponse = jwtAuthenticationManager.Authenticate(user.Email, user.Password);
                userResponse.Token = tokenResponse.Token;
                userResponse.RefreshToken = tokenResponse.RefreshToken;
                return userResponse;
            }

            userResponse.Errors.Add("Incorrect Pssword");
            return userResponse;
        }

        public bool ResetPassword(string email, string newPassword)
        {
            var user = GetUserByEmail(email);
            if (user != null)
            {
                var hashPassword = CryptoGraphy.GetHash(newPassword);
                var filter = new BsonDocument("email", email);
                var update = Builders<User>.Update.Set("password", hashPassword);
                _users.FindOneAndUpdate(filter, update);
                return true;
            }

            return false;
        }
    }
}

