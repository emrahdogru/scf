using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Scf.Domain
{
    public abstract class TenantEntity : Entity, ITenantEntity
    {
        private Tenant? tenant = null;

        public TenantEntity(DomainContext context, Tenant tenant)
            : base(context)
        {
            this.tenant = tenant;
            this.TenantId = tenant.Id;
        }

        [BsonElement]
        internal ObjectId TenantId { get; private set; }

        [BsonIgnore]
        public Tenant Tenant
        {
            get
            {
                tenant ??= Context.Tenants.FindAsync(this.TenantId).Result;
                return tenant ?? throw new EntityNotFountException(nameof(Tenant), this.TenantId);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Tenant));

                tenant = Context.Tenants.FindAsync(value.Id).Result ?? throw new EntityNotFountException(nameof(Tenant), this.TenantId); ;


                TenantId = value.Id;
            }
        }

        ObjectId ITenantEntity.TenantId => this.TenantId;

        ITenant ITenantEntity.Tenant { get => this.Tenant; }

    }
}
