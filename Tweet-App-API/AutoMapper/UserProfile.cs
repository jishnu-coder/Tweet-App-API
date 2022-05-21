using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.Model;

namespace Tweet_App_API.AutoMapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>();

            //CreateMap<List<User>, List<UserViewModel>>();
        }
    }
}
