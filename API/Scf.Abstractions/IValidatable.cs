using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models
{
    internal interface IValidatable
    {
        Task Validate();
    }
}
