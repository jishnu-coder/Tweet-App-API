﻿using Microsoft.AspNetCore.Http;
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
        private readonly UserServices _userService;

        public static List<User> userList = new List<User>();

        public TweetsController(ILogger<TweetsController> logger, UserServices userServices)
        {
            _logger = logger;
            _userService = userServices;
        }

        [HttpPost("Register")]
        public User Register(User user)
        {
            _userService.Post(user);
            return user;
        }

        [HttpGet]
        public List<User> GetAll()
        {
            return _userService.Get();
        }

    }
}
