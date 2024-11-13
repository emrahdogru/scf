using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain
{
    public class AppSettings
    {
        /// <summary>
        /// Dakika cinsinden token için mutlak zaman aşımı süresi. Bu süre dolduğunda her halükarda token geçerliliğini yitirir.
        /// </summary>
        public int? TokenAbsoluteTimeout { get; set; }

        public string UIDomain { get; set; } = null!;
    }
}
