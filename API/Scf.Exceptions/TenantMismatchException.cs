using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf
{


    [Serializable]
    public class TenantMismatchException : Exception
    {
        public TenantMismatchException(string message)
            : base(message)
        { }

        public TenantMismatchException()
        {
        }

        public TenantMismatchException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}
