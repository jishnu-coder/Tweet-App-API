using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.DataBaseLayer;
using Tweet_App_API.Kafka;
using Tweet_App_API.Model;
using Tweet_App_API.TokenHandler;

namespace Tweet_App_API.Services
{
    public class UserServices : IUserServices
    {
        private readonly IMongoCollection<User> _users;
        private readonly IJwtAuthenticationManager jwtAuthenticationManager;
        private readonly IMapper _mapper;

        public UserServices(IDBClient client, IJwtAuthenticationManager jwtAuthenticationManager, IMapper mapper)
        {
            _users = client.GetUserCollection();
            this.jwtAuthenticationManager = jwtAuthenticationManager;
            _mapper = mapper;
        }

        private async Task<UserViewModel> GetUserByConatctAndEmail(string email, string conatctNumber)
        {
            var user = await _users.FindAsync<User>(emp => emp.Email.Equals(email) && emp.ContactNumber.Equals(conatctNumber));

            var result = await user?.FirstOrDefaultAsync();

            UserViewModel userViewModel = _mapper.Map<UserViewModel>(result);

            return userViewModel;
        }

        public async Task<List<UserViewModel>> GetAllUsers()
        {

            var users = await _users.FindAsync(usr => true);
            var result = users?.ToList();

            var userViewModel = _mapper.Map<List<UserViewModel>>(result);

            return userViewModel;
        }

        public async Task<UserViewModel> GetUserByEmail(string email)
        {
            var user = await _users.FindAsync<User>(emp => emp.Email.Equals(email));

            var result = await user.FirstOrDefaultAsync();

            UserViewModel userViewModel = _mapper.Map<UserViewModel>(result);

            return userViewModel;
        }

        private static int GetNextSequence(List<UserViewModel> exisitingRecords)
        {
           var preSeq = exisitingRecords.Max(x => x.Seq);
            return preSeq + 1;
        }


        public async Task<UserResponse> Register(User usr)
        {
            var responsse = new UserResponse() { Email = usr.Email, LoginId = usr.LoginId, Errors = new List<string>() };

            var isemailexisit = await GetUserByEmail(usr.Email);
            if(isemailexisit != null)
            {
                responsse.Errors.Add("Email is already exisit");
                responsse.LoginId = null;

                return responsse;
            }

            usr.LoginId = usr.FirstName + Guid.NewGuid().ToString();
            

            var exisitingRecords = await GetAllUsers();
            if(exisitingRecords == null || !exisitingRecords.Any())
            {
                usr.Seq = 0;
            }
            else
            {
                usr.Seq =  GetNextSequence(exisitingRecords);
            }

            try
            {
                //Hash the password before storing
                usr.Password = CryptoGraphy.GetHash(usr.Password);
                await _users.InsertOneAsync(usr);
                var tokenContainer = jwtAuthenticationManager.Authenticate(usr.Email, usr.Password);
                responsse.Token = tokenContainer.Token;
                responsse.RefreshToken = tokenContainer.RefreshToken;
                responsse.Seq = usr.Seq;
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
                    responsse.LoginId = null;
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
                userResponse.Errors.Add("Invalid Email ID");
                return userResponse;
            }

            userResponse.Email = user.Email;
            userResponse.LoginId = user.LoginId;
            userResponse.Seq = user.Seq;

            //Create hash value of entered password
            var newHashValue = CryptoGraphy.GetHash(password);


            //Check the hash value of entered password and hash password stored in the DB
            if (CryptoGraphy.CompareHash(newHashValue, user.Password))
            {
                var tokenResponse = jwtAuthenticationManager.Authenticate(user.Email, user.Password);
                userResponse.Token = tokenResponse.Token;
                userResponse.RefreshToken = tokenResponse.RefreshToken;

              
                return userResponse;
            }

            userResponse.LoginId = null;
            userResponse.Errors.Add("Incorrect Pssword");
            return userResponse;
        }

        public async Task<bool> ResetPassword(string email, string newPassword, string contactNumber)
        {
            var user = await GetUserByConatctAndEmail(email, contactNumber);
            if (user != null)
            {
                var hashPassword = CryptoGraphy.GetHash(newPassword);
                var filter = new BsonDocument("email", email);
                var update = Builders<User>.Update.Set("password", hashPassword);
                await _users.FindOneAndUpdateAsync(filter, update);
                return true;
            }

            return false;
        }
    }
}

