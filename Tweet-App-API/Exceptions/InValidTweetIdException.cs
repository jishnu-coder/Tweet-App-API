using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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

        protected InvalidTweetIdException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
