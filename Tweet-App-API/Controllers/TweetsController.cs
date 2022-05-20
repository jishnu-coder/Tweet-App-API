using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tweet_App_API.Model;
using Tweet_App_API.Services;

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

        public TweetsController(ILogger<TweetsController> logger, IUserServices userServices, ITweetService tweetService)
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
        public async Task<IActionResult> Login(string userName, string password)
        {

            var result = await _userService.LoginUser(userName, password);
            return Ok(result);
        }

        [HttpGet("{userName}/forget-Password")]
        public string ResetPassWord(string userName, string newPassword)
        {
            if (_userService.ResetPassword(userName, newPassword))
            {
                _logger.LogInformation($"{userName} suucessfully update the password");
                return "Password Successfully Changed";
            }

            return "Login Id Incorrect";
        }


        [HttpGet("users/all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userService.Get());
        }

        [HttpGet("search/{username}")]
        public IActionResult GetUserByName(string username)
        {
            return Ok(_userService.GetUserByEmail(username));
        }


        [Authorize(Policy = "whocanedit")]
        [HttpPost("{userName}/Add")]
        public async Task<Tweet> CreateTweet(Tweet tweet)
        {
            return await _tweetService.PostTweet(tweet);
        }

        [HttpGet("all")]
        public List<Tweet> GetAllTweet()
        {
            return _tweetService.GetAll();
        }

        [HttpGet("{userName}")]
        public async Task<List<Tweet>> GetTweetById(string userName)
        {
            return await _tweetService.GetByUserId(userName);
        }


        [Authorize(Policy = "whocanedit")]
        [HttpPut("{userName}/update/{id}")]
        public async Task<IActionResult> UpdateTweet(string userName, string id, Tweet tweet)
        {
            //string userId = User.Claims.First().Value;
            var result = await _tweetService.UpdateTweet(id, tweet);
            return Ok(result);
        }


        [Authorize(Policy = "whocanedit")]
        [HttpDelete("{userName}/delete/{id}")]
        public async Task<DeleteResult> DeleteTweet(string userName, string id)
        {
            return await _tweetService.DeleteTweet(id);
        }

        [Authorize(Policy = "whocanedit")]
        [HttpPut("{userName}/Like/{id}")]
        public async Task<Tweet> LikeTweet(string userName, string id)
        {
            return await _tweetService.LikeTweet(userName, id);
        }

        [Authorize(Policy = "whocanedit")]
        [HttpPost("{userid}/reply/{id}")]
        public async Task<Tweet> ReplyTweet(string userName, string id, TweetReply tweetReply)
        {
            return await _tweetService.ReplyTweet(userName, id, tweetReply);
        }
    }
}
