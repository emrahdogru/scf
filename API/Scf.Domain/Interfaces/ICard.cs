using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.Interfaces
{
    public interface ICard
    {
        public string Code { get; }
        public string Name { get; }
    }
}
