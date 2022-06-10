using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tweet_App_API.Exceptions;
using Tweet_App_API.Kafka;
using Tweet_App_API.Model;
using Tweet_App_API.Services;

namespace Tweet_App_API.Controllers
{

  
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [EnableCors]
    [Route("[controller]")]
    [ApiController]
    public class TweetsController : ControllerBase
    {
        private readonly ILogger<TweetsController> _logger; //Use Log4Net framework for logging -> log4net.config
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
            _logger.LogInformation($"{user.Email} Sign Up Request");
            if (ModelState.IsValid)
            {
                var result = await _userService.Register(user);

                _logger.LogInformation($"{user.Email} suvccesfully Registed");
                return Ok(result);
            }
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginModel userLoginModel)
        {
            _logger.LogInformation($"{userLoginModel.UserName} Log in request");
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUser(userLoginModel.UserName, userLoginModel.Password);

                _logger.LogInformation($"{userLoginModel.UserName} User succesfully log in");

                return Ok(result);
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("{userName}/forget-Password")]
        public async Task<IActionResult> ResetPassWord(string userName, UserLoginModel userLoginModel)
        {
            _logger.LogInformation($"{userLoginModel.UserName} Reset password Request");
            if (ModelState.IsValid)
            {
                if (await _userService.ResetPassword(userLoginModel.UserName, userLoginModel.Password, userLoginModel.ContactNumber))
                {
                    _logger.LogInformation($"{userLoginModel.UserName} suucessfully update the password");
                    return Ok("Password Successfully Changed");
                }

                return BadRequest("Login Id or Contact Number Incorrect");
            }

            return BadRequest("Invalid Model");
        }

        [AllowAnonymous]
        [HttpGet("users/all")]
        public async Task<List<UserViewModel>> GetAllUsers()
        {
            _logger.LogInformation(" Get All users Request");
            //Return all users
            var response = await _userService.GetAllUsers();
            return response;
        }

        [HttpGet("search/{username}")]
        public async Task<IActionResult> GetUserByName(string username) //Email is considered as userName
        {

            var user = await _userService.GetUserByEmail(username);

            return Ok(user);
        }


     
        [HttpPost("{userName}/Add")]
        public async Task<IActionResult> CreateTweet(string userName, Tweet tweet)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _tweetService.PostTweet(tweet);

                    _logger.LogInformation($"{userName} sucessfully create a tweet with id {result.TweetId}");
                   

                    return Ok("Tweet Posted Succesfully");
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.ToString());
                    return BadRequest(ex.ToString());
                }
            }
            return BadRequest("Invalid Model");
        }

        [HttpGet("all")]
        public List<Tweet> GetAllTweet()
        {
            var res = _tweetService.GetAll();
            return res;
        }

        [HttpGet("{userName}")]
        public async Task<IActionResult> GetTweetById(string userName)
        {
            try
            {
                var result = await _tweetService.GetTweetsByUserId(userName);
                return Ok(result);
            }
            catch (InvalidUserNameException ex)
            {
                _logger.LogInformation(ex.ToString());
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

                _logger.LogInformation($"{userName} sucessfully Update the tweet {id}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }


        [Authorize(Policy = "whocanedit")]
        [HttpDelete("{userName}/delete/{id}")]
        public async Task<DeleteResult> DeleteTweet(string userName, string id)
        {
            _logger.LogInformation($"{userName} sucessfully Delete the tweet {id}");
            return await _tweetService.DeleteTweet(id);
        }

        [Authorize(Policy = "whocanedit")]
        [HttpPut("{userName}/Like/{id}")]
        public async Task<Tweet> LikeTweet(string userName, string id)
        {
            _logger.LogInformation($"{userName} sucessfully Liked the tweet {id}");
            return await _tweetService.LikeTweet(userName, id);
        }

        [Authorize(Policy = "whocanedit")]
        [HttpPost("{userName}/reply/{id}")]
        public async Task<IActionResult> ReplyTweet(string userName, string id, TweetReply tweetReply)
        {
            try
            {
                var result = await _tweetService.ReplyTweet(userName, id, tweetReply);
                _logger.LogInformation($"{userName} sucessfully Replied the tweet {id} with reply message {tweetReply.ReplyMessage}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("tweetId/{id}")]
        public async Task<IActionResult> GetTweetByTweetId(string id)
        {
            var tweet = await _tweetService.GetTweetByTweetId(id);

            return Ok(tweet);
        }
    }
}
