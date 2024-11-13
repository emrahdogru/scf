using Scf.LanguageResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Scf
{
    [Serializable]
    public class UserException : Exception
    {
        public UserException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public UserException(string message)
            : base(message)
        { }
    }
}
