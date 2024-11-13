using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.TenantModels
{
    [BsonKnownTypes(
        typeof(AccountTrade),
        typeof(AccountPersonal)
    )]
    public abstract class Account : BaseCard
    {
        protected Account(DomainContext context, Tenant tenant) : base(context, tenant)
        {
        }
    }
}
