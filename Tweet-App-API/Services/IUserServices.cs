using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.Model;

namespace Tweet_App_API.Services
{
    public interface IUserServices
    {
        public List<User> Get();

        public User GetUserById(string id);

        public User Post(User usr);

        public User LoginUser(string loginId, string password);

        public User ResetPassword(string userId, string newPassword);
    }
}
