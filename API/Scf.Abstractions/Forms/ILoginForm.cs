using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Forms
{
    public interface ILoginForm
    {
        string Email { get; set; }
        string Password { get; set; }   
    }
}
