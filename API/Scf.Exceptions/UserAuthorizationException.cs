using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf
{
    [Serializable]
    public class UserAuthorizationException : Exception
    {
        public UserAuthorizationException(string message, Exception innerException)
             : base(message, innerException)
        {

        }

        public UserAuthorizationException(string message)
            : base(message)
        { }

        public UserAuthorizationException() 
            :base("Unauthorized access")
        { }
    }
}
