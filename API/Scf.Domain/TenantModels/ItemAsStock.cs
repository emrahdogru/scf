using MongoDB.Bson.Serialization.Attributes;
using Scf.Domain.SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.TenantModels
{
    /// <summary>
    /// Stok
    /// </summary>
    public class ItemAsStock : Item
    {
        public ItemAsStock(DomainContext context, Tenant tenant) : base(context, tenant)
        {

        }



    }
}
