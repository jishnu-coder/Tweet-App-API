using System;
using System.Runtime.Serialization;

namespace Tweet_App_API.Exceptions
{
    [Serializable]
    public class TweetLengthExceedException : Exception
    {
        public TweetLengthExceedException() { }

        public TweetLengthExceedException(string message)
            : base(message)
        {

        }

        protected TweetLengthExceedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }
}
