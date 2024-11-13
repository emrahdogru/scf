using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.Interfaces
{
    public interface ITaxApplyable
    {
        /// <summary>
        /// Vergi matrahı
        /// </summary>
        double BaseAmount { get; }
    }
}
