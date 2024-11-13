using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain
{
    public interface ITenant : IEntity
    {
        string Code { get; set; }
        string Title { get; set; }
        Languages DefaultLanguage { get; set; }
    }
}
