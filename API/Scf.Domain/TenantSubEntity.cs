using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain
{
    public abstract class TenantSubEntity : Entity, ITenantEntity
    {
        private Tenant? tenant = null;

        public TenantSubEntity(ITenantEntity parentEntity)
            : base((DomainContext)parentEntity.Context)
        {
            this.tenant = parentEntity.Tenant as Tenant;
            this.ParentEntity = parentEntity;
        }

        protected internal ITenantEntity ParentEntity { get; set; }

        [BsonIgnore]
        internal ObjectId TenantId => ParentEntity.TenantId;

        [BsonIgnore]
        public Tenant Tenant
        {
            get
            {
                return ParentEntity.Tenant as Tenant ?? Context.Tenants.FindOrThrowAsync(this.TenantId).Result;
            }
        }

        ObjectId ITenantEntity.TenantId => this.TenantId;

        ITenant ITenantEntity.Tenant { get => this.Tenant; }

    }
}
