using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.Interfaces
{
    public interface IDiscountApplyable
    {
        /// <summary>
        /// İskonto matrahı
        /// </summary>
        double BaseAmount { get; }
    }
}
