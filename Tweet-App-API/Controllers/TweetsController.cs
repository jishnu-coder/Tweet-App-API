using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
            if( ModelState.IsValid )
            {
                var result = await _userService.Register(user);
                return Ok(result);
            }
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet("Login")]
        public async Task<IActionResult> Login(UserLoginModel userLoginModel)
        {
           if(ModelState.IsValid)
            {
                var result = await _userService.LoginUser(userLoginModel.UserName, userLoginModel.Password);
                return Ok(result);
            }

            return BadRequest();            
        }

        [HttpGet("{userName}/forget-Password")]
        public IActionResult ResetPassWord(string userName,UserLoginModel userLoginModel)
        {
            if (ModelState.IsValid)
            {
                if (_userService.ResetPassword(userLoginModel.UserName, userLoginModel.Password))
                {
                    _logger.LogInformation($"{userLoginModel.UserName} suucessfully update the password");
                    return Ok("Password Successfully Changed");
                }

                return BadRequest("Login Id Incorrect");
            }
                
            return BadRequest("Invalid Model");
        }


        [HttpGet("users/all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userService.Get());
        }

        [HttpGet("search/{username}")]
        public async  Task<IActionResult> GetUserByName(string username)
        {
          
             var user = await _userService.GetUserByEmail(username);

            return Ok(user);
        }


        [Authorize(Policy = "whocanedit")]
        [HttpPost("{userName}/Add")]
        public async Task<IActionResult> CreateTweet(string userName,Tweet tweet)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var result = await _tweetService.PostTweet(tweet);
                    return Ok("Tweet Posted Succesfully");
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.ToString());
                }
            }
            return BadRequest("Invalid Model");
        }

        [HttpGet("all")]
        public List<Tweet> GetAllTweet()
        {
            return _tweetService.GetAll();
        }

        [HttpGet("{userName}")]
        public async Task<IActionResult> GetTweetById(string userName)
        {
            try
            {
                var result = await _tweetService.GetTweetsByUserId(userName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }


        [Authorize(Policy = "whocanedit")]
        [HttpPut("{userName}/update/{id}")]
        public async Task<IActionResult> UpdateTweet(string userName, string id, Tweet tweet)
        {
            try
            {
                var result = await _tweetService.UpdateTweet(userName, id, tweet);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
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
        [HttpPost("{userName}/reply/{id}")]
        public async Task<IActionResult> ReplyTweet(string userName, string id, TweetReply tweetReply)
        {
            try
            {
                var result = await _tweetService.ReplyTweet(userName, id, tweetReply);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
