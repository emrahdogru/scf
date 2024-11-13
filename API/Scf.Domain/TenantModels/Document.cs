using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.TenantModels
{
    [BsonKnownTypes(
        typeof(Invoice)
        )]
    public abstract partial class Document : TenantEntity
    {
        protected Document(DomainContext context, Tenant tenant) : base(context, tenant)
        {
        }

        public string Type
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public DateTime Date { get; set; }
        public string? DocumentNumber { get; set; }
        public IEnumerable<string> Tags { get; set; } = Array.Empty<string>();

        protected void SetParents(IEnumerable<TenantSubEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.ParentEntity = this;
            }
        }

        public abstract class DocumentRow : TenantSubEntity
        {
            protected DocumentRow(Document parentEntity) : base(parentEntity)
            {

            }
        }
    }
}
