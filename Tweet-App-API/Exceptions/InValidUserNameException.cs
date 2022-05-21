using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet_App_API.Exceptions
{
    [Serializable]
    public class InvalidUserNameException :Exception
    {
        public InvalidUserNameException() { }

        public InvalidUserNameException(string message)
            : base(message)
        {

        }
    }
}
