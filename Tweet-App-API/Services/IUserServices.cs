using System.Collections.Generic;
using System.Threading.Tasks;
using Tweet_App_API.Model;

namespace Tweet_App_API.Services
{
    public interface IUserServices
    {
        public Task<List<UserViewModel>> GetAllUsers();

        public Task<UserViewModel> GetUserByEmail(string email);

        public Task<UserResponse> Register(User usr);

        public Task<UserResponse> LoginUser(string email, string password);

        public Task<bool> ResetPassword(string email, string newPassword, string contactNumber);
    }
}
