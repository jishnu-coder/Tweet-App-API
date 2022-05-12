using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.Model;

namespace Tweet_App_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TweetsController : ControllerBase
    {
        private readonly ILogger<TweetsController> _logger;

        public static List<User> userList = new List<User>();

        public TweetsController(ILogger<TweetsController> logger)
        {
            _logger = logger;
        }

        [HttpPost("Register")]
        public List<User> Register(User user)
        {
            userList.Add(user);
            return userList;
        }

        [HttpGet]
        public List<User> GetAll()
        {
            return userList;
        }

    }
}
