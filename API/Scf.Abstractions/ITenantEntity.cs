using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain
{
    public  interface ITenantEntity : IEntity
    {
        ObjectId TenantId { get; }
        ITenant? Tenant { get; }
    }
}
