using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.TenantModels
{
    /// <summary>
    /// Hizmet
    /// </summary>
    public class ItemAsService : Item
    {
        public ItemAsService(DomainContext context, Tenant tenant) : base(context, tenant)
        {
        }
    }
}
