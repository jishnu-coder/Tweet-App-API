using System;
using System.Runtime.Serialization;

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
