using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.Enums
{
    public enum DiscountType
    {
        /// <summary>
        /// Matrahtan yüzde
        /// </summary>
        PercentFromBase,

        /// <summary>
        /// Önceki ara toplamdan yüzde
        /// </summary>
        PercentFromPreviousSubtotal,

        /// <summary>
        /// Tutar
        /// </summary>
        Amount
    }
}
