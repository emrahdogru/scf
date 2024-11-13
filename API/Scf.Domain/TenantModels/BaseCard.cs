using Scf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.TenantModels
{
    public abstract class BaseCard : TenantEntity, IKart
    {
        public BaseCard(DomainContext context, Tenant tenant) : base(context, tenant)
        {
        }

        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
