using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_App_API.Model;

namespace Tweet_App_API.Services
{
    public interface ITweetService
    {
        public Tweet PostTweet(Tweet tweet);

     //   public Tweet GetTweet();
    }
}
