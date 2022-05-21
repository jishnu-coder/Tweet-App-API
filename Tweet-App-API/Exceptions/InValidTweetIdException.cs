using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet_App_API.Exceptions
{
    [Serializable]
    public class InvalidTweetIdException : Exception
    {
        public InvalidTweetIdException() { }

        public InvalidTweetIdException(string message)
            : base(message)
        {

        }
    }
}
