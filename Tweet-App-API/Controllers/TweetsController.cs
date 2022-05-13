using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.Model;
using Tweet_App_API.Services;

namespace Tweet_App_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TweetsController : ControllerBase
    {
        private readonly ILogger<TweetsController> _logger;
        private readonly IUserServices _userService;
        private readonly ITweetService _tweetService;

        public static List<User> userList = new List<User>();

        public TweetsController(ILogger<TweetsController> logger, IUserServices userServices , ITweetService tweetService)
        {
            _logger = logger;
            _userService = userServices;
            _tweetService = tweetService;
        }

        [HttpPost("Register")]
        public User Register(User user)
        {
            _userService.Post(user);
            return user;
        }

        [HttpGet("Login")]
        public User Login(string loginId,string password)
        {
            var result = _userService.LoginUser(loginId, password);
            return result;
        }

        [HttpGet("{userId}/forget-Password")]
        public User ResetPassWord(string userId,string newPassword)
        {
            return _userService.ResetPassword(userId, newPassword);
        }
        

        [HttpGet]
        public List<User> GetAll()
        {
            return _userService.Get();
        }

        [HttpPost("{userid}/Add")]
        public Tweet CreateTweet(Tweet tweet)
        {
            return _tweetService.PostTweet(tweet);
        }

    }
}
