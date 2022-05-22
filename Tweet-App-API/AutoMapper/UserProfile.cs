using AutoMapper;
using Tweet_App_API.Model;

namespace Tweet_App_API.AutoMapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>();

        }
    }
}
