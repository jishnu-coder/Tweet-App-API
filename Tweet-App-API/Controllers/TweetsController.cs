using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.Model;
using Tweet_App_API.Services;
using MongoDB.Driver;

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
        public async Task<UserResponse> Register(User user)
        {
            return await _userService.Register(user);            
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
        

        [HttpGet("users/all")]
        public List<User> GetAll()
        {
            return _userService.Get();
        }

        [HttpGet("search/{username}")]
        public List<User> GetUserByName(string username)
        {
            return _userService.GetUserById(username);
        }


        [HttpPost("{userid}/Add")]
        public Tweet CreateTweet(Tweet tweet)
        {
            return _tweetService.PostTweet(tweet);
        }

        [HttpGet("all")]
        public List<Tweet> GetAllTweet()
        {
            return _tweetService.GetAll(); 
        }

        [HttpGet("{userid}")]
        public List<Tweet> GetTweetById(string userid)
        {
            return _tweetService.GetByUserId(userid);
        }

        [HttpPut("{userid}/update/{id}")]
        public async Task<Tweet> UpdateTweet(string userid,string id , Tweet tweet)
        {
            return await _tweetService.UpdateTweet(id, tweet);
        }

        [HttpDelete("{userid}/delete/{id}")]
        public async Task<DeleteResult> DeleteTweet(string userid, string id)
        {
            return await _tweetService.DeleteTweet(id);
        }

        [HttpPut("{userid}/Like/{id}")]
        public async Task<Tweet> LikeTweet(string userid, string id)
        {
            return await _tweetService.LikeTweet(userid,id);
        }

        [HttpPost("{userid}/reply/{id}")]
        public async Task<Tweet> ReplyTweet(string userid, string id , TweetReply tweetReply)
        {
            return await _tweetService.ReplyTweet(userid, id ,tweetReply);
        }
    }
}
