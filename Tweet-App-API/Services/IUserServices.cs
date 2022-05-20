using System.Collections.Generic;
using System.Threading.Tasks;
using Tweet_App_API.Model;

namespace Tweet_App_API.Services
{
    public interface IUserServices
    {
        public Task<List<User>> Get();

        public Task<User> GetUserByEmail(string id);

        public Task<UserResponse> Register(User usr);

        public Task<UserResponse> LoginUser(string loginId, string password);

        public bool ResetPassword(string userId, string newPassword);
    }
}
