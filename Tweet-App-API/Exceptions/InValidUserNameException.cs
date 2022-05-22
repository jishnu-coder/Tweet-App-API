using System;
using System.Runtime.Serialization;

namespace Tweet_App_API.Exceptions
{
    [Serializable]
    public class InvalidUserNameException : Exception
    {
        public InvalidUserNameException() { }

        public InvalidUserNameException(string message)
            : base(message)
        {

        }

        protected InvalidUserNameException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
