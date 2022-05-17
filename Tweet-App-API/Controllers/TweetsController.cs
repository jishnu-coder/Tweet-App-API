using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.Model;
using Tweet_App_API.Services;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Tweet_App_API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(User user)
        {
            var result = await _userService.Register(user);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("Login")]
        public async Task<IActionResult> Login(string loginId,string password)
        {
         
            var result = await _userService.LoginUser(loginId, password);
            return Ok(result);
        }

        [HttpGet("{userId}/forget-Password")]
        public string ResetPassWord(string userId,string newPassword)
        {
            if( _userService.ResetPassword(userId, newPassword))
            {              
                _logger.LogInformation($"{userId} suucessfully update the password");
                return "Password Successfully Changed";
            }

            return "Login Id Incorrect";
        }
        
        
        [HttpGet("users/all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok( await _userService.Get());
        }

        [HttpGet("search/{username}")]
        public IActionResult GetUserByName(string username)
        {            
            return  Ok(_userService.GetUserById(username));
        }

       
        [Authorize(Policy = "whocanedit")]
        [HttpPost("{userid}/Add")]
        public async Task<Tweet> CreateTweet(Tweet tweet)
        {
            return await _tweetService.PostTweet(tweet);
        }

        [HttpGet("all")]
        public List<Tweet> GetAllTweet()
        {
            return _tweetService.GetAll(); 
        }

        [HttpGet("{userid}")]
        public async Task< List<Tweet>> GetTweetById(string userid)
        {
            return await _tweetService.GetByUserId(userid);
        }

       
        [Authorize(Policy = "whocanedit")]    
        [HttpPut("{userid}/update/{id}")]
        public async Task<IActionResult> UpdateTweet(string userid,string id , Tweet tweet)
        {
            //string userId = User.Claims.First().Value;
            var result = await _tweetService.UpdateTweet(id, tweet);
            return Ok(result);
        }

      
        [Authorize(Policy = "whocanedit")]
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
